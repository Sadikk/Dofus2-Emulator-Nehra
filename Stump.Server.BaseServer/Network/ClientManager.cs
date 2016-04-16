using NLog;
using Stump.Core.Attributes;
using Stump.Core.Pool.New;
using Stump.Core.Reflection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace Stump.Server.BaseServer.Network
{
	public class ClientManager : Singleton<ClientManager>
	{
		public delegate BaseClient CreateClientHandler(Socket clientSocket);
		private readonly Logger logger = LogManager.GetCurrentClassLogger();
		[Variable]
		public static int MaxConcurrentConnections = 2000;
		[Variable]
		public static int MaxPendingConnections = 100;
		[Variable]
		public static int? MaxIPConnexions = new int?(10);
		[Variable]
		public static int? MinMessageInterval = new int?(1);
		[Variable]
		public static int BufferSize = 8192;
		private ClientManager.CreateClientHandler m_createClientDelegate;
		private readonly Socket m_listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		private readonly List<BaseClient> m_clients = new List<BaseClient>();
		private readonly SocketAsyncEventArgs m_acceptArgs = new SocketAsyncEventArgs();
		private SemaphoreSlim m_semaphore;
		private readonly AutoResetEvent m_resumeEvent = new AutoResetEvent(false);
		public event Action<BaseClient> ClientConnected;
		public event Action<BaseClient> ClientDisconnected;
		public ReadOnlyCollection<BaseClient> Clients
		{
			get
			{
				return this.m_clients.AsReadOnly();
			}
		}
		public int Count
		{
			get
			{
				return this.m_clients.Count;
			}
		}
		public bool Paused
		{
			get;
			private set;
		}
		public bool IsInitialized
		{
			get;
			private set;
		}
		public bool Started
		{
			get;
			private set;
		}
		public string Host
		{
			get;
			private set;
		}
		public int Port
		{
			get;
			private set;
		}
		public bool IsFull
		{
			get;
			private set;
		}
		private void NotifyClientConnected(BaseClient client)
		{
			Action<BaseClient> clientConnected = this.ClientConnected;
			if (clientConnected != null)
			{
				clientConnected(client);
			}
		}
		private void NotifyClientDisconnected(BaseClient client)
		{
			Action<BaseClient> clientDisconnected = this.ClientDisconnected;
			if (clientDisconnected != null)
			{
				clientDisconnected(client);
			}
		}
		public void Initialize(ClientManager.CreateClientHandler createClientHandler)
		{
			if (this.IsInitialized)
			{
				throw new Exception("ClientManager already initialized");
			}
			if (!ObjectPoolMgr.ContainsType<SocketAsyncEventArgs>())
			{
				ObjectPoolMgr.RegisterType<SocketAsyncEventArgs>(() => new SocketAsyncEventArgs());
				ObjectPoolMgr.SetMinimumSize<SocketAsyncEventArgs>(100);
			}
			this.m_createClientDelegate = createClientHandler;
			this.m_semaphore = new SemaphoreSlim(ClientManager.MaxConcurrentConnections, ClientManager.MaxConcurrentConnections);
			this.m_acceptArgs.Completed += delegate(object sender, SocketAsyncEventArgs e)
			{
				this.ProcessAccept(e);
			};
			this.IsInitialized = true;
		}
		public void Start(string address, int port)
		{
			if (!this.IsInitialized)
			{
				throw new Exception("Attempt to start ClientManager before initializing it. Call Initialize()");
			}
			if (this.Started)
			{
				throw new Exception("ClientManager already started");
			}
			this.Host = address;
			this.Port = port;
			IPEndPoint localEP = new IPEndPoint(Dns.GetHostAddresses(this.Host).First((IPAddress ip) => ip.AddressFamily == AddressFamily.InterNetwork), this.Port);
			this.m_listenSocket.NoDelay = true;
			this.m_listenSocket.Bind(localEP);
			this.m_listenSocket.Listen(ClientManager.MaxPendingConnections);
			this.Started = true;
			this.StartAccept();
		}
		public void Pause()
		{
			this.Paused = true;
		}
		public void Resume()
		{
			this.Paused = false;
			this.m_resumeEvent.Set();
		}
		public void Close()
		{
			this.Paused = true;
			this.m_listenSocket.Close();
			this.m_listenSocket.Dispose();
		}
		private void StartAccept()
		{
			this.m_acceptArgs.AcceptSocket = null;
			if (this.m_semaphore.CurrentCount == 0)
			{
				this.logger.Warn("Connected clients limits reached ! ({0}) Waiting for a disconnection ...", this.Count);
				this.IsFull = true;
			}
			this.m_semaphore.Wait();
			if (this.IsFull)
			{
				this.IsFull = false;
				this.logger.Warn("A client get disconnected, connection allowed", this.m_semaphore.CurrentCount);
			}
			if (!this.m_listenSocket.AcceptAsync(this.m_acceptArgs))
			{
				this.ProcessAccept(this.m_acceptArgs);
			}
		}
		private void ProcessAccept(SocketAsyncEventArgs e)
		{
			SocketAsyncEventArgs socketAsyncEventArgs = null;
			try
			{
				if (this.Paused)
				{
					this.logger.Warn("Pause state. Connection pending ...", this.m_semaphore.CurrentCount);
					this.m_resumeEvent.WaitOne();
				}
				IPAddress address = ((IPEndPoint)e.AcceptSocket.RemoteEndPoint).Address;
				if (ClientManager.MaxIPConnexions.HasValue && this.CountClientWithSameIp(address) > ClientManager.MaxIPConnexions.Value)
				{
					this.logger.Error<string, int>("Client {0} try to connect more then {1} times", e.AcceptSocket.RemoteEndPoint.ToString(), ClientManager.MaxIPConnexions.Value);
					this.m_semaphore.Release();
					this.StartAccept();
				}
				else
				{
					socketAsyncEventArgs = this.PopSocketArg();
					BaseClient baseClient = this.m_createClientDelegate(e.AcceptSocket);
					socketAsyncEventArgs.UserToken = baseClient;
					lock (this.m_clients)
					{
						this.m_clients.Add(baseClient);
					}
					this.NotifyClientConnected(baseClient);
					baseClient.BeginReceive();
					this.StartAccept();
				}
			}
			catch (Exception argument)
			{
				this.logger.Error<EndPoint, Exception>("Cannot accept a connection from {0}. Exception : {1}", e.RemoteEndPoint, argument);
				this.m_semaphore.Release();
				if (socketAsyncEventArgs != null)
				{
					this.PushSocketArg(socketAsyncEventArgs);
				}
				if (e.AcceptSocket != null)
				{
					e.AcceptSocket.Disconnect(false);
				}
				this.StartAccept();
			}
		}
		public void OnClientDisconnected(BaseClient client)
		{
			bool flag2;
			lock (this.m_clients)
			{
				flag2 = this.m_clients.Remove(client);
			}
			if (flag2)
			{
				this.NotifyClientDisconnected(client);
				this.m_semaphore.Release();
			}
		}
		public SocketAsyncEventArgs PopSocketArg()
		{
			return ObjectPoolMgr.ObtainObject<SocketAsyncEventArgs>();
		}
		public void PushSocketArg(SocketAsyncEventArgs args)
		{
			ObjectPoolMgr.ReleaseObject<SocketAsyncEventArgs>(args);
		}
		public BaseClient[] FindAll(Predicate<BaseClient> predicate)
		{
			BaseClient[] result;
			lock (this.m_clients)
			{
				result = (
					from entry in this.m_clients
					where predicate(entry)
					select entry).ToArray<BaseClient>();
			}
			return result;
		}
		public T[] FindAll<T>(Predicate<T> predicate)
		{
			T[] result;
			lock (this.m_clients)
			{
				result = (
					from entry in this.m_clients.OfType<T>()
					where predicate(entry)
					select entry).ToArray<T>();
			}
			return result;
		}
		public T[] FindAll<T>()
		{
			T[] result;
			lock (this.m_clients)
			{
				result = this.m_clients.OfType<T>().ToArray<T>();
			}
			return result;
		}
		public int CountClientWithSameIp(IPAddress ipAddress)
		{
			int result;
			lock (this.m_clients)
			{
				result = this.m_clients.Count((BaseClient t) => t.Socket != null && t.Socket.Connected && ((IPEndPoint)t.Socket.RemoteEndPoint).Address.Equals(ipAddress));
			}
			return result;
		}
	}
}
