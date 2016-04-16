using NLog;
using Stump.Core.Attributes;
using Stump.Core.Reflection;
using Stump.Core.Threading;
using Stump.Core.Timers;
using Stump.Server.BaseServer;
using Stump.Server.BaseServer.IPC;
using Stump.Server.BaseServer.IPC.Messages;
using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using System;
using System.Linq;
using System.Net.Sockets;
namespace Stump.Server.WorldServer.Core.IPC
{
	public class IPCAccessor : IPCEntity
	{
		public new delegate void IPCMessageHandler(IPCMessage message);
		public delegate void RequestCallbackDefaultDelegate(IPCMessage unattemptMessage);
		public delegate void RequestCallbackDelegate<in T>(T callbackMessage) where T : IPCMessage;
		public delegate void RequestCallbackErrorDelegate(IPCErrorMessage errorMessage);
		[Variable(DefinableRunning = true)]
		public static int DefaultRequestTimeout = 5;
		[Variable(DefinableRunning = true)]
		public static int TaskPoolInterval = 150;
		[Variable(DefinableRunning = true)]
		public static int UpdateInterval = 10000;
		[Variable]
		public static string RemoteHost = "localhost";
		[Variable]
		public static int RemotePort = 9100;
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();
		private static IPCAccessor m_instance;
		private readonly System.Collections.Generic.Dictionary<System.Type, IPCAccessor.IPCMessageHandler> m_additionalsHandlers = new System.Collections.Generic.Dictionary<System.Type, IPCAccessor.IPCMessageHandler>();
		private readonly TimerEntry m_updateTimer;
		private IPCMessagePart m_messagePart;
		private bool m_requestingAccess;
		private System.Collections.Generic.Dictionary<System.Guid, IIPCRequest> m_requests = new System.Collections.Generic.Dictionary<System.Guid, IIPCRequest>();
		private bool m_wasConnected;
		public event Action<IPCAccessor, IPCMessage> MessageReceived;
		public event Action<IPCAccessor, IPCMessage> MessageSent;
		public event System.Action<IPCAccessor> Connected;
		public event System.Action<IPCAccessor> Disconnected;
		public event System.Action<IPCAccessor> Granted;
		public static IPCAccessor Instance
		{
			get
			{
				IPCAccessor arg_14_0;
				if ((arg_14_0 = IPCAccessor.m_instance) == null)
				{
					arg_14_0 = (IPCAccessor.m_instance = new IPCAccessor());
				}
				return arg_14_0;
			}
			private set
			{
				IPCAccessor.m_instance = value;
			}
		}
		public bool Running
		{
			get;
			set;
		}
		public SelfRunningTaskPool TaskPool
		{
			get;
			private set;
		}
		public Socket Socket
		{
			get;
			private set;
		}
		public bool AccessGranted
		{
			get;
			private set;
		}
		public bool IsReacheable
		{
			get
			{
				return this.Socket != null && this.Socket.Connected;
			}
		}
		public bool IsConnected
		{
			get
			{
				return this.IsReacheable && this.AccessGranted;
			}
		}
		protected override int RequestTimeout
		{
			get
			{
				return IPCAccessor.DefaultRequestTimeout;
			}
		}
		public IPCAccessor()
		{
			this.TaskPool = new SelfRunningTaskPool(IPCAccessor.TaskPoolInterval, "IPCAccessor Task Pool");
			this.m_updateTimer = new TimerEntry(0, IPCAccessor.UpdateInterval, new System.Action<int>(this.Tick));
		}
		private void OnMessageReceived(IPCMessage message)
		{
			Action<IPCAccessor, IPCMessage> messageReceived = this.MessageReceived;
			if (messageReceived != null)
			{
				messageReceived(this, message);
			}
		}
		private void OnMessageSended(IPCMessage message)
		{
			Action<IPCAccessor, IPCMessage> messageSent = this.MessageSent;
			if (messageSent != null)
			{
				messageSent(this, message);
			}
		}
		private void OnClientConnected()
		{
			IPCAccessor.logger.Info("IPC connection etablished");
			System.Action<IPCAccessor> connected = this.Connected;
			if (connected != null)
			{
				connected(this);
			}
		}
		private void OnClientDisconnected()
		{
			this.m_wasConnected = false;
			IPCAccessor.logger.Info("IPC connection lost");
			System.Action<IPCAccessor> disconnected = this.Disconnected;
			if (disconnected != null)
			{
				disconnected(this);
			}
		}
		public void Start()
		{
			if (!this.Running)
			{
				this.Running = true;
				this.TaskPool.Start();
                Tick(0);
				this.m_updateTimer.Start();
				this.TaskPool.AddTimer(this.m_updateTimer);
			}
		}
		public void Stop()
		{
			if (this.Running)
			{
				this.Running = false;
				this.TaskPool.RemoveTimer(this.m_updateTimer);
				this.TaskPool.Stop(false);
				if (this.IsReacheable)
				{
					this.Disconnect();
				}
			}
		}
		private void Connect()
		{
			this.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			this.Socket.Connect(IPCAccessor.RemoteHost, IPCAccessor.RemotePort);
			this.OnClientConnected();
			this.ReceiveLoop();
		}
		private void OnAccessGranted(CommonOKMessage msg)
		{
			this.m_requestingAccess = false;
			this.AccessGranted = true;
			IPCAccessor.logger.Info("Access to auth. server granted");
			System.Action<IPCAccessor> granted = this.Granted;
			if (granted != null)
			{
				granted(this);
			}
		}
		private void OnAccessDenied(IPCErrorMessage error)
		{
			this.m_requestingAccess = false;
			if (!(error is IPCErrorTimeoutMessage))
			{
				this.AccessGranted = false;
				IPCAccessor.logger.Error("Access to auth. server denied ! Reason : {0}", error.Message);
				ServerBase<WorldServer>.Instance.Shutdown();
			}
		}
		private void Disconnect()
		{
			try
			{
				this.Close();
			}
			finally
			{
				this.OnClientDisconnected();
			}
		}
		private void Tick(int dt)
		{
			if (!this.Running)
			{
				if (this.IsReacheable)
				{
					this.Disconnect();
				}
			}
			else
			{
				if (!this.IsReacheable)
				{
					if (!this.m_requestingAccess)
					{
						if (this.m_wasConnected)
						{
							this.Disconnect();
						}
						IPCAccessor.logger.Info("Attempt connection");
						try
						{
							this.Connect();
						}
						catch (System.Exception)
						{
							IPCAccessor.logger.Error<string, int, int>("Connection to {0}:{1} failed. Try again in {2}s", IPCAccessor.RemoteHost, IPCAccessor.RemotePort, IPCAccessor.UpdateInterval / 1000);
							return;
						}
						this.m_requestingAccess = true;
						this.m_wasConnected = true;
						base.SendRequest<CommonOKMessage>(new HandshakeMessage(WorldServer.ServerInformation), new Stump.Server.BaseServer.IPC.RequestCallbackDelegate<CommonOKMessage>(this.OnAccessGranted), new Stump.Server.BaseServer.IPC.RequestCallbackErrorDelegate(this.OnAccessDenied));
					}
				}
				else
				{
					this.Send(new ServerUpdateMessage(ServerBase<WorldServer>.Instance.ClientManager.Count));
				}
			}
		}
		public override void Send(IPCMessage message)
		{
			if (this.IsReacheable)
			{
				SocketAsyncEventArgs socketAsyncEventArgs = new SocketAsyncEventArgs();
				socketAsyncEventArgs.Completed += new System.EventHandler<SocketAsyncEventArgs>(this.OnSendCompleted);
				byte[] array = Singleton<IPCMessageSerializer>.Instance.SerializeWithLength(message);
				socketAsyncEventArgs.SetBuffer(array, 0, array.Length);
				this.Socket.SendAsync(socketAsyncEventArgs);
			}
		}
		private void OnSendCompleted(object sender, SocketAsyncEventArgs e)
		{
			e.Dispose();
		}
		protected override TimerEntry RegisterTimer(System.Action<int> action, int timeout)
		{
			TimerEntry timerEntry = new TimerEntry
			{
				Action = action,
				InitialDelay = timeout
			};
			this.TaskPool.AddTimer(timerEntry);
			return timerEntry;
		}
		private void ReceiveLoop()
		{
			if (this.IsReacheable)
			{
				SocketAsyncEventArgs socketAsyncEventArgs = new SocketAsyncEventArgs();
				socketAsyncEventArgs.Completed += new System.EventHandler<SocketAsyncEventArgs>(this.OnReceiveCompleted);
				socketAsyncEventArgs.SetBuffer(new byte[8192], 0, 8192);
				if (!this.Socket.ReceiveAsync(socketAsyncEventArgs))
				{
					this.ProcessReceiveCompleted(socketAsyncEventArgs);
				}
			}
		}
		private void OnReceiveCompleted(object sender, SocketAsyncEventArgs e)
		{
			switch (e.LastOperation)
			{
			case SocketAsyncOperation.Disconnect:
				this.Disconnect();
				break;
			case SocketAsyncOperation.Receive:
				this.ProcessReceiveCompleted(e);
				break;
			}
		}
		private void ProcessReceiveCompleted(SocketAsyncEventArgs args)
		{
			if (this.IsReacheable)
			{
				if (args.BytesTransferred <= 0 || args.SocketError != SocketError.Success)
				{
					args.Dispose();
					this.Disconnect();
				}
				else
				{
					this.Receive(args.Buffer, args.Offset, args.BytesTransferred);
					args.Dispose();
					this.ReceiveLoop();
				}
			}
		}
		private void Receive(byte[] data, int offset, int count)
		{
			System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(new System.IO.MemoryStream(data, offset, count));
			while (binaryReader.BaseStream.Length - binaryReader.BaseStream.Position > 0L)
			{
				if (this.m_messagePart == null)
				{
					this.m_messagePart = new IPCMessagePart();
				}
				this.m_messagePart.Build(binaryReader, binaryReader.BaseStream.Length - binaryReader.BaseStream.Position);
				if (!this.m_messagePart.IsValid)
				{
					break;
				}
				IPCMessage message;
				try
				{
					message = Singleton<IPCMessageSerializer>.Instance.Deserialize(this.m_messagePart.Data);
				}
				catch (System.Exception arg)
				{
					IPCAccessor.logger.Error("Cannot deserialize received message ! Exception : {0}" + arg);
					break;
				}
				finally
				{
					this.m_messagePart = null;
				}
				this.TaskPool.AddMessage(delegate
				{
					this.ProcessMessage(message);
				});
			}
		}
		protected override void ProcessAnswer(IIPCRequest request, IPCMessage answer)
		{
			request.TimeoutTimer.Stop();
			this.TaskPool.RemoveTimer(request.TimeoutTimer);
			request.ProcessMessage(answer);
		}
		protected override void ProcessRequest(IPCMessage request)
		{
			if (request is IPCErrorMessage)
			{
				this.HandleError(request as IPCErrorMessage);
			}
			if (request is DisconnectClientMessage)
			{
				this.HandleMessage(request as DisconnectClientMessage);
			}
			if (this.m_additionalsHandlers.ContainsKey(request.GetType()))
			{
				this.m_additionalsHandlers[request.GetType()](request);
			}
		}
		public void AddMessageHandler(System.Type messageType, IPCAccessor.IPCMessageHandler handler)
		{
			this.m_additionalsHandlers.Add(messageType, handler);
		}
		private void HandleMessage(DisconnectClientMessage message)
		{
			WorldClient[] array = ServerBase<WorldServer>.Instance.FindClients((WorldClient client) => client.Account != null && client.Account.Id == message.AccountId);
			if (array.Length > 1)
			{
				IPCAccessor.logger.Error("Several clients connected on the same account ({0}). Disconnect them all", message.AccountId);
			}
			bool flag = false;
			for (int i = 0; i < array.Length; i++)
			{
				WorldClient worldClient = array[i];
				if ((flag = (worldClient.Character != null)) && i == 0)
				{
					worldClient.Character.Saved += delegate(Character chr)
					{
						this.OnCharacterSaved(message);
					};
				}
				worldClient.Disconnect();
			}
			if (!flag)
			{
				base.ReplyRequest(new DisconnectedClientMessage(array.Any<WorldClient>()), message);
			}
		}
		private void OnCharacterSaved(DisconnectClientMessage request)
		{
			base.ReplyRequest(new DisconnectedClientMessage(true), request);
		}
		private void HandleError(IPCErrorMessage error)
		{
			IPCAccessor.logger.Error<System.Type, string, string>("Error received of type {0}. Message : {1} StackTrace : {2}", error.GetType(), error.Message, error.StackTrace);
		}
		private void Close()
		{
			if (this.Socket != null && this.Socket.Connected)
			{
				this.Socket.Shutdown(SocketShutdown.Both);
				this.Socket.Close();
				this.Socket = null;
			}
		}
	}
}
