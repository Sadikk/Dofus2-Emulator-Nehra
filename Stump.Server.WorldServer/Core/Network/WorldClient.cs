using NLog;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Messages;
using Stump.Server.BaseServer;
using Stump.Server.BaseServer.IPC.Objects;
using Stump.Server.BaseServer.Network;
using Stump.Server.WorldServer.Database.Accounts;
using Stump.Server.WorldServer.Database.Characters;
using Stump.Server.WorldServer.Game.Accounts;
using Stump.Server.WorldServer.Game.Accounts.Startup;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Handlers.Approach;
using Stump.Server.WorldServer.Handlers.Basic;
using System.Net.Sockets;
namespace Stump.Server.WorldServer.Core.Network
{
    public sealed class WorldClient : BaseClient
    {
        // FIELDS
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        // PROPERTIES
        private WorldAccount m_worldAccount;
        public bool AutoConnect
        {
            get;
            set;
        }
        public AccountData Account
        {
            get;
            private set;
        }
        public System.DateTime InQueueUntil
        {
            get;
            set;
        }
        public bool QueueShowed
        {
            get;
            set;
        }
        public WorldAccount WorldAccount
        {
            get
            {
                return this.m_worldAccount;
            }
            internal set
            {
                this.m_worldAccount = value;
            }
        }
        public System.Collections.Generic.List<StartupAction> StartupActions
        {
            get;
            private set;
        }
        public System.Collections.Generic.List<CharacterRecord> Characters
        {
            get;
            internal set;
        }
        public Character Character
        {
            get;
            internal set;
        }
        public UserGroup UserGroup
        {
            get;
            private set;
        }

        // CONSTRUCTORS
        public WorldClient(Socket socket)
            : base(socket)
        {
            this.Send(new ProtocolRequired(VersionExtension.ProtocolRequired, VersionExtension.ActualProtocol));
            this.Send(new HelloGameMessage());
            base.CanReceive = true;
            this.StartupActions = new System.Collections.Generic.List<StartupAction>();
            lock (ApproachHandler.ConnectionQueue.SyncRoot)
            {
                ApproachHandler.ConnectionQueue.Add(this);
            }
            this.InQueueUntil = System.DateTime.Now;
        }

        // METHODS
        public void SetCurrentAccount(AccountData account)
        {
            if (this.Account != null)
            {
                throw new System.Exception("Account already set");
            }
            this.Account = account;
            this.Characters = Singleton<CharacterManager>.Instance.GetCharactersByAccount(this);
            this.UserGroup = Singleton<AccountManager>.Instance.GetGroupOrDefault(account.UserGroupId);
            if (this.UserGroup == AccountManager.DefaultUserGroup)
            {
                WorldClient.logger.Error("Group {0} not found. Use default group instead !", account.UserGroupId);
            }
        }
        protected override void OnMessageReceived(Message message)
        {
            Singleton<WorldPacketHandler>.Instance.Dispatch(this, message);
            base.OnMessageReceived(message);
        }
        public void DisconnectAfk()
        {
            BasicHandler.SendSystemMessageDisplayMessage(this, true, 1, new object[0]);
            base.Disconnect();
        }
        protected override void OnDisconnect()
        {
            if (this.Character != null)
            {
                this.Character.LogOut();
            }
            ServerBase<WorldServer>.Instance.IOTaskPool.AddMessage(delegate
            {
                if (this.WorldAccount != null)
                {
                    this.WorldAccount.ConnectedCharacter = null;
                    ServerBase<WorldServer>.Instance.DBAccessor.Database.Update(this.WorldAccount);
                }
            });
            base.OnDisconnect();
        }
        public override string ToString()
        {
            return base.ToString() + ((this.Account != null) ? (" (" + this.Account.Login + ")") : "");
        }
    }
}
