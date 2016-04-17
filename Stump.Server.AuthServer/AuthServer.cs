using MySql.Data.MySqlClient;
using Stump.Core.Attributes;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Messages;
using Stump.DofusProtocol.Types;
using Stump.ORM;
using Stump.Server.AuthServer.Database;
using Stump.Server.AuthServer.IO;
using Stump.Server.AuthServer.IPC;
using Stump.Server.AuthServer.Managers;
using Stump.Server.AuthServer.Network;
using Stump.Server.BaseServer;
using Stump.Server.BaseServer.Database;
using Stump.Server.BaseServer.IPC;
using Stump.Server.BaseServer.Network;
using Stump.Server.BaseServer.Plugins;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace Stump.Server.AuthServer
{
	public class AuthServer : ServerBase<AuthServer>
	{
        [Variable]
        public static readonly bool HostAutoDefined = true;

        [Variable]
        public static readonly string CustomHost = "127.0.0.1";

        [Variable]
        public static readonly int Port = 443;

        [Variable]
        public static string IpcAddress = "localhost";

        [Variable]
        public static int IpcPort = 9100;

        private string m_host;

        [Variable(Priority = 10)]
        public static DatabaseConfiguration DatabaseConfiguration = new DatabaseConfiguration
        {
            ProviderName = "MySql.Data.MySqlClient",
            Host = "localhost",
            Port = 3306,
            DbName = "ubys_auth",
            User = "root",
            Password = ""
        };

        public AuthPacketHandler HandlerManager
        {
            get;
            private set;
        }

        public AuthServer()
            : base(Definitions.ConfigFilePath, Definitions.SchemaFilePath)
        {
        }

		public override void Initialize()
		{
			try
            {
                base.Initialize();
                base.ConsoleInterface = new AuthConsole();
                ConsoleBase.SetTitle("#Stump Authentification Server");
                this.logger.Info("Initializing Database...");
                base.DBAccessor = new DatabaseAccessor(AuthServer.DatabaseConfiguration);
                base.DBAccessor.RegisterMappingAssembly(Assembly.GetExecutingAssembly());
                base.DBAccessor.Initialize();
                this.logger.Info("Opening Database...");
                base.DBAccessor.OpenConnection();
                DataManagerAllocator.Assembly = System.Reflection.Assembly.GetExecutingAssembly();
                DataManager.DefaultDatabase = base.DBAccessor.Database;
                this.logger.Info("Register Messages...");
                MessageReceiver.Initialize();
                ProtocolTypeManager.Initialize();
                this.logger.Info("Register Packets Handlers...");
                this.HandlerManager = Singleton<AuthPacketHandler>.Instance;
                this.HandlerManager.RegisterAll(Assembly.GetExecutingAssembly());
                this.logger.Info("Register Commands...");
                base.CommandManager.RegisterAll(Assembly.GetExecutingAssembly());
                this.logger.Info("Start World Servers Manager");
                Singleton<WorldServerManager>.Instance.Initialize();
                this.logger.Info("Initialize IPC Server..");
                IPCHost.Instance.Initialize();
                base.InitializationManager.InitializeAll();
                base.IsInitialized = true;
			}
			catch (Exception ex)
			{
				base.HandleCrashException(ex);
				base.Shutdown();
			}
		}
		protected override void OnPluginAdded(PluginContext plugincontext)
		{
			base.OnPluginAdded(plugincontext);
		}
		public override void Start()
		{
            base.Start();
            this.logger.Info("Start Ipc Server");
            IPCHost.Instance.Start();
            this.logger.Info("Starting Console Handler Interface...");
            base.ConsoleInterface.Start();
            this.logger.Info("Start listening on port : " + AuthServer.Port + "...");
            this.m_host = (AuthServer.HostAutoDefined ? IPAddress.Loopback.ToString() : AuthServer.CustomHost);
            base.ClientManager.Start(this.m_host, AuthServer.Port);
            base.StartTime = DateTime.Now;
		}
		protected override void OnShutdown()
        {
            base.DBAccessor.CloseConnection();
		}

		protected override BaseClient CreateClient(Socket s)
		{
			return new AuthClient(s);
		}
		public IEnumerable<AuthClient> FindClients(Predicate<AuthClient> predicate)
		{
			return base.ClientManager.FindAll<AuthClient>(predicate);
		}
	}
}
