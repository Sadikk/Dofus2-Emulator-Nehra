namespace Stump.Server.WorldServer.Database.Items
{
	public class PlayerItemRelator
	{
		public static string FetchQuery = "SELECT * FROM characters_items";
		public static string FetchByOwner = "SELECT * FROM characters_items WHERE OwnerId={0}";
	}
}
