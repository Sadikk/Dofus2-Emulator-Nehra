using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Spells;

namespace Stump.Server.WorldServer.AI.Fights.Spells
{
	public class SpellCastInformations
	{
		public Spell Spell
		{
			get;
			private set;
		}
		public bool IsSummoningSpell
		{
			get;
			set;
		}
		public Cell SummonCell
		{
			get;
			set;
		}
		public int MPToUse
		{
			get;
			set;
		}
		public bool HasToMove
		{
			get
			{
				return this.MPToUse > 0;
			}
		}
		public System.Collections.Generic.List<SpellTarget> Impacts
		{
			get;
			private set;
		}
		public SpellCastInformations(Spell spell)
		{
			this.Spell = spell;
			this.Impacts = new System.Collections.Generic.List<SpellTarget>();
		}
	}
}
