using Stump.Core.IO;
using Stump.DofusProtocol.Classes;
using Stump.DofusProtocol.Tools.D2o;
using Stump.ORM;
using Stump.ORM.SubSonic.SQLGeneration.Schema;
using System.Linq;
namespace Stump.Server.WorldServer.Database.Items
{
	[D2OClass("LivingObjectSkinJntMood", "com.ankamagames.dofus.datacenter.livingObjects", true), TableName("items_livingobjects")]
	public class LivingObjectRecord : IAutoGeneratedRecord, IAssignedByD2O
	{
		private byte[] m_moodsBin;
		private System.Collections.Generic.List<System.Collections.Generic.List<int>> m_moods;
		private System.Collections.Generic.List<int> m_skins = new System.Collections.Generic.List<int>();
		private string m_skinsCSV;
		[PrimaryKey("Id", false)]
		public int Id
		{
			get;
			set;
		}
		[Ignore]
		public System.Collections.Generic.List<System.Collections.Generic.List<int>> Moods
		{
			get
			{
				return this.m_moods;
			}
			set
			{
				this.m_moods = value;
				this.m_moodsBin = ((value == null) ? null : value.ToBinary());
			}
		}
		public byte[] MoodsBin
		{
			get
			{
				return this.m_moodsBin;
			}
			set
			{
				this.m_moodsBin = value;
				this.m_moods = ((value == null) ? null : value.ToObject<System.Collections.Generic.List<System.Collections.Generic.List<int>>>());
			}
		}
		[Ignore]
		public System.Collections.Generic.List<int> Skins
		{
			get
			{
				return this.m_skins;
			}
			set
			{
				this.m_skins = value;
				this.m_skinsCSV = value.ToCSV(",");
			}
		}
		[NullString]
		public string SkinsCSV
		{
			get
			{
				return this.m_skinsCSV;
			}
			set
			{
				this.m_skinsCSV = value;
                this.m_skins = ((value == null) ? new System.Collections.Generic.List<int>() : value.FromCSV<int>(",").ToList<int>());
			}
		}
		public int ItemType
		{
			get;
			set;
		}
		public virtual void AssignFields(object obj)
		{
			LivingObjectSkinJntMood livingObjectSkinJntMood = (LivingObjectSkinJntMood)obj;
			this.Id = livingObjectSkinJntMood.skinId;
			this.Moods = livingObjectSkinJntMood.moods;
		}
	}
}
