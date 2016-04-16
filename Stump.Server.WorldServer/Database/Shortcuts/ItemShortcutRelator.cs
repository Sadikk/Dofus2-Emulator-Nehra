namespace Stump.Server.WorldServer.Database.Shortcuts
{
	public class ItemShortcutRelator
	{
		public static string FetchQuery = "SELECT * FROM characters_shortcuts_items";
		public static string FetchByOwner = "SELECT * FROM characters_shortcuts_items WHERE OwnerId={0}";
	}
}
