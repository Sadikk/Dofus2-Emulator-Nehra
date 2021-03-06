using Stump.ORM;
using Stump.ORM.SubSonic.SQLGeneration.Schema;

namespace Stump.Server.WorldServer.Database.Accounts
{
	[TableName("accounts_relations")]
	public class AccountRelation : IAutoGeneratedRecord
	{
		[PrimaryKey("Id", true)]
		public int Id
		{
			get;
			set;
		}
		public AccountRelationType Type
		{
			get;
			set;
		}
		public int AccountId
		{
			get;
			set;
		}
		public int TargetId
		{
			get;
			set;
		}
	}
}
