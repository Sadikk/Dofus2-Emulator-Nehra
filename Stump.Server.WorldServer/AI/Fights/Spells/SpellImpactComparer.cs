using System;

namespace Stump.Server.WorldServer.AI.Fights.Spells
{
	public class SpellImpactComparer : System.Collections.Generic.IComparer<SpellTarget>
	{
		private static readonly System.Collections.Generic.Dictionary<SpellCategory, Func<SpellTarget, SpellTarget, int>> m_comparers = new System.Collections.Generic.Dictionary<SpellCategory, Func<SpellTarget, SpellTarget, int>>
		{

			{
				SpellCategory.Buff,
				new Func<SpellTarget, SpellTarget, int>(SpellImpactComparer.CompareBoost)
			},

			{
				SpellCategory.Damages,
				new Func<SpellTarget, SpellTarget, int>(SpellImpactComparer.CompareDamage)
			},

			{
				SpellCategory.Healing,
				new Func<SpellTarget, SpellTarget, int>(SpellImpactComparer.CompareHeal)
			},

			{
				SpellCategory.Curse,
				new Func<SpellTarget, SpellTarget, int>(SpellImpactComparer.CompareCurse)
			}
		};
		private readonly SpellSelector m_spellSelector;
		public SpellCategory Category
		{
			get;
			set;
		}
		public SpellImpactComparer(SpellSelector spellSelector, SpellCategory category)
		{
			this.Category = category;
			this.m_spellSelector = spellSelector;
		}
		public int Compare(SpellTarget cast1, SpellTarget cast2)
		{
			int result;
			if (!SpellImpactComparer.m_comparers.ContainsKey(this.Category))
			{
				result = 0;
			}
			else
			{
				result = SpellImpactComparer.m_comparers[this.Category](cast1, cast2);
			}
			return result;
		}
		public static int CompareBoost(SpellTarget impact1, SpellTarget impact2)
		{
			return impact1.Boost.CompareTo(impact2.Boost);
		}
		public static int CompareDamage(SpellTarget impact1, SpellTarget impact2)
		{
			return impact1.Damage.CompareTo(impact2.Damage);
		}
		public static int CompareHeal(SpellTarget impact1, SpellTarget impact2)
		{
			return impact1.Heal.CompareTo(impact2.Heal);
		}
		public static int CompareCurse(SpellTarget impact1, SpellTarget impact2)
		{
			return impact1.Curse.CompareTo(impact2.Curse);
		}
	}
}
