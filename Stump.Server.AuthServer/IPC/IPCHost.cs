using NLog;
using Stump.Core.Reflection;
using Stump.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Stump.Server.AuthServer.IPC
{
    public class IPCHost : Singleton<IPCHost>
    {
        [Variable]
        public static readonly int ServersMaxCount = 10;

        [Variable]
        public static int BufferSize = 8192;

        [Variable]
        public static string IPCAddress = "localhost";

        [Variable]
        public static int IPCPort = 9100;

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly Socket m_listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private SocketAsyncEventArgs m_acceptArgs = new SocketAsyncEventArgs();
        private SemaphoreSlim m_semaphore;
        private List<IPCClient> m_clients = new List<IPCClient>();
        private bool m_paused;

        public event Action<IPCHost, IPCClient> ClientConnected;

        public event Action<IPCHost, IPCClient> ClientDisconnected;

        public bool IsPaused
        {
            get { return m_paused; }
        }

        public bool Started { get; private set; }

        public Socket Socket
        {
            get { return m_listenSocket; }
        }

        private void NotifyClientConnected(IPCClient client)
        {
            Action<IPCHost, IPCClient> clientConnected = ClientConnected;
            if (clientConnected != null)
            {
                clientConnected(this, client);
            }
        }

        private void NotifyClientDisconnected(IPCClient client)
        {
            Action<IPCHost, IPCClient> clientDisconnected = ClientDisconnected;
            if (clientDisconnected != null)
            {
                clientDisconnected(this, client);
            }
        }

        public void Initialize()
        {
            m_semaphore = new SemaphoreSlim(ServersMaxCount, ServersMaxCount);
            m_acceptArgs.Completed += delegate(object sender, SocketAsyncEventArgs e)
            {
                ProcessAccept(e);
            };
        }

        public void Start()
        {
            if (IsPaused)
            {
                m_paused = false;
                StartAccept();
            }
            else
            {
                if (Started)
                {
                    throw new Exception("IPCHost already started");
                }
                IPEndPoint localEP = new IPEndPoint(Dns.GetHostAddresses(IPCAddress).First(ip => ip.AddressFamily == AddressFamily.InterNetwork), IPCPort);
                m_listenSocket.Bind(localEP);
                m_listenSocket.Listen(1);
                Started = true;
                StartAccept();
            }
        }

        public void Stop()
        {
            if (!Started)
            {
                throw new Exception("IPCHost not started yet");
            }
            m_paused = true;
        }

        private void StartAccept()
        {
            m_acceptArgs.AcceptSocket = null;

            if (m_semaphore.CurrentCount == 0)
            {
                logger.Warn("Connected servers limits reached ! ({0}) Waiting for a disconnection ...", m_clients.Count);
            }
            m_semaphore.Wait();
            if (!m_listenSocket.AcceptAsync(m_acceptArgs))
            {
                ProcessAccept(m_acceptArgs);
            }
        }

        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            try
            {
                if (IsPaused)
                {
                    logger.Warn("Pause state. Connection cancelled ...", m_semaphore.CurrentCount);
                    e.AcceptSocket.Disconnect(false);
                    m_semaphore.Release();
                }
                else
                {
                    SocketAsyncEventArgs socketAsyncEventArgs = new SocketAsyncEventArgs();
                    socketAsyncEventArgs.SetBuffer(new byte[BufferSize], 0, BufferSize);
                    socketAsyncEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnReceiveCompleted);
                    IPCClient IPCClient = new IPCClient(e.AcceptSocket);
                    socketAsyncEventArgs.UserToken = IPCClient;
                    m_clients.Add(IPCClient);
                    NotifyClientConnected(IPCClient);
                    if (!e.AcceptSocket.ReceiveAsync(socketAsyncEventArgs))
                    {
                        StartAccept();
                        ProcessReceive(socketAsyncEventArgs);
                    }
                    else
                    {
                        StartAccept();
                    }
                }
            }
            catch (Exception argument)
            {
                logger.Error("Cannot accept a connection from {0}. Exception : {1}", e.RemoteEndPoint, argument);
                if (e.AcceptSocket != null)
                {
                    e.AcceptSocket.Disconnect(false);
                }
                m_semaphore.Release();
                StartAccept();
            }
        }

        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred <= 0 || e.SocketError != SocketError.Success)
            {
                CloseClientSocket(e);
            }
            else
            {
                IPCClient IPCClient = e.UserToken as IPCClient;
                if (IPCClient == null)
                {
                    CloseClientSocket(e);
                }
                else
                {
                    IPCClient.ProcessReceive(e.Buffer, e.Offset, e.BytesTransferred);
                    if (IPCClient.Socket == null)
                    {
                        CloseClientSocket(e);
                    }
                    else
                    {
                        if (!IPCClient.Socket.ReceiveAsync(e))
                        {
                            ProcessReceive(e);
                        }
                    }
                }
            }
        }

        private void CloseClientSocket(SocketAsyncEventArgs e)
        {
            IPCClient IPCClient = e.UserToken as IPCClient;
            if (IPCClient != null)
            {
                try
                {
                    IPCClient.Disconnect();
                    NotifyClientDisconnected(IPCClient);
                }
                finally
                {
                    m_clients.Remove(IPCClient);
                    m_semaphore.Release();
                }
            }
        }

        private void OnReceiveCompleted(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                switch (e.LastOperation)
                {
                    case SocketAsyncOperation.Disconnect:
                        CloseClientSocket(e);
                        break;

                    case SocketAsyncOperation.Receive:
                        ProcessReceive(e);
                        break;
                }
            }
            catch (Exception arg)
            {
                logger.Error("Last chance exception on receiving ! : " + arg);
            }
        }
    }
}