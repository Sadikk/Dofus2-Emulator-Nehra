namespace Stump.Server.WorldServer.Database.Accounts
{
	public class WorldAccountRelator
	{
		public static string FetchQuery = "SELECT * FROM accounts";
		public static string FetchById = "SELECT * FROM accounts WHERE Id={0}";
		public static string FetchByNickname = "SELECT * FROM accounts WHERE Nickname=@0";
	}
}
