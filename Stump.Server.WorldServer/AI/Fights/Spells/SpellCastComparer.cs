using System;
using System.Linq;
namespace Stump.Server.WorldServer.AI.Fights.Spells
{
	public class SpellCastComparer : System.Collections.Generic.IComparer<SpellCastInformations>
	{
		private readonly System.Collections.Generic.Dictionary<SpellCategory, Func<SpellCastInformations, SpellCastInformations, int>> m_comparers;
		private readonly SpellSelector m_spellSelector;
		public SpellCategory Category
		{
			get;
			set;
		}
		public SpellCastComparer(SpellSelector spellSelector, SpellCategory category)
		{
			this.Category = category;
			this.m_spellSelector = spellSelector;
			this.m_comparers = new System.Collections.Generic.Dictionary<SpellCategory, Func<SpellCastInformations, SpellCastInformations, int>>
			{

				{
					SpellCategory.Summoning,
					new Func<SpellCastInformations, SpellCastInformations, int>(this.CompareSummon)
				},

				{
					SpellCategory.Buff,
					new Func<SpellCastInformations, SpellCastInformations, int>(this.CompareBoost)
				},

				{
					SpellCategory.Damages,
					new Func<SpellCastInformations, SpellCastInformations, int>(this.CompareDamage)
				},

				{
					SpellCategory.Healing,
					new Func<SpellCastInformations, SpellCastInformations, int>(this.CompareHeal)
				},

				{
					SpellCategory.Curse,
					new Func<SpellCastInformations, SpellCastInformations, int>(this.CompareCurse)
				}
			};
		}
		public int Compare(SpellCastInformations cast1, SpellCastInformations cast2)
		{
			return this.m_comparers[this.Category](cast1, cast2);
		}
		public int CompareSummon(SpellCastInformations cast1, SpellCastInformations cast2)
		{
			return cast1.IsSummoningSpell.CompareTo(cast2.IsSummoningSpell);
		}
		public int CompareBoost(SpellCastInformations cast1, SpellCastInformations cast2)
		{
			int result;
			if (cast1.Impacts.Count == 0 || cast2.Impacts.Count == 0)
			{
				result = cast1.Impacts.Count.CompareTo(cast2.Impacts.Count);
			}
			else
			{
				double num = cast1.Impacts.Max((SpellTarget x) => x.Boost);
				double num2 = cast2.Impacts.Max((SpellTarget x) => x.Boost);
				int efficiency = this.GetEfficiency(cast1);
				int efficiency2 = this.GetEfficiency(cast2);
				result = (num * (double)efficiency).CompareTo(num2 * (double)efficiency2);
			}
			return result;
		}
		public int CompareDamage(SpellCastInformations cast1, SpellCastInformations cast2)
		{
			int result;
			if (cast1.Impacts.Count == 0 || cast2.Impacts.Count == 0)
			{
				result = cast1.Impacts.Count.CompareTo(cast2.Impacts.Count);
			}
			else
			{
				double num = cast1.Impacts.Max((SpellTarget x) => x.Damage);
				double num2 = cast2.Impacts.Max((SpellTarget x) => x.Damage);
				int efficiency = this.GetEfficiency(cast1);
				int efficiency2 = this.GetEfficiency(cast2);
				result = (num * (double)efficiency).CompareTo(num2 * (double)efficiency2);
			}
			return result;
		}
		public int CompareHeal(SpellCastInformations cast1, SpellCastInformations cast2)
		{
			int result;
			if (cast1.Impacts.Count == 0 || cast2.Impacts.Count == 0)
			{
				result = cast1.Impacts.Count.CompareTo(cast2.Impacts.Count);
			}
			else
			{
				double num = cast1.Impacts.Max((SpellTarget x) => x.Heal);
				double num2 = cast2.Impacts.Max((SpellTarget x) => x.Heal);
				int efficiency = this.GetEfficiency(cast1);
				int efficiency2 = this.GetEfficiency(cast2);
				result = (num * (double)efficiency).CompareTo(num2 * (double)efficiency2);
			}
			return result;
		}
		public int CompareCurse(SpellCastInformations cast1, SpellCastInformations cast2)
		{
			int result;
			if (cast1.Impacts.Count == 0 || cast2.Impacts.Count == 0)
			{
				result = cast1.Impacts.Count.CompareTo(cast2.Impacts.Count);
			}
			else
			{
				double num = cast1.Impacts.Max((SpellTarget x) => x.Curse);
				double num2 = cast2.Impacts.Max((SpellTarget x) => x.Curse);
				int efficiency = this.GetEfficiency(cast1);
				int efficiency2 = this.GetEfficiency(cast2);
				result = (num * (double)efficiency).CompareTo(num2 * (double)efficiency2);
			}
			return result;
		}
		public int GetEfficiency(SpellCastInformations cast)
		{
			return (int)System.Math.Floor((double)this.m_spellSelector.Fighter.AP / cast.Spell.CurrentSpellLevel.ApCost);
		}
	}
}
