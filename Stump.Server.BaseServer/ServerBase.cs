using NLog;
using Stump.Core.Attributes;
using Stump.Core.IO;
using Stump.Core.Reflection;
using Stump.Core.Threading;
using Stump.Core.Xml.Config;
using Stump.ORM;
using Stump.Server.BaseServer.Commands;
using Stump.Server.BaseServer.Exceptions;
using Stump.Server.BaseServer.Initialization;
using Stump.Server.BaseServer.Network;
using Stump.Server.BaseServer.Plugins;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;
namespace Stump.Server.BaseServer
{
	public abstract class ServerBase
	{
		internal static ServerBase InstanceAsBase;
		[Variable]
		public static int IOTaskInterval = 50;
		[Variable]
		public static bool ScheduledAutomaticShutdown = true;
		[Variable]
		public static int AutomaticShutdownTimer = 360;
		[Variable]
		public static string CommandsInfoFilePath = "./commands.xml";
		protected Dictionary<string, Assembly> LoadedAssemblies;
		protected Logger logger;
		private bool m_ignoreReload;
		public string ConfigFilePath
		{
			get;
			protected set;
		}
		public string SchemaFilePath
		{
			get;
			protected set;
		}
		public XmlConfig Config
		{
			get;
			protected set;
		}
		public ConsoleBase ConsoleInterface
		{
			get;
			protected set;
		}
		public DatabaseAccessor DBAccessor
		{
			get;
			protected set;
		}
		public CommandManager CommandManager
		{
			get;
			protected set;
		}
		public ClientManager ClientManager
		{
			get;
			protected set;
		}
		public SelfRunningTaskPool IOTaskPool
		{
			get;
			protected set;
		}
		public InitializationManager InitializationManager
		{
			get;
			protected set;
		}
		public PluginManager PluginManager
		{
			get;
			protected set;
		}
		public bool Running
		{
			get;
			protected set;
		}
		public DateTime StartTime
		{
			get;
			protected set;
		}
		public TimeSpan UpTime
		{
			get
			{
				return DateTime.Now - this.StartTime;
			}
		}
		public bool Initializing
		{
			get;
			protected set;
		}
		public bool IsInitialized
		{
			get;
			protected set;
		}
		public bool IsShutdownScheduled
		{
			get;
			protected set;
		}
		public DateTime ScheduledShutdownDate
		{
			get;
			protected set;
		}
		public string ScheduledShutdownReason
		{
			get;
			protected set;
		}
		protected ServerBase(string configFile, string schemaFile)
		{
			this.ConfigFilePath = configFile;
			this.SchemaFilePath = schemaFile;
		}
		public virtual void Initialize()
		{
			ServerBase.InstanceAsBase = this;
			this.Initializing = true;
			NLogHelper.DefineLogProfile(true, true);
			NLogHelper.EnableLogging();
			this.logger = LogManager.GetCurrentClassLogger();
			if (!Debugger.IsAttached)
			{
				AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.OnUnhandledException);
				TaskScheduler.UnobservedTaskException += new EventHandler<UnobservedTaskExceptionEventArgs>(this.OnUnobservedTaskException);
				Contract.ContractFailed += new EventHandler<ContractFailedEventArgs>(this.OnContractFailed);
			}
			else
			{
				this.logger.Warn("Exceptions not handled cause Debugger is attatched");
			}
			ServerBase.PreLoadReferences(Assembly.GetCallingAssembly());
			this.LoadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToDictionary((Assembly entry) => entry.GetName().Name);
			AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(this.OnAssemblyLoad);
			if (Environment.GetCommandLineArgs().Contains("-config"))
			{
				this.UpdateConfigFiles();
			}
			ConsoleBase.DrawAsciiLogo();
			Console.WriteLine();
			ServerBase.InitializeGarbageCollector();
			this.logger.Info("Initializing Configuration...");
			this.Config = new XmlConfig(this.ConfigFilePath);
			this.Config.AddAssemblies(this.LoadedAssemblies.Values.ToArray<Assembly>());
			if (!File.Exists(this.ConfigFilePath))
			{
				this.Config.Create(false);
				this.logger.Info("Config file created");
			}
			else
			{
				this.Config.Load();
			}
			this.logger.Info("Initialize Task Pool");
			this.IOTaskPool = new SelfRunningTaskPool(ServerBase.IOTaskInterval, "IO Task Pool");
			this.CommandManager = Singleton<CommandManager>.Instance;
			this.CommandManager.RegisterAll(Assembly.GetExecutingAssembly());
			this.logger.Info("Initializing Network Interfaces...");
			this.ClientManager = Singleton<ClientManager>.Instance;
			this.ClientManager.Initialize(new ClientManager.CreateClientHandler(this.CreateClient));
			if (Settings.InactivityDisconnectionTime.HasValue)
			{
				this.IOTaskPool.CallPeriodically(Settings.InactivityDisconnectionTime.Value / 4 * 1000, new Action(this.DisconnectAfkClient));
			}
			this.ClientManager.ClientConnected += new Action<BaseClient>(this.OnClientConnected);
			this.ClientManager.ClientDisconnected += new Action<BaseClient>(this.OnClientDisconnected);
			this.logger.Info("Register Plugins...");
			this.InitializationManager = Singleton<InitializationManager>.Instance;
			this.InitializationManager.AddAssemblies(AppDomain.CurrentDomain.GetAssemblies());
			this.PluginManager = Singleton<PluginManager>.Instance;
			this.PluginManager.PluginAdded += new PluginManager.PluginContextHandler(this.OnPluginAdded);
			this.PluginManager.PluginRemoved += new PluginManager.PluginContextHandler(this.OnPluginRemoved);
			this.logger.Info("Loading Plugins...");
			Singleton<PluginManager>.Instance.LoadAllPlugins();
			this.CommandManager.LoadOrCreateCommandsInfo(ServerBase.CommandsInfoFilePath);
		}
		public virtual void UpdateConfigFiles()
		{
			this.logger.Info("Recreate server config file ...");
			if (File.Exists(this.ConfigFilePath))
			{
				this.logger.Info("Update {0} file", this.ConfigFilePath);
				this.Config = new XmlConfig(this.ConfigFilePath);
				this.Config.AddAssemblies(this.LoadedAssemblies.Values.ToArray<Assembly>());
				this.Config.Load();
				this.Config.Create(true);
			}
			else
			{
				this.logger.Info("Create {0} file", this.ConfigFilePath);
				this.Config = new XmlConfig(this.ConfigFilePath);
				this.Config.AddAssemblies(this.LoadedAssemblies.Values.ToArray<Assembly>());
				this.Config.Create(false);
			}
			this.logger.Info("Recreate plugins config files ...", this.ConfigFilePath);
			this.PluginManager = Singleton<PluginManager>.Instance;
			Singleton<PluginManager>.Instance.LoadAllPlugins();
			foreach (PluginBase current in (
				from entry in this.PluginManager.GetPlugins()
				select entry.Plugin).OfType<PluginBase>())
			{
				if (current.UseConfig && current.AllowConfigUpdate)
				{
					bool flag;
					if (!(flag = File.Exists(current.GetConfigPath())))
					{
						this.logger.Info<string, string>("Create '{0}' config file => '{1}'", current.Name, Path.GetFileName(current.GetConfigPath()));
					}
					current.LoadConfig();
					if (flag)
					{
						this.logger.Info<string, string>("Update '{0}' config file => '{1}'", current.Name, Path.GetFileName(current.GetConfigPath()));
						current.Config.Create(true);
					}
				}
			}
			this.logger.Info("All config files were correctly updated/created ! Shutdown ...");
			Thread.Sleep(TimeSpan.FromSeconds(2.0));
			Environment.Exit(0);
		}
		private static void PreLoadReferences(Assembly executingAssembly)
		{
			foreach (Assembly current in 
				from assemblyName in executingAssembly.GetReferencedAssemblies()
				where AppDomain.CurrentDomain.GetAssemblies().Count((Assembly entry) => entry.GetName().FullName == assemblyName.FullName) <= 0
				select Assembly.Load(assemblyName))
			{
				ServerBase.PreLoadReferences(current);
			}
		}
		protected virtual void OnPluginRemoved(PluginContext plugincontext)
		{
			this.logger.Info("Plugin Unloaded : {0}", plugincontext.Plugin.GetDefaultDescription());
		}
		protected virtual void OnPluginAdded(PluginContext plugincontext)
		{
			this.logger.Info("Plugin Loaded : {0}", plugincontext.Plugin.GetDefaultDescription());
		}
		private void OnClientConnected(BaseClient client)
		{
			this.logger.Info<BaseClient>("Client {0} connected", client);
		}
		private void OnClientDisconnected(BaseClient client)
		{
			this.logger.Info<BaseClient>("Client {0} disconnected", client);
		}
		private static void InitializeGarbageCollector()
		{
			GCSettings.LatencyMode = (GCSettings.IsServerGC ? GCLatencyMode.Batch : GCLatencyMode.Interactive);
		}
		private void OnAssemblyLoad(object sender, AssemblyLoadEventArgs e)
		{
			string name = e.LoadedAssembly.GetName().Name;
			if (!this.LoadedAssemblies.ContainsKey(name))
			{
				this.LoadedAssemblies.Add(name, e.LoadedAssembly);
			}
		}
		private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
		{
			this.HandleCrashException(e.Exception);
			e.SetObserved();
		}
		private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			this.HandleCrashException((Exception)e.ExceptionObject);
			if (e.IsTerminating)
			{
				this.Shutdown();
			}
		}
		private void OnContractFailed(object sender, ContractFailedEventArgs e)
		{
			this.logger.Fatal("Contract failed : {0}", e.Condition);
			if (e.OriginalException != null)
			{
				this.HandleCrashException(e.OriginalException);
			}
			else
			{
				this.logger.Fatal(string.Format(" Stack Trace:\r\n{0}", Environment.StackTrace));
			}
			e.SetHandled();
		}
		public void HandleCrashException(Exception e)
		{
			Singleton<ExceptionManager>.Instance.RegisterException(e);
			this.logger.Fatal(string.Format(" Crash Exception : {0}\r\n", e.Message) + string.Format(" Source: {0} -> {1}\r\n", e.Source, e.TargetSite) + string.Format(" Stack Trace:\r\n{0}", e.StackTrace));
			if (e.InnerException != null)
			{
				this.HandleCrashException(e.InnerException);
			}
		}
		public virtual void Start()
		{
			this.Running = true;
			this.Initializing = false;
			this.IOTaskPool.CallPeriodically((int)TimeSpan.FromSeconds(30.0).TotalMilliseconds, new Action(this.KeepSQLConnectionAlive));
			this.IOTaskPool.CallPeriodically((int)TimeSpan.FromSeconds(5.0).TotalMilliseconds, new Action(this.CheckScheduledShutdown));
		}
		public void IgnoreNextConfigReload()
		{
			this.m_ignoreReload = true;
		}
		protected virtual void KeepSQLConnectionAlive()
		{
			try
			{
				this.DBAccessor.Database.Execute("DO 1", new object[0]);
			}
			catch (Exception argument)
			{
				this.logger.Error<Exception>("Cannot ping SQL connection : {0}", argument);
				this.logger.Warn("Try to Re-open the connection");
				try
				{
					this.DBAccessor.CloseConnection();
					this.DBAccessor.OpenConnection();
				}
				catch (Exception argument2)
				{
					this.logger.Error<Exception>("Cannot reopen the SQL connection : {0}", argument2);
				}
			}
		}
		protected virtual void DisconnectAfkClient()
		{
			BaseClient[] array = this.ClientManager.FindAll(delegate(BaseClient client)
			{
				double totalSeconds = DateTime.Now.Subtract(client.LastActivity).TotalSeconds;
				int? inactivityDisconnectionTime = Settings.InactivityDisconnectionTime;
				return totalSeconds >= (double)inactivityDisconnectionTime.GetValueOrDefault() && inactivityDisconnectionTime.HasValue;
			});
			BaseClient[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				BaseClient baseClient = array2[i];
				baseClient.Disconnect();
			}
		}
		protected abstract BaseClient CreateClient(Socket s);
		protected virtual void OnShutdown()
		{
			this.IOTaskPool.Stop(false);
		}
		public virtual void ScheduleShutdown(TimeSpan timeBeforeShuttingDown, string reason)
		{
			this.ScheduledShutdownReason = reason;
			this.ScheduleShutdown(timeBeforeShuttingDown);
		}
		public virtual void ScheduleShutdown(TimeSpan timeBeforeShuttingDown)
		{
			this.IsShutdownScheduled = true;
			this.ScheduledShutdownDate = DateTime.Now + timeBeforeShuttingDown;
		}
		public virtual void CancelScheduledShutdown()
		{
			this.IsShutdownScheduled = false;
			this.ScheduledShutdownDate = DateTime.MaxValue;
			this.ScheduledShutdownReason = null;
		}
		protected virtual void CheckScheduledShutdown()
		{
			if ((ServerBase.ScheduledAutomaticShutdown && this.UpTime.TotalMinutes > (double)ServerBase.AutomaticShutdownTimer) || (this.IsShutdownScheduled && this.ScheduledShutdownDate <= DateTime.Now))
			{
				this.Shutdown();
			}
		}
		public void Shutdown()
		{
			bool flag = false;
			try
			{
				Monitor.Enter(this, ref flag);
				this.Running = false;
				this.OnShutdown();
				GC.Collect();
				GC.WaitForPendingFinalizers();
				Console.WriteLine("Application is now terminated. Wait " + Definitions.ExitWaitTime + " seconds to exit ... or press any key to cancel");
				if (ConditionWaiter.WaitFor(() => Console.KeyAvailable, Definitions.ExitWaitTime * 1000, 20))
				{
					Console.ReadKey(true);
					Thread.Sleep(500);
					Console.WriteLine("Press now a key to exit...");
					Console.ReadKey(true);
				}
				Environment.Exit(0);
			}
			finally
			{
				if (flag)
				{
					Monitor.Exit(this);
				}
			}
		}
	}
	public abstract class ServerBase<T> : ServerBase where T : class
	{
		public static T Instance;
		protected ServerBase(string configFile, string schemaFile) : base(configFile, schemaFile)
		{
		}
		public override void Initialize()
		{
			ServerBase<T>.Instance = (this as T);
			base.Initialize();
		}
	}
}
