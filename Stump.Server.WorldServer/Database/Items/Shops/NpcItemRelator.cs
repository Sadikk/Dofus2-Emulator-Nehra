namespace Stump.Server.WorldServer.Database.Items.Shops
{
	public class NpcItemRelator
	{
		public static string FetchQuery = "SELECT * FROM npcs_items";
		public static string FetchByNpcShop = "SELECT * FROM npcs_items WHERE NpcShopId = {0}";
	}
}
