using Stump.Core.Threading;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Fights.Buffs;
using Stump.Server.WorldServer.Game.Fights.Triggers;
using Stump.Server.WorldServer.Game.Spells;

namespace Stump.Server.WorldServer.Game.Fights
{
	public class Damage
	{
		private int m_amount;
		public EffectSchoolEnum School
		{
			get;
			set;
		}
		public FightActor Source
		{
			get;
			set;
		}
		public MarkTrigger MarkTrigger
		{
			get;
			set;
		}
		public Buff Buff
		{
			get;
			set;
		}
		public int BaseMinDamages
		{
			get;
			set;
		}
		public int BaseMaxDamages
		{
			get;
			set;
		}
		public int Amount
		{
			get
			{
				return this.m_amount;
			}
			set
			{
				if (value < 0)
				{
					value = 0;
				}
				this.m_amount = value;
				this.Generated = true;
			}
		}
		public bool Generated
		{
			get;
			private set;
		}
		public Spell Spell
		{
			get;
			set;
		}
		public bool PvP
		{
			get
			{
				return this.Source is CharacterFighter;
			}
		}
		public bool IgnoreDamageReduction
		{
			get;
			set;
		}
		public bool IgnoreDamageBoost
		{
			get;
			set;
		}
		public EffectGenerationType EffectGenerationType
		{
			get;
			set;
		}
		public bool ReflectedDamages
		{
			get;
			set;
		}
		public Damage(int amount)
		{
			this.Amount = amount;
		}
		public Damage(EffectDice effect)
		{
			this.BaseMaxDamages = (int)System.Math.Max(effect.DiceFace, effect.DiceNum);
			this.BaseMinDamages = (int)System.Math.Min(effect.DiceFace, effect.DiceNum);
		}
		public Damage(EffectDice effect, EffectSchoolEnum school, FightActor source, Spell spell) : this(effect)
		{
			this.School = school;
			this.Source = source;
			this.Spell = spell;
		}
		public void GenerateDamages()
		{
			if (!this.Generated)
			{
				switch (this.EffectGenerationType)
				{
				case EffectGenerationType.MaxEffects:
					this.Amount = this.BaseMaxDamages;
					break;
				case EffectGenerationType.MinEffects:
					this.Amount = this.BaseMinDamages;
					break;
				default:
				{
					AsyncRandom asyncRandom = new AsyncRandom();
					this.Amount = asyncRandom.Next(this.BaseMinDamages, this.BaseMaxDamages + 1);
					break;
				}
				}
			}
		}
	}
}
