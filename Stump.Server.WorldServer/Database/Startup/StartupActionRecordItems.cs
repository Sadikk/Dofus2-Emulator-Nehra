using Stump.ORM.SubSonic.SQLGeneration.Schema;

namespace Stump.Server.WorldServer.Database.Startup
{
	[TableName("startup_actions_items_binds")]
	public class StartupActionRecordItems
	{
		[PrimaryKey("StartupActionId", true)]
		public int StartupActionId
		{
			get;
			set;
		}
		[PrimaryKey("ItemId", true)]
		public int ItemId
		{
			get;
			set;
		}
	}
}
