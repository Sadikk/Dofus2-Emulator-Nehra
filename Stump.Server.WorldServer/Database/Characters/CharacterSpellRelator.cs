namespace Stump.Server.WorldServer.Database.Characters
{
	public class CharacterSpellRelator
	{
		public static string FetchQuery = "SELECT * FROM characters_spells";
		public static string FetchByOwner = "SELECT * FROM characters_spells WHERE OwnerId={0}";
	}
}
