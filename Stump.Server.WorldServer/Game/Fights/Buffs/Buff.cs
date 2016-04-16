using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Spells;

namespace Stump.Server.WorldServer.Game.Fights.Buffs
{
	public abstract class Buff
	{
		public const int CHARACTERISTIC_STATE = 71;

		public int Id
		{
			get;
			private set;
		}
		public FightActor Target
		{
			get;
			private set;
		}
		public FightActor Caster
		{
			get;
			private set;
		}
		public EffectBase Effect
		{
			get;
			private set;
		}
		public Spell Spell
		{
			get;
			private set;
		}
		public short Duration
		{
			get;
			set;
		}
		public bool Critical
		{
			get;
			private set;
		}
		public bool Dispellable
		{
			get;
			private set;
		}
		public short? CustomActionId
		{
			get;
			private set;
		}
		public double Efficiency
		{
			get;
			set;
		}
		public virtual BuffType Type
		{
			get
			{
				BuffType result;
				if (this.Effect.Template.Characteristic == 71)
				{
					result = BuffType.CATEGORY_STATE;
				}
				else
				{
					if (this.Effect.Template.Operator == "-")
					{
						result = (this.Effect.Template.Active ? BuffType.CATEGORY_ACTIVE_MALUS : BuffType.CATEGORY_PASSIVE_MALUS);
					}
					else
					{
						if (this.Effect.Template.Operator == "+")
						{
							result = (this.Effect.Template.Active ? BuffType.CATEGORY_ACTIVE_BONUS : BuffType.CATEGORY_PASSIVE_BONUS);
						}
						else
						{
							result = BuffType.CATEGORY_OTHER;
						}
					}
				}
				return result;
			}
		}

		protected Buff(int id, FightActor target, FightActor caster, EffectBase effect, Spell spell, bool critical, bool dispelable)
		{
			this.Id = id;
			this.Target = target;
			this.Caster = caster;
			this.Effect = effect;
			this.Spell = spell;
			this.Critical = critical;
			this.Dispellable = dispelable;
			this.Duration = (short)this.Effect.Duration;
			this.Efficiency = 1.0;
		}
		protected Buff(int id, FightActor target, FightActor caster, EffectBase effect, Spell spell, bool critical, bool dispelable, short customActionId)
		{
			this.Id = id;
			this.Target = target;
			this.Caster = caster;
			this.Effect = effect;
			this.Spell = spell;
			this.Critical = critical;
			this.Dispellable = dispelable;
			this.CustomActionId = new short?(customActionId);
			this.Duration = (short)this.Effect.Duration;
			this.Efficiency = 1.0;
		}

		public virtual bool DecrementDuration()
		{
			return (this.Duration -= 1) <= 0;
		}
		public abstract void Apply();
		public abstract void Dispell();
		public virtual short GetActionId()
		{
			short result;
			if (this.CustomActionId.HasValue)
			{
				result = this.CustomActionId.Value;
			}
			else
			{
				result = (short)this.Effect.EffectId;
			}
			return result;
		}
		public EffectInteger GenerateEffect()
		{
			EffectInteger effectInteger = this.Effect.GenerateEffect(EffectGenerationContext.Spell, EffectGenerationType.Normal) as EffectInteger;
			if (effectInteger != null)
			{
				effectInteger.Value = (short)((double)effectInteger.Value * this.Efficiency);
			}
			return effectInteger;
		}
		public FightDispellableEffectExtendedInformations GetFightDispellableEffectExtendedInformations()
		{
			return new FightDispellableEffectExtendedInformations((ushort)this.GetActionId(), this.Caster.Id, this.GetAbstractFightDispellableEffect());
		}
		public abstract AbstractFightDispellableEffect GetAbstractFightDispellableEffect();
	}
}
