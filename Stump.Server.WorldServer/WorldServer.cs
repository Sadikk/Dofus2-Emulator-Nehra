using ServiceStack.Text;
using Stump.Core.Attributes;
using Stump.Core.Mathematics;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Messages;
using Stump.DofusProtocol.Types;
using Stump.ORM;
using Stump.Server.BaseServer;
using Stump.Server.BaseServer.Database;
using Stump.Server.BaseServer.Initialization;
using Stump.Server.BaseServer.IPC.Objects;
using Stump.Server.BaseServer.Network;
using Stump.Server.BaseServer.Plugins;
using Stump.Server.WorldServer.Core.IO;
using Stump.Server.WorldServer.Core.IPC;
using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Game;
using System;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;

namespace Stump.Server.WorldServer
{
	public class WorldServer : ServerBase<WorldServer>
	{
		[Variable]
		public static readonly string Host = "127.0.0.1";
		[Variable]
		public static readonly int Port = 3467;
		[Variable(true)]
		public static WorldServerData ServerInformation = new WorldServerData
		{
			Id = 1,
			Name = "Jiva",
			Address = "localhost",
			Port = 3467,
			Capacity = 2000,
			RequiredRole = RoleEnum.Player,
			RequireSubscription = false
		};
		[Variable(Priority = 10)]
        public static DatabaseConfiguration DatabaseConfiguration = new DatabaseConfiguration
        {
            Host = "localhost",
            Port = 3306,
            DbName = "ubys_world",
            User = "root",
            Password = "",
            ProviderName = "MySql.Data.MySqlClient"
        };
		[Variable(true)]
		public static int AutoSaveInterval = 180;
		[Variable(true)]
		public static bool SaveMessage = true;
		private System.TimeSpan? m_lastAnnouncedTime;
		public WorldVirtualConsole VirtualConsoleInterface
		{
			get;
			protected set;
		}
		public WorldPacketHandler HandlerManager
		{
			get;
			private set;
		}
		public WorldServer() : base(Definitions.ConfigFilePath, Definitions.SchemaFilePath)
		{
		}
		public override void Initialize()
		{
			base.Initialize();
			base.ConsoleInterface = new WorldConsole();
			this.VirtualConsoleInterface = new WorldVirtualConsole();
			ConsoleBase.SetTitle("#Stump World Server : " + WorldServer.ServerInformation.Name);
			this.logger.Info("Initializing Database...");
			base.DBAccessor = new DatabaseAccessor(WorldServer.DatabaseConfiguration);
			base.DBAccessor.RegisterMappingAssembly(System.Reflection.Assembly.GetExecutingAssembly());
			base.InitializationManager.Initialize(InitializationPass.Database);
			base.DBAccessor.Initialize();
			this.logger.Info("Opening Database...");
			base.DBAccessor.OpenConnection();
			DataManager.DefaultDatabase = base.DBAccessor.Database;
			DataManagerAllocator.Assembly = System.Reflection.Assembly.GetExecutingAssembly();
			this.logger.Info("Register Messages...");
			MessageReceiver.Initialize();
			ProtocolTypeManager.Initialize();
			this.logger.Info("Register Packet Handlers...");
			this.HandlerManager = Singleton<WorldPacketHandler>.Instance;
			this.HandlerManager.RegisterAll(System.Reflection.Assembly.GetExecutingAssembly());
			this.logger.Info("Register Commands...");
			base.CommandManager.RegisterAll(System.Reflection.Assembly.GetExecutingAssembly());
			base.InitializationManager.InitializeAll();
			base.IsInitialized = true;
		}
		protected override void OnPluginAdded(PluginContext plugincontext)
		{
			base.CommandManager.RegisterAll(plugincontext.PluginAssembly);
			base.OnPluginAdded(plugincontext);
		}
		public override void Start()
		{
			base.Start();
			this.logger.Info("Start Auto-Save Cyclic Task");
			base.IOTaskPool.CallPeriodically(WorldServer.AutoSaveInterval * 1000, new Action(Singleton<World>.Instance.Save));
			this.logger.Info("Starting Console Handler Interface...");
			base.ConsoleInterface.Start();
			this.logger.Info("Starting IPC Communications ...");
			IPCAccessor.Instance.Start();
			this.logger.Info("Start listening on port : " + WorldServer.Port + "...");
			base.ClientManager.Start(WorldServer.Host, WorldServer.Port);
			base.StartTime = System.DateTime.Now;
		}
		protected override BaseClient CreateClient(Socket s)
		{
			return new WorldClient(s);
		}
		protected override void DisconnectAfkClient()
		{
			WorldClient[] array = this.FindClients(delegate(WorldClient client)
			{
				double totalSeconds = System.DateTime.Now.Subtract(client.LastActivity).TotalSeconds;
				int? inactivityDisconnectionTime = Stump.Server.BaseServer.Settings.InactivityDisconnectionTime;
				return totalSeconds >= (double)inactivityDisconnectionTime.GetValueOrDefault() && inactivityDisconnectionTime.HasValue;
			});
			WorldClient[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				WorldClient worldClient = array2[i];
				worldClient.DisconnectAfk();
			}
		}
		public bool DisconnectClient(int accountId)
		{
			System.Collections.Generic.IEnumerable<WorldClient> enumerable = this.FindClients((WorldClient client) => client.Account != null && client.Account.Id == accountId);
			foreach (WorldClient current in enumerable)
			{
				current.Disconnect();
			}
			return enumerable.Any<WorldClient>();
		}
		public WorldClient[] FindClients(System.Predicate<WorldClient> predicate)
		{
			return base.ClientManager.FindAll<WorldClient>(predicate);
		}
		public override void ScheduleShutdown(System.TimeSpan timeBeforeShuttingDown)
		{
			base.ScheduleShutdown(timeBeforeShuttingDown);
			this.AnnounceTimeBeforeShutdown(timeBeforeShuttingDown, false);
		}
		public override void CancelScheduledShutdown()
		{
			base.CancelScheduledShutdown();
			Singleton<World>.Instance.SendAnnounce("Reboot canceled !", Color.Red);
		}
		protected override void CheckScheduledShutdown()
		{
			System.TimeSpan timeSpan = System.TimeSpan.FromMinutes((double)ServerBase.AutomaticShutdownTimer) - base.UpTime;
			bool automatic = true;
			if (base.IsShutdownScheduled && timeSpan > base.ScheduledShutdownDate - System.DateTime.Now)
			{
				timeSpan = base.ScheduledShutdownDate - System.DateTime.Now;
				automatic = false;
			}
			if (timeSpan < System.TimeSpan.FromMinutes(30.0))
			{
				System.TimeSpan? timeSpan2 = this.m_lastAnnouncedTime.HasValue ? new System.TimeSpan?(System.TimeSpan.MaxValue) : (this.m_lastAnnouncedTime - timeSpan);
				if (timeSpan > System.TimeSpan.FromMinutes(10.0) && timeSpan2 >= System.TimeSpan.FromMinutes(5.0))
				{
					this.AnnounceTimeBeforeShutdown(System.TimeSpan.FromMinutes(timeSpan.TotalMinutes.RoundToNearest(5.0)), automatic);
				}
				if (timeSpan > System.TimeSpan.FromMinutes(5.0) && timeSpan <= System.TimeSpan.FromMinutes(10.0) && timeSpan2 >= System.TimeSpan.FromMinutes(1.0))
				{
					this.AnnounceTimeBeforeShutdown(System.TimeSpan.FromMinutes(timeSpan.TotalMinutes), automatic);
				}
				if (timeSpan > System.TimeSpan.FromMinutes(1.0) && timeSpan <= System.TimeSpan.FromMinutes(5.0) && timeSpan2 >= System.TimeSpan.FromSeconds(30.0))
				{
					this.AnnounceTimeBeforeShutdown(new System.TimeSpan(0, 0, 0, (int)timeSpan.TotalSeconds.RoundToNearest(30.0)), automatic);
				}
				if (timeSpan > System.TimeSpan.FromSeconds(10.0) && timeSpan <= System.TimeSpan.FromMinutes(1.0) && timeSpan2 >= System.TimeSpan.FromSeconds(10.0))
				{
					this.AnnounceTimeBeforeShutdown(new System.TimeSpan(0, 0, 0, (int)timeSpan.TotalSeconds.RoundToNearest(10.0)), automatic);
				}
				if (timeSpan <= System.TimeSpan.FromSeconds(10.0) && timeSpan > System.TimeSpan.Zero)
				{
					this.AnnounceTimeBeforeShutdown(System.TimeSpan.FromSeconds(timeSpan.Seconds.RoundToNearest(5)), automatic);
				}
			}
			base.CheckScheduledShutdown();
		}
		private void AnnounceTimeBeforeShutdown(System.TimeSpan time, bool automatic)
		{
			string str = automatic ? "Automatic reboot in <b>{0:mm\\:ss}</b>" : "Reboot in <b>{0:mm\\:ss}</b>";
			if (!automatic && !string.IsNullOrEmpty(base.ScheduledShutdownReason))
			{
				str = str + " : " + base.ScheduledShutdownReason;
			}
			if (WorldServer.SaveMessage)
			{
				Singleton<World>.Instance.SendAnnounce(TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 15, new object[]
				{
					time
				});
			}
			this.m_lastAnnouncedTime = new System.TimeSpan?(time);
		}
		protected override void OnShutdown()
		{
			if (base.IsInitialized)
			{
				System.Threading.AutoResetEvent wait = new System.Threading.AutoResetEvent(false);
				base.IOTaskPool.ExecuteInContext(delegate
				{
					Singleton<World>.Instance.Stop(true);
					Singleton<World>.Instance.Save();
					wait.Set();
				});
				wait.WaitOne();
			}
			IPCAccessor.Instance.Stop();
			if (base.IOTaskPool != null)
			{
				base.IOTaskPool.Stop(false);
			}
			base.ClientManager.Pause();
			BaseClient[] array = base.ClientManager.Clients.ToArray<BaseClient>();
			for (int i = 0; i < array.Length; i++)
			{
				BaseClient baseClient = array[i];
				baseClient.Disconnect();
			}
			base.ClientManager.Close();
		}
	}
}
