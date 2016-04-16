using System;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Spells;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Steals
{
	[EffectHandler(EffectsEnum.Effect_StealStrength), EffectHandler(EffectsEnum.Effect_StealRange), EffectHandler(EffectsEnum.Effect_StealVitality), EffectHandler(EffectsEnum.Effect_StealAgility), EffectHandler(EffectsEnum.Effect_StealIntelligence), EffectHandler(EffectsEnum.Effect_StealChance), EffectHandler(EffectsEnum.Effect_StealWisdom)]
	public class StatsSteal : SpellEffectHandler
	{
		public StatsSteal(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
		{
		}
		public override bool Apply()
		{
			bool result;
			foreach (FightActor current in base.GetAffectedActors())
			{
				EffectInteger effectInteger = base.GenerateEffect();
				if (effectInteger == null)
				{
					result = false;
					return result;
				}
				EffectsEnum[] buffDisplayedEffect = StatsSteal.GetBuffDisplayedEffect(this.Effect.EffectId);
				base.AddStatBuff(current, Convert.ToInt16(-effectInteger.Value), StatsSteal.GetEffectCaracteristic(this.Effect.EffectId), true, (short)buffDisplayedEffect[1]);
				base.AddStatBuff(base.Caster, effectInteger.Value, StatsSteal.GetEffectCaracteristic(this.Effect.EffectId), true, (short)buffDisplayedEffect[0]);
			}
			result = true;
			return result;
		}
		private static PlayerFields GetEffectCaracteristic(EffectsEnum effect)
		{
			PlayerFields result;
			switch (effect)
			{
			case EffectsEnum.Effect_StealChance:
				result = PlayerFields.Chance;
				break;
			case EffectsEnum.Effect_StealVitality:
				result = PlayerFields.Vitality;
				break;
			case EffectsEnum.Effect_StealAgility:
				result = PlayerFields.Agility;
				break;
			case EffectsEnum.Effect_StealIntelligence:
				result = PlayerFields.Intelligence;
				break;
			case EffectsEnum.Effect_StealWisdom:
				result = PlayerFields.Wisdom;
				break;
			case EffectsEnum.Effect_StealStrength:
				result = PlayerFields.Strength;
				break;
			default:
				if (effect != EffectsEnum.Effect_StealRange)
				{
					throw new System.Exception("No associated caracteristic to effect '" + effect + "'");
				}
				result = PlayerFields.Range;
				break;
			}
			return result;
		}
		private static EffectsEnum[] GetBuffDisplayedEffect(EffectsEnum effect)
		{
			EffectsEnum[] result;
			switch (effect)
			{
			case EffectsEnum.Effect_StealChance:
				result = new EffectsEnum[]
				{
					EffectsEnum.Effect_AddChance,
					EffectsEnum.Effect_SubChance
				};
				break;
			case EffectsEnum.Effect_StealVitality:
				result = new EffectsEnum[]
				{
					EffectsEnum.Effect_AddVitality,
					EffectsEnum.Effect_SubVitality
				};
				break;
			case EffectsEnum.Effect_StealAgility:
				result = new EffectsEnum[]
				{
					EffectsEnum.Effect_AddAgility,
					EffectsEnum.Effect_SubAgility
				};
				break;
			case EffectsEnum.Effect_StealIntelligence:
				result = new EffectsEnum[]
				{
					EffectsEnum.Effect_AddIntelligence,
					EffectsEnum.Effect_SubIntelligence
				};
				break;
			case EffectsEnum.Effect_StealWisdom:
				result = new EffectsEnum[]
				{
					EffectsEnum.Effect_AddWisdom,
					EffectsEnum.Effect_SubWisdom
				};
				break;
			case EffectsEnum.Effect_StealStrength:
				result = new EffectsEnum[]
				{
					EffectsEnum.Effect_AddStrength,
					EffectsEnum.Effect_SubStrength
				};
				break;
			default:
				if (effect != EffectsEnum.Effect_StealRange)
				{
					throw new System.Exception("No associated caracteristic to effect '" + effect + "'");
				}
				result = new EffectsEnum[]
				{
					EffectsEnum.Effect_AddRange,
					EffectsEnum.Effect_SubRange
				};
				break;
			}
			return result;
		}
	}
}
