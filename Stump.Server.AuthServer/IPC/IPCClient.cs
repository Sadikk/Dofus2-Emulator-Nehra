using NLog;
using Stump.Core.Timers;
using Stump.Server.AuthServer.Database;
using Stump.Server.AuthServer.Managers;
using Stump.Server.BaseServer.IPC;
using Stump.Server.BaseServer.IPC.Messages;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Stump.Server.AuthServer.IPC
{
    public class IPCClient : IPCEntity
    {
        public new delegate void IPCMessageHandler(IPCMessage message);

        public delegate void RequestCallbackDefaultDelegate(IPCMessage unattemptMessage);
        public delegate void RequestCallbackDelegate<in T>(T callbackMessage) where T : IPCMessage;
        public delegate void RequestCallbackErrorDelegate(IPCErrorMessage errorMessage);

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly Dictionary<System.Type, IPCMessageHandler> m_additionalsHandlers = new Dictionary<System.Type, IPCMessageHandler>();
        private readonly TimedTimerEntry m_updateTimer;
        private IPCMessagePart m_messagePart;
        private bool m_requestingAccess;
        private object m_recvLock = new object();
        private bool m_recvLockAcquired = false;
        private IPCMessage m_currentRequest;
        private IPCOperations m_operations;

        public WorldServer Server { get; private set; }

        public Socket Socket { get; private set; }

        public int Port
        {
            get { return ((IPEndPoint)Socket.RemoteEndPoint).Port; }
        }

        public IPAddress Address
        {
            get { return ((IPEndPoint)Socket.RemoteEndPoint).Address; }
        }

        public bool Connected
        {
            get { return Socket != null && Socket.Connected; }
        }
        public DateTime LastActivity { get; set; }

        public IPCClient(Socket socket)
        {
            Socket = socket;
        }

        public override void Send(IPCMessage message)
        {
            if (Connected)
            {
                SocketAsyncEventArgs socketAsyncEventArgs = new SocketAsyncEventArgs();
                byte[] array = IPCMessageSerializer.Instance.SerializeWithLength(message);
                socketAsyncEventArgs.SetBuffer(array, 0, array.Length);
                Socket.SendAsync(socketAsyncEventArgs);
            }
        }

        internal void ProcessReceive(byte[] data, int offset, int count)
        {
            try
            {
                LastActivity = DateTime.Now;
                System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(new System.IO.MemoryStream(data, offset, count));
                while (binaryReader.BaseStream.Length - binaryReader.BaseStream.Position > 0L)
                {
                    if (m_messagePart == null)
                    {
                        m_messagePart = new IPCMessagePart();
                    }
                    m_messagePart.Build(binaryReader, binaryReader.BaseStream.Length - binaryReader.BaseStream.Position);
                    if (!m_messagePart.IsValid)
                    {
                        break;
                    }

                    IPCMessage message;
                    try
                    {
                        message = IPCMessageSerializer.Instance.Deserialize(m_messagePart.Data);
                    }
                    catch (System.Exception arg)
                    {
                        logger.Error("Cannot deserialize received message ! Exception : {0}" + arg);
                        break;
                    }
                    finally
                    {
                        m_messagePart = null;
                    }

                    if (m_recvLockAcquired)
                    {
                        IPCClient.logger.Error("Recv lock should not be set 'cause it's mono thread !");
                    }
                    Monitor.Enter(m_recvLock, ref m_recvLockAcquired);
                    try
                    {
                        ProcessMessage(message);
                    }
                    finally
                    {
                        Monitor.Exit(m_recvLock);
                        m_recvLockAcquired = false;
                    }
                }
            }
            catch (Exception arg)
            {
                IPCClient.logger.Error("Forced disconnection during reception : " + arg);
                Disconnect();
            }
        }

        protected override void ProcessAnswer(IIPCRequest request, IPCMessage answer)
        {
            request.TimeoutTimer.Stop();
            AuthServer.Instance.IOTaskPool.AddTimer(request.TimeoutTimer);
            request.ProcessMessage(answer);
        }

        protected override void ProcessRequest(IPCMessage message)
        {
            if (m_operations == null)
            {
                if (!(message is HandshakeMessage))
                {
                    SendError(string.Format("The first received packet should be a HandshakeMessage not {0}", message.GetType()));
                    Disconnect();
                }
                else
                {
                    HandshakeMessage handshakeMessage = message as HandshakeMessage;
                    WorldServer server;
                    try
                    {
                        server = WorldServerManager.Instance.RequestConnection(this, handshakeMessage.World);
                    }
                    catch (Exception exception)
                    {
                        SendError(exception);
                        Disconnect();
                        return;
                    }
                    Server = server;
                    m_operations = new IPCOperations(this);
                    Send(new CommonOKMessage
                    {
                        RequestGuid = message.RequestGuid
                    });
                }
            }
            else
            {
                if (message.RequestGuid != Guid.Empty)
                {
                    m_currentRequest = message;
                }
                else
                {
                    m_currentRequest = null;
                }
                try
                {
                    m_operations.HandleMessage(message);
                }
                catch (Exception exception)
                {
                    SendError(exception);
                }
            }
        }

        public void SendError(Exception exception)
        {
            Send(new IPCErrorMessage(exception.Message, exception.StackTrace));
        }

        public void SendError(string error)
        {
            Send(new IPCErrorMessage(error));
        }

        public void Disconnect()
        {
            if (Server != null)
            {
                WorldServerManager.Instance.RemoveWorld(Server);
            }
            if (m_operations != null)
            {
                m_operations.Dispose();
            }
            Server = null;
            m_operations = null;
        }

        protected override int RequestTimeout
        {
            get
            {
                return 5;
            }
        }

        protected override TimerEntry RegisterTimer(Action<int> action, int timeout)
        {
            TimerEntry timerEntry = new TimerEntry
            {
                Action = action,
                InitialDelay = timeout
            };
            AuthServer.Instance.IOTaskPool.AddTimer(timerEntry);
            return timerEntry;
        }
    }
}