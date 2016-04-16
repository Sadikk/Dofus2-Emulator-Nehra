namespace Stump.Server.WorldServer.Database.Items
{
	public class PlayerMerchantItemRelator
	{
		public static string FetchQuery = "SELECT * FROM characters_items_selled";
		public static string FetchByOwner = "SELECT * FROM characters_items_selled WHERE OwnerId={0}";
	}
}
