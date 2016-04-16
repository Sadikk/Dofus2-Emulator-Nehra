using Stump.Core.Reflection;
using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Database.Spells;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Spells
{
	public class Spell
	{
		private readonly ISpellRecord m_record;
		private readonly int m_id;
		private byte m_level;
		private SpellLevelTemplate m_currentLevel;
		public int Id
		{
			get
			{
				return this.m_id;
			}
		}
		public SpellTemplate Template
		{
			get;
			private set;
		}
		public SpellType SpellType
		{
			get;
			private set;
		}
		public byte CurrentLevel
		{
			get
			{
				return this.m_level;
			}
			set
			{
				this.m_record.Level = (short)value;
				this.m_level = value;
				this.m_currentLevel = ((!this.ByLevel.ContainsKey((int)this.CurrentLevel)) ? this.ByLevel[1] : this.ByLevel[(int)this.CurrentLevel]);
			}
		}
		public SpellLevelTemplate CurrentSpellLevel
		{
			get
			{
				SpellLevelTemplate arg_45_0;
				if ((arg_45_0 = this.m_currentLevel) == null)
				{
					arg_45_0 = (this.m_currentLevel = ((!this.ByLevel.ContainsKey((int)this.CurrentLevel)) ? this.ByLevel[1] : this.ByLevel[(int)this.CurrentLevel]));
				}
				return arg_45_0;
			}
		}
		public byte Position
		{
			get
			{
				return 63;
			}
		}
		public System.Collections.Generic.Dictionary<int, SpellLevelTemplate> ByLevel
		{
			get;
			private set;
		}
		public Spell(ISpellRecord record)
		{
			this.m_record = record;
			this.m_id = this.m_record.SpellId;
			this.m_level = (byte)this.m_record.Level;
			this.Template = Singleton<SpellManager>.Instance.GetSpellTemplate(this.Id);
			this.SpellType = Singleton<SpellManager>.Instance.GetSpellType(this.Template.TypeId);
			int counter = 1;
			this.ByLevel = Singleton<SpellManager>.Instance.GetSpellLevels(this.Template).ToDictionary((SpellLevelTemplate entry) => counter++);
		}
		public Spell(int id, byte level)
		{
			this.m_id = id;
			this.m_level = level;
			this.Template = Singleton<SpellManager>.Instance.GetSpellTemplate(this.Id);
			this.SpellType = Singleton<SpellManager>.Instance.GetSpellType(this.Template.TypeId);
			int counter = 1;
			this.ByLevel = Singleton<SpellManager>.Instance.GetSpellLevels(this.Template).ToDictionary((SpellLevelTemplate entry) => counter++);
		}
		public Spell(SpellTemplate template, byte level)
		{
			this.m_id = template.Id;
			this.m_level = level;
			this.Template = template;
			this.SpellType = Singleton<SpellManager>.Instance.GetSpellType(this.Template.TypeId);
			int counter = 1;
			this.ByLevel = Singleton<SpellManager>.Instance.GetSpellLevels(this.Template).ToDictionary((SpellLevelTemplate entry) => counter++);
		}
		public bool CanBoostSpell()
		{
			return this.ByLevel.ContainsKey((int)(this.CurrentLevel + 1));
		}
		public bool BoostSpell()
		{
			bool result;
			if (!this.CanBoostSpell())
			{
				result = false;
			}
			else
			{
				this.m_level += 1;
				this.m_record.Level = (short)this.m_level;
				this.m_currentLevel = this.ByLevel[(int)this.m_level];
				result = true;
			}
			return result;
		}
		public bool UnBoostSpell()
		{
			bool result;
			if (!this.ByLevel.ContainsKey((int)(this.CurrentLevel - 1)))
			{
				result = false;
			}
			else
			{
				this.m_level -= 1;
				this.m_record.Level = (short)this.m_level;
				this.m_currentLevel = this.ByLevel[(int)this.m_level];
				result = true;
			}
			return result;
		}
		public SpellItem GetSpellItem()
		{
			return new SpellItem(this.Position, this.Id, (sbyte)this.CurrentLevel);
		}
	}
}
