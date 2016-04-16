namespace Stump.Server.WorldServer.Database.Characters
{
	public class CharacterRelator
	{
		public static string FetchQuery = "SELECT * FROM characters";
		public static string FetchById = "SELECT * FROM characters WHERE Id = {0}";
		public static string FetchByName = "SELECT * FROM characters WHERE Name = @0";
		public static string FetchByMultipleId = "SELECT * FROM characters WHERE Id IN ({0})";
	}
}
