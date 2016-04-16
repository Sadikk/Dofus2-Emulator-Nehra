using NLog;
using Stump.ORM;
using System;
using System.Data;
using System.Data.Common;
namespace Stump.Server.BaseServer.Database
{
    public class DatabaseLogged : Stump.ORM.Database
	{
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();
		public DatabaseLogged(IDbConnection connection) : base(connection)
		{
		}
		public DatabaseLogged(string connectionString, string providerName) : base(connectionString, providerName)
		{
		}
		public DatabaseLogged(string connectionString, DbProviderFactory provider) : base(connectionString, provider)
		{
		}
		public DatabaseLogged(string connectionStringName) : base(connectionStringName)
		{
		}
		public override IDbConnection OnConnectionOpened(IDbConnection conn)
		{
			return base.OnConnectionOpened(conn);
		}
		public override void OnConnectionClosing(IDbConnection conn)
		{
			DatabaseLogged.logger.Warn("Database connection closed !");
			base.OnConnectionClosing(conn);
		}
		public override void OnExecutingCommand(IDbCommand cmd)
		{
			base.OnExecutingCommand(cmd);
		}
		public override void OnExecutedCommand(IDbCommand cmd)
		{
			base.OnExecutedCommand(cmd);
		}
		public override void OnException(Exception x)
		{
			DatabaseLogged.logger.Error<Exception>("DB error : {0}", x);
			base.OnException(x);
		}
	}
}
