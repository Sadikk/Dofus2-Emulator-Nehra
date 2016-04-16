namespace Stump.Server.WorldServer.Database.Accounts
{
	public class AccountRelationRelator
	{
		public static string FetchQuery = "SELECT * FROM accounts_relations";
		public static string FetchByAccount = "SELECT * FROM accounts_relations WHERE AccountId={0}";
	}
}
