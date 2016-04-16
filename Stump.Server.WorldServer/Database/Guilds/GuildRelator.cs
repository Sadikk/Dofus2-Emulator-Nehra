namespace Stump.Server.WorldServer.Database.Guilds
{
	public class GuildRelator
	{
		public static string FetchQuery = "SELECT * FROM guilds";
		public static string FetchById = "SELECT * FROM guilds WHERE Id={0}";
	}
}
