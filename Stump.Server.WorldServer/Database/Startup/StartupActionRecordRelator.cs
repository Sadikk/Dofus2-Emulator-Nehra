namespace Stump.Server.WorldServer.Database.Startup
{
	public class StartupActionRecordRelator
	{
		public static string FetchQuery = "SELECT * FROM startup_actions LEFT JOIN startup_actions_items_binds ON startup_actions_items_binds.StartupActionId = startup_actions.Id LEFT JOIN startup_actions_items ON startup_actions_items.Id = startup_actions_items_binds.ItemId";
	}
}
