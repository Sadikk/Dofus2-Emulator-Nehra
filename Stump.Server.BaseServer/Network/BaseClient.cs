using NLog;
using Stump.Core.Attributes;
using Stump.Core.Extensions;
using Stump.Core.IO;
using Stump.Core.Pool.New;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Messages;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace Stump.Server.BaseServer.Network
{
	public abstract class BaseClient : IDisposable, IPacketReceiver
	{
		private readonly ClientManager m_clientManager;

		[Variable(DefinableRunning = true)]
		public static bool LogPackets = false;

		private readonly Logger logger = LogManager.GetCurrentClassLogger();
		private readonly BigEndianReader m_buffer = new BigEndianReader();
		private readonly object m_lock = new object();

        private Message m_lastMessage;
		private MessagePart m_currentMessage;
		private bool m_disconnecting;
		private bool m_onDisconnectCalled = false;
		private int m_offset;
		private int m_remainingLength;
		private BufferSegment m_bufferSegment;
		private long m_bytesReceived;
		private long m_totalBytesReceived;
		public event Action<BaseClient, Message> MessageReceived;
		public event Action<BaseClient, Message> MessageSent;

		public Socket Socket
		{
			get;
			private set;
		}
		public bool CanReceive
		{
			get;
			protected set;
		}
		public bool Connected
		{
			get
			{
				return this.Socket != null && this.Socket.Connected && !this.m_disconnecting;
			}
		}
		public string IP
		{
			get;
			private set;
		}
		public DateTime LastActivity
		{
			get;
			private set;
		}
        public Message LastMessage
        {
            get;
            set;
        }

		protected BaseClient(Socket socket, ClientManager clientManager)
		{
			this.Socket = socket;
			this.IP = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
			this.m_clientManager = clientManager;
			this.m_bufferSegment = BufferManager.Default.CheckOut();
		}
		protected BaseClient(Socket socket) : this(socket, Singleton<ClientManager>.Instance)
		{
		}
		public virtual void Send(Message message)
		{
			SegmentStream stream = BufferManager.Default.CheckOutStream();
            CustomDataWriter writer = new CustomDataWriter(stream);
			message.Pack(writer);
			this.Send(stream);
			this.OnMessageSent(message);
		}
		public void Send(SegmentStream stream)
		{
			if (this.Socket != null && this.Connected)
			{
				SocketAsyncEventArgs socketAsyncEventArgs = ObjectPoolMgr.ObtainObject<SocketAsyncEventArgs>();
				socketAsyncEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(this.OnSendCompleted);
				socketAsyncEventArgs.SetBuffer(stream.Segment.Buffer.Array, stream.Segment.Offset, (int)stream.Position);
				socketAsyncEventArgs.UserToken = stream;
				if (!this.Socket.SendAsync(socketAsyncEventArgs))
				{
					socketAsyncEventArgs.Completed -= new EventHandler<SocketAsyncEventArgs>(this.OnSendCompleted);
					stream.Dispose();
					ObjectPoolMgr.ReleaseObject<SocketAsyncEventArgs>(socketAsyncEventArgs);
				}
				this.LastActivity = DateTime.Now;
			}
		}
		private void OnSendCompleted(object sender, SocketAsyncEventArgs e)
		{
			e.Completed -= new EventHandler<SocketAsyncEventArgs>(this.OnSendCompleted);
			((SegmentStream)e.UserToken).Dispose();
			ObjectPoolMgr.ReleaseObject<SocketAsyncEventArgs>(e);
		}
		protected virtual void OnMessageReceived(Message message)
		{
			if (this.MessageReceived != null)
			{
				this.MessageReceived(this, message);
			}
		}
		public virtual void OnMessageSent(Message message)
		{
			if (BaseClient.LogPackets)
			{
				Console.WriteLine("(SEND) {0} : {1}", this, message);
			}
			if (this.MessageSent != null)
			{
				this.MessageSent(this, message);
			}
		}
		public void BeginReceive()
		{
			if (!this.CanReceive)
			{
				throw new Exception("Cannot receive packet : CanReceive is false");
			}
			this.ResumeReceive();
		}
		private void ResumeReceive()
		{
			if (this.Socket != null && this.Socket.Connected)
			{
				SocketAsyncEventArgs socketAsyncEventArgs = Singleton<ClientManager>.Instance.PopSocketArg();
				socketAsyncEventArgs.SetBuffer(this.m_bufferSegment.Buffer.Array, this.m_bufferSegment.Offset + this.m_offset, ClientManager.BufferSize - this.m_offset);
				socketAsyncEventArgs.UserToken = this;
				socketAsyncEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(this.ProcessReceive);
				if (!this.Socket.ReceiveAsync(socketAsyncEventArgs))
				{
					this.ProcessReceive(this, socketAsyncEventArgs);
				}
			}
		}
		private void ProcessReceive(object sender, SocketAsyncEventArgs e)
		{
			try
			{
				int bytesTransferred = e.BytesTransferred;
				if (bytesTransferred == 0)
				{
					this.Disconnect();
				}
				else
				{
					this.m_bytesReceived += (long)((ulong)bytesTransferred);
					Interlocked.Add(ref this.m_totalBytesReceived, (long)bytesTransferred);
					this.m_remainingLength += bytesTransferred;
					if (this.BuildMessage(this.m_bufferSegment))
					{
						this.m_offset = 0;
						this.m_bufferSegment.DecrementUsage();
						this.m_bufferSegment = BufferManager.Default.CheckOut();
					}
					else
					{
						this.EnsureBuffer();
					}
					this.ResumeReceive();
				}
			}
			catch (Exception ex)
			{
				this.logger.Error(string.Concat(new object[]
				{
					"Forced disconnection ",
					this.ToString(),
					" : ",
					ex
				}));
				this.Disconnect();
			}
			finally
			{
				e.Completed -= new EventHandler<SocketAsyncEventArgs>(this.ProcessReceive);
				Singleton<ClientManager>.Instance.PushSocketArg(e);
			}
		}
		protected virtual bool BuildMessage(BufferSegment buffer)
		{
			if (this.m_currentMessage == null)
			{
				this.m_currentMessage = new MessagePart(false);
			}
            CustomDataReader fastBigEndianReader = new CustomDataReader(buffer);
            fastBigEndianReader.Seek((buffer.Offset + this.m_offset), SeekOrigin.Current);
            var maxPosition = (long)(buffer.Offset + this.m_offset + this.m_remainingLength);
			bool result;
			if (this.m_currentMessage.Build(fastBigEndianReader))
			{
				long position = fastBigEndianReader.Position;
				if (this.m_currentMessage.ExceedBufferSize)
				{
					fastBigEndianReader = new CustomDataReader(this.m_currentMessage.Data);
				}
				else
				{
                    maxPosition = (long)buffer.Offset + position + (long)this.m_currentMessage.Length.Value;
				}
				Message message;
				try
				{
					message = MessageReceiver.BuildMessage((ushort)this.m_currentMessage.MessageId.Value, fastBigEndianReader);
				}
				catch (Exception)
				{
					if (this.m_currentMessage.ReadData)
					{
						this.logger.Debug("Message = {0}", this.m_currentMessage.Data.ToString(" "));
					}
					else
					{
						fastBigEndianReader.Seek((int)position, SeekOrigin.Begin);
						this.logger.Debug("Message = {0}", fastBigEndianReader.ReadBytes(this.m_currentMessage.Length.Value).ToString(" "));
					}
					throw;
				}
				this.LastActivity = DateTime.Now;
				if (BaseClient.LogPackets)
				{
					Console.WriteLine("(RECV) {0} : {1}", this, message);
				}
				this.OnMessageReceived(message);
				this.m_remainingLength -= (int)fastBigEndianReader.Position - buffer.Offset - this.m_offset;
				this.m_offset = (int)fastBigEndianReader.Position - buffer.Offset;
				this.m_currentMessage = null;
				result = (this.m_remainingLength <= 0 || this.BuildMessage(buffer));
			}
			else
			{
				this.m_offset = (int)fastBigEndianReader.Position;
                this.m_remainingLength = (int)(maxPosition - (long)this.m_offset);
				result = false;
			}
			return result;
		}
		protected void EnsureBuffer()
		{
			BufferSegment bufferSegment = BufferManager.Default.CheckOut();
			Array.Copy(this.m_bufferSegment.Buffer.Array, this.m_bufferSegment.Offset + this.m_offset, bufferSegment.Buffer.Array, bufferSegment.Offset, this.m_remainingLength);
			this.m_bufferSegment.DecrementUsage();
			this.m_bufferSegment = bufferSegment;
			this.m_offset = 0;
		}
		public void Disconnect()
		{
			if (!this.m_onDisconnectCalled)
			{
				this.m_onDisconnectCalled = true;
				this.m_disconnecting = true;
				try
				{
					this.OnDisconnect();
				}
				catch (Exception argument)
				{
					this.logger.Error<string, Exception>("Exception occurs while disconnecting client {0} : {1}", this.ToString(), argument);
				}
				finally
				{
					Singleton<ClientManager>.Instance.OnClientDisconnected(this);
					this.Dispose();
				}
			}
		}
		public void DisconnectLater(int timeToWait = 0)
		{
			if (timeToWait == 0)
			{
				ServerBase.InstanceAsBase.IOTaskPool.AddMessage(new Action(this.Disconnect));
			}
			else
			{
				ServerBase.InstanceAsBase.IOTaskPool.CallDelayed(timeToWait, new Action(this.Disconnect));
			}
		}
		protected virtual void OnDisconnect()
		{
		}
		~BaseClient()
		{
			this.Dispose(false);
		}
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing)
		{
			if (this.Socket != null && this.Socket.Connected)
			{
				this.Socket.Shutdown(SocketShutdown.Both);
				this.Socket.Close();
			}
			if (this.m_bufferSegment != null)
			{
				this.m_bufferSegment.DecrementUsage();
			}
		}
		public override string ToString()
		{
			return "<" + this.IP + ">";
		}
	}
}
