namespace Stump.Server.WorldServer.Database.Shortcuts
{
	public class SpellShortcutRelator
	{
		public static string FetchQuery = "SELECT * FROM characters_shortcuts_spells";
		public static string FetchByOwner = "SELECT * FROM characters_shortcuts_spells WHERE OwnerId={0}";
	}
}
