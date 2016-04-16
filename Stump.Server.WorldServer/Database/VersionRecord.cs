using Stump.ORM.SubSonic.SQLGeneration.Schema;
using Stump.Server.BaseServer.Database.Interfaces;

namespace Stump.Server.WorldServer.Database
{
	[TableName("version")]
	public class VersionRecord : IVersionRecord
	{
		public string DofusVersion
		{
			get;
			set;
		}
		[PrimaryKey("Revision", true)]
		public uint Revision
		{
			get;
			set;
		}
		public System.DateTime UpdateDate
		{
			get;
			set;
		}
	}
}
