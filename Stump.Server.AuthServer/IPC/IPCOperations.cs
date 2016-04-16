using NLog;
using Stump.DofusProtocol.Enums;
using Stump.Server.AuthServer.Database;
using Stump.Server.AuthServer.IPC;
using Stump.Server.AuthServer.Managers;
using Stump.Server.BaseServer.IPC;
using Stump.Server.BaseServer.IPC.Messages;
using Stump.Server.BaseServer.IPC.Objects;
using Stump.Server.BaseServer.Network;
using System;
using System.Collections.Generic;
using System.Reflection;
using Stump.Core.Reflection;
using System.IO;

namespace Stump.Server.AuthServer.IPC
{
    public class IPCOperations
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private Dictionary<Type, Action<object, IPCMessage>> m_handlers = new Dictionary<Type, Action<object, IPCMessage>>();

        public IPCClient Client { get; private set; }

        public WorldServer WorldServer
        {
            get { return Client.Server; }
        }

        private Stump.ORM.Database Database
        {
            get;
            set;
        }

        private AccountManager AccountManager
        {
            get;
            set;
        }

        public IPCOperations(IPCClient ipcClient)
        {
            this.Client = ipcClient;
            this.InitializeHandlers();
            this.InitializeDatabase();
        }

        private void InitializeHandlers()
        {
            MethodInfo[] methods = base.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            for (int i = 0; i < methods.Length; i++)
            {
                MethodInfo methodInfo = methods[i];
                if (!(methodInfo.Name == "HandleMessage"))
                {
                    ParameterInfo[] parameters = methodInfo.GetParameters();
                    if (parameters.Length == 1 && parameters[0].ParameterType.IsSubclassOf(typeof(IPCMessage)))
                    {
                        m_handlers.Add(parameters[0].ParameterType, (Action<object, IPCMessage>)methodInfo.CreateDelegate(new[] { typeof(IPCMessage) }));
                    }
                }
            }
        }

        private void InitializeDatabase()
        {
            IPCOperations.logger.Info("Opening Database connection for '{0}' server", this.WorldServer.Name);
            this.Database = new Stump.ORM.Database(AuthServer.DatabaseConfiguration.GetConnectionString(), AuthServer.DatabaseConfiguration.ProviderName)
            {
                KeepConnectionAlive = true
            };
            this.Database.OpenSharedConnection();
            this.AccountManager = new AccountManager();
            this.AccountManager.ChangeDataSource(this.Database);
            this.AccountManager.Initialize();
        }

        public void HandleMessage(IPCMessage message)
        {
            Action<object, IPCMessage> action;
            if (!m_handlers.TryGetValue(message.GetType(), out action))
            {
                IPCOperations.logger.Error<Type>("Received message {0} but no method handle it !", message.GetType());
            }
            else
            {
                action(this, message);
            }
        }

        private void Handle(GroupsRequestMessage message)
        {
            Client.ReplyRequest(new GroupsListMessage(UserGroupManager.Instance.Users), message);
        }

        private void Handle(AccountRequestMessage message)
        {
            if (!string.IsNullOrEmpty(message.Ticket))
            {
                Account account = AccountManager.Instance.FindCachedAccountByTicket(message.Ticket);
                if (account == null)
                {
                    this.Client.SendError(string.Format("Account not found with ticket {0}", message.Ticket));
                }
                else
                {
                    this.Client.ReplyRequest(new AccountAnswerMessage(account.Serialize()), message);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(message.Nickname))
                {
                    Account account = this.AccountManager.FindAccountByNickname(message.Nickname);
                    if (account == null)
                    {
                        this.Client.SendError(string.Format("Account not found with nickname {0}", message.Nickname));
                    }
                    else
                    {
                        this.Client.ReplyRequest(new AccountAnswerMessage(account.Serialize()), message);
                    }
                }
                else
                {
                    this.Client.SendError("Ticket and Nickname null or empty");
                }
            }
        }

        private void Handle(ChangeStateMessage message)
        {
            WorldServerManager.Instance.ChangeWorldState(WorldServer, message.State);
            Client.ReplyRequest(new CommonOKMessage(), message);
        }

        private void Handle(ServerUpdateMessage message)
        {
            if (WorldServer.CharsCount == message.CharsCount)
            {
                Client.ReplyRequest(new CommonOKMessage(), message);
            }
            else
            {
                WorldServer.CharsCount = message.CharsCount;
                //dont give a fuck
                /*if (WorldServer.CharsCount >= WorldServer.CharsCount && WorldServer.Status == ServerStatusEnum.ONLINE)
                {
                    WorldServerManager.Instance.ChangeWorldState(WorldServer, ServerStatusEnum.FULL);
                }
                if (WorldServer.CharsCount < WorldServer.CharsCount && WorldServer.Status == ServerStatusEnum.FULL)
                {
                    WorldServerManager.Instance.ChangeWorldState(WorldServer, ServerStatusEnum.ONLINE);
                }*/
                Client.ReplyRequest(new CommonOKMessage(), message);
            }
        }

        private void Handle(CreateAccountMessage message)
        {
            AccountData account = message.Account;
            Account account2 = new Account
            {
                Id = account.Id,
                Login = account.Login,
                PasswordHash = account.PasswordHash,
                Nickname = account.Nickname,
                AvailableBreeds = account.AvailableBreeds,
                Ticket = account.Ticket,
                SecretQuestion = account.SecretQuestion,
                SecretAnswer = account.SecretAnswer,
                Lang = account.Lang,
                Email = account.Email
            };
            if (AccountManager.CreateAccount(account2))
            {
                Client.ReplyRequest(new CommonOKMessage(), message);
            }
            else
            {
                Client.SendError(string.Format("Login {0} already exists", account.Login));
            }
        }

        private void Handle(UpdateAccountMessage message)
        {
            Account account = AccountManager.FindAccountById(message.Account.Id);
            if (account == null)
            {
                Client.SendError(string.Format("Account {0} not found", message.Account.Id));
            }
            else
            {
                account.PasswordHash = message.Account.PasswordHash;
                account.SecretQuestion = message.Account.SecretQuestion;
                account.SecretAnswer = message.Account.SecretAnswer;
                account.Tokens = (int)message.Account.Tokens;
                Database.Update(account);
                Client.ReplyRequest(new CommonOKMessage(), message);
            }
        }

        private void Handle(DeleteAccountMessage message)
        {
            Account account;
            if (message.AccountId.HasValue)
            {
                account = AccountManager.FindAccountById(message.AccountId.Value);
            }
            else
            {
                if (string.IsNullOrEmpty(message.AccountName))
                {
                    Client.SendError("AccoundId and AccountName are null or empty");
                    return;
                }
                account = AccountManager.FindAccountByLogin(message.AccountName);
            }
            if (account == null)
            {
                Client.SendError(string.Format("Account {0}{1} not found", message.AccountId, message.AccountName));
            }
            else
            {
                AccountManager.DisconnectClientsUsingAccount(account);
                if (AccountManager.DeleteAccount(account))
                {
                    Client.ReplyRequest(new CommonOKMessage(), message);
                }
                else
                {
                    Client.SendError(string.Format("Cannot delete {0}", account.Login));
                }
            }
        }


        private void Handle(AddCharacterMessage message)
        {
            Account account = AccountManager.Instance.FindAccountById(message.AccountId);
            if (account == null)
            {
                Client.SendError(string.Format("Account {0} not found", message.AccountId));
            }
            else
            {
                if (AccountManager.AddAccountCharacter(account, WorldServer, message.CharacterId))
                {
                    Client.ReplyRequest(new CommonOKMessage(), message);
                }
                else
                {
                    Client.SendError(string.Format("Cannot add {0} character to {1} account", message.CharacterId, message.AccountId));
                }
            }
        }

        private void Handle(DeleteCharacterMessage message)
        {
            Account account = AccountManager.Instance.FindAccountById(message.AccountId);
            if (account == null)
            {
                Client.SendError(string.Format("Account {0} not found", message.AccountId));
            }
            else
            {
                if (AccountManager.DeleteAccountCharacter(account, WorldServer, message.CharacterId))
                {
                    Client.ReplyRequest(new CommonOKMessage(), message);
                }
                else
                {
                    Client.SendError(string.Format("Cannot delete {0} character from {1} account", message.CharacterId, message.AccountId));
                }
            }
        }

        private void Handle(BanAccountMessage message)
        {
            Account account;
            if (message.AccountId.HasValue)
            {
                account = AccountManager.FindAccountById(message.AccountId.Value);
            }
            else
            {
                if (string.IsNullOrEmpty(message.AccountName))
                {
                    Client.SendError("AccoundId and AccountName are null or empty");
                    return;
                }
                account = AccountManager.FindAccountByLogin(message.AccountName);
            }
            if (account == null)
            {
                Client.SendError(string.Format("Account {0}{1} not found", message.AccountId, message.AccountName));
            }
            else
            {
                account.IsBanned = true;
                account.BanReason = message.BanReason;
                account.BanEndDate = message.BanEndDate;
                account.BannerAccountId = message.BannerAccountId;
                Database.Update(account);
                Client.ReplyRequest(new CommonOKMessage(), message);
            }
        }

        private void Handle(UnBanAccountMessage message)
        {
            Account account;
            if (message.AccountId.HasValue)
            {
                account = AccountManager.FindAccountById(message.AccountId.Value);
            }
            else
            {
                if (string.IsNullOrEmpty(message.AccountName))
                {
                    Client.SendError("AccoundId and AccountName are null or empty");
                    return;
                }
                account = AccountManager.FindAccountByLogin(message.AccountName);
            }
            if (account == null)
            {
                Client.SendError(string.Format("Account {0}{1} not found", message.AccountId, message.AccountName));
            }
            else
            {
                account.IsBanned = false;
                account.BanEndDate = null;
                account.BanReason = null;
                account.BannerAccountId = null;
                Database.Update(account);
                Client.ReplyRequest(new CommonOKMessage(), message);
            }
        }

        private void Handle(BanIPMessage message)
        {
            IpBan ipBan = AccountManager.FindIpBan(message.IPRange);
            IPAddressRange iP = IPAddressRange.Parse(message.IPRange);
            if (ipBan != null)
            {
                ipBan.BanReason = message.BanReason;
                ipBan.BannedBy = message.BannerAccountId;
                ipBan.Duration = (message.BanEndDate.HasValue ? (message.BanEndDate - DateTime.Now) : null);
                ipBan.Date = DateTime.Now;
                Database.Update(ipBan);
            }
            else
            {
                IpBan poco = new IpBan
                {
                    IP = iP,
                    BanReason = message.BanReason,
                    BannedBy = message.BannerAccountId,
                    Duration = message.BanEndDate.HasValue ? (message.BanEndDate - DateTime.Now) : null,
                    Date = DateTime.Now
                };
                Database.Insert(poco);
            }
            Client.ReplyRequest(new CommonOKMessage(), message);
        }

        private void Handle(UnBanIPMessage message)
        {
            IpBan ipBan = AccountManager.FindIpBan(message.IPRange);
            if (ipBan == null)
            {
                Client.SendError(string.Format("IP ban {0} not found", message.IPRange));
            }
            else
            {
                Database.Delete(ipBan);
                Client.ReplyRequest(new CommonOKMessage(), message);
            }
        }

        public void Dispose()
        {
            if (this.Database != null)
            {
                this.Database.CloseSharedConnection();
            }
            this.m_handlers.Clear();
        }
    }
}