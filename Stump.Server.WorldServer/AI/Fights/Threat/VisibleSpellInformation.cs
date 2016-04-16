using Stump.Server.WorldServer.Database.Spells;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Spells;

namespace Stump.Server.WorldServer.AI.Fights.Threat
{
	public class VisibleSpellInformation
	{
		public AIFighter Sighter
		{
			get;
			private set;
		}
		public FightActor Caster
		{
			get;
			private set;
		}
		public Spell Spell
		{
			get;
			private set;
		}
		public SpellLevelTemplate SpellLevel
		{
			get
			{
				return this.Spell.CurrentSpellLevel;
			}
		}
		public uint Cost
		{
			get
			{
				return this.Spell.CurrentSpellLevel.ApCost;
			}
		}
		public int InflictedDamage
		{
			get;
			private set;
		}
		public int HealedPoints
		{
			get;
			private set;
		}
		public bool CanHeal
		{
			get
			{
				return this.HealedPoints > 0;
			}
		}
		public bool CanInflictDamage
		{
			get
			{
				return this.InflictedDamage > 0;
			}
		}
		public void AddInflictedDamage(int value, FightActor target)
		{
			this.InflictedDamage += value;
		}
		public void AddHealedPoints(int value, FightActor target)
		{
			this.HealedPoints += value;
		}
		public float GetThreat()
		{
			int num = (int)System.Math.Ceiling((double)(this.Caster.AP + (int)this.Caster.UsedAP) / this.Cost);
			return (float)((this.InflictedDamage + this.HealedPoints) * num);
		}
	}
}
