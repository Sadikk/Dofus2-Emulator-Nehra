using Stump.ORM;
using Stump.ORM.SubSonic.SQLGeneration.Schema;
using Stump.Server.BaseServer.Database.Interfaces;

namespace Stump.Server.WorldServer.Database.I18n
{
	[TableName("langs_ui")]
	public class LangTextUi : IAutoGeneratedRecord, ILangTextUI
	{
		[PrimaryKey("Id", true)]
		public uint Id
		{
			get;
			set;
		}
		public string Name
		{
			get;
			set;
		}
		[NullString]
		public string French
		{
			get;
			set;
		}
		[NullString]
		public string English
		{
			get;
			set;
		}
		[NullString]
		public string German
		{
			get;
			set;
		}
		[NullString]
		public string Spanish
		{
			get;
			set;
		}
		[NullString]
		public string Italian
		{
			get;
			set;
		}
		[NullString]
		public string Japanish
		{
			get;
			set;
		}
		[NullString]
		public string Dutsh
		{
			get;
			set;
		}
		[NullString]
		public string Portugese
		{
			get;
			set;
		}
		[NullString]
		public string Russish
		{
			get;
			set;
		}
	}
}
