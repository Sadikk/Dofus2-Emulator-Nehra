using NLog;
using Stump.Core.Attributes;
using Stump.Core.Reflection;
using Stump.Server.AuthServer.Database;
using Stump.Server.AuthServer.Handlers.Connection;
using Stump.Server.AuthServer.Network;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Stump.Server.BaseServer.Network;
using System.Text;
using System.Threading.Tasks;
using Stump.Core.Threading;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Messages;

namespace Stump.Server.AuthServer.Managers
{
    public class AuthQueueManager : Singleton<AuthQueueManager>
    {
        [Variable]
        public static int QueueDelay = 500;

        private ConcurrentQueue<AuthClient> m_clients = new ConcurrentQueue<AuthClient>();
        private Logger logger = LogManager.GetCurrentClassLogger();
        private bool m_running;

        public bool IsInQueue(AuthClient client)
        {
            return m_clients.Contains(client);
        }

        public void AddClient(AuthClient client)
        {
            m_clients.Enqueue(client);
            if (!m_running)
            {
                m_running = true;
                Authentificate();
                Task.Factory.StartNewDelayed(QueueDelay, () =>
                {
                    try
                    {
                        Authentificate();
                    }
                    catch (Exception ex)
                    {
                        logger.Warn("errror while Authentificate some clients\n{1}", client, ex.Message);
                    }
                });
            }
        }

        private void Authentificate()
        {
            AuthClient client;

            if (!m_running || !m_clients.TryDequeue(out client))
            {
                return;
            }

            while (client == null || (client != null && !client.Connected))
            {
                if (m_clients.Count == 0)
                {
                    break;
                }

                m_clients.TryDequeue(out client);
                if (client != null && !client.Connected)
                {
                    client = null;
                }
            }

            if (client == null || client.IdentificationMessage == null)
            {
                if (m_clients.Count == 0)
                {
                    m_running = false;
                }
                if (m_running)
                {
                    Task.Factory.StartNewDelayed(QueueDelay, Authentificate);
                }
            }

            Task.Factory.StartNew(RefreshQueue);

            try
            {
                Indentificate(client);
            }
            catch (Exception ex)
            {
                logger.Warn("errror while Indentificate {0} :\n{1}", client, ex.Message);
            }

            if (m_clients.Count == 0)
            {
                m_running = false;
            }
            if (m_running)
            {
                Task.Factory.StartNewDelayed(QueueDelay, Authentificate);
            }
        }

        public void Indentificate(AuthClient client)
        {
            ConnectionHandler.SendCredentialsAcknowledgementMessage(client);

            if (!client.IdentificationMessage.version.IsUpToDate())
            {
                ConnectionHandler.SendIdentificationFailedForBadVersionMessage(client, VersionExtension.ExpectedVersion);
                client.DisconnectLater(1000);
                return;
            }

            Account account;
            if (!CredentialManager.Instance.DecryptCredentials(out account, client, client.IdentificationMessage.credentials.Select(entry => (byte)entry)))
            {
                ConnectionHandler.SendIdentificationFailedMessage(client, IdentificationFailureReasonEnum.WRONG_CREDENTIALS);
                client.DisconnectLater(1000);
                return;
            }

            if(account.IsLifeBanned)
            {
                ConnectionHandler.SendIdentificationFailedMessage(client, IdentificationFailureReasonEnum.BANNED);
                client.DisconnectLater(1000);
                return;
            }

            var ipBan = AccountManager.Instance.FindIpBan(client.IP);
            if (ipBan != null)
            {
                ConnectionHandler.SendIdentificationFailedBannedMessage(client, ipBan.GetEndDate());
                client.DisconnectLater(1000);
                return;
            }

            AccountManager.Instance.DisconnectClientsUsingAccount(account);
            client.Account = account;

            if (client.Account.Nickname == string.Empty)
            {
                client.Send(new NicknameRegistrationMessage());
                return;
            }

            ConnectionHandler.SendIdentificationSuccessMessage(client, false);

            if (client.IdentificationMessage.autoconnect)
            {
                WorldServer worldServer = null;
                if (client.IdentificationMessage.serverId != 0)
                {
                    worldServer = WorldServerManager.Instance.GetServerById(client.IdentificationMessage.serverId);
                }
                else if (client.Account.LastConnectionWorld.HasValue)
                {
                    worldServer = WorldServerManager.Instance.GetServerById(client.Account.LastConnectionWorld.Value);
                }

                if (worldServer != null && WorldServerManager.Instance.CanAccessToWorld(client, worldServer))
                {
                    ConnectionHandler.SendSelectServerData(client, worldServer);
                }
                else
                {
                    client.LookingOfServers = true;
                    ConnectionHandler.SendServersListMessage(client);
                }
            }
            else
            {
                client.LookingOfServers = true;
                ConnectionHandler.SendServersListMessage(client);
            }
        }

        private void RefreshQueue()
        {
            foreach (var client in m_clients)
            {
                ConnectionHandler.SendLoginQueueStatusMessage(client, GetPosition(client), GetCount());
            }
        }

        private ushort GetPosition(AuthClient client)
        {
            return (ushort)m_clients.ToList().IndexOf(client);
        }

        private ushort GetCount()
        {
            return (ushort)m_clients.Count;
        }
    }
}
