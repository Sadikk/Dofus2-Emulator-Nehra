namespace Stump.Server.WorldServer.Database.Items
{
	public class TaxCollectorItemRelator
	{
		public static string FetchQuery = "SELECT * FROM taxcollectors_items";
		public static string FetchByOwner = "SELECT * FROM taxcollectors_items WHERE OwnerId={0}";
	}
}
