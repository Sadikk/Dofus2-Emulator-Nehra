using System;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Spells;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Debuffs
{
	[EffectHandler(EffectsEnum.Effect_SubLock), EffectHandler(EffectsEnum.Effect_SubDodgeAPProbability), EffectHandler(EffectsEnum.Effect_SubDamageBonusPercent), EffectHandler(EffectsEnum.Effect_SubDodge), EffectHandler(EffectsEnum.Effect_SubStrength), EffectHandler(EffectsEnum.Effect_SubChance), EffectHandler(EffectsEnum.Effect_SubDamageBonus), EffectHandler(EffectsEnum.Effect_SubRange_135), EffectHandler(EffectsEnum.Effect_SubDodgeMPProbability), EffectHandler(EffectsEnum.Effect_SubWisdom), EffectHandler(EffectsEnum.Effect_SubCriticalHit), EffectHandler(EffectsEnum.Effect_SubIntelligence), EffectHandler(EffectsEnum.Effect_SubAgility), EffectHandler(EffectsEnum.Effect_SubRange), EffectHandler(EffectsEnum.Effect_SubVitality)]
	public class StatsDebuff : SpellEffectHandler
	{
		public StatsDebuff(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
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
				if (this.Effect.Duration > 0)
				{
					base.AddStatBuff(current, Convert.ToInt16(-effectInteger.Value), StatsDebuff.GetEffectCaracteristic(this.Effect.EffectId), true);
				}
			}
			result = true;
			return result;
		}
		public static PlayerFields GetEffectCaracteristic(EffectsEnum effect)
		{
			if (effect <= EffectsEnum.Effect_SubDamageBonus)
			{
				if (effect == EffectsEnum.Effect_SubRange || effect == EffectsEnum.Effect_SubRange_135)
				{
					PlayerFields result = PlayerFields.Range;
					return result;
				}
				if (effect == EffectsEnum.Effect_SubDamageBonus)
				{
					PlayerFields result = PlayerFields.DamageBonus;
					return result;
				}
			}
			else
			{
				if (effect <= EffectsEnum.Effect_SubCriticalHit)
				{
					switch (effect)
					{
					case EffectsEnum.Effect_SubChance:
					{
						PlayerFields result = PlayerFields.Chance;
						return result;
					}
					case EffectsEnum.Effect_SubVitality:
					case EffectsEnum.Effect_IncreaseWeight:
					case EffectsEnum.Effect_DecreaseWeight:
						break;
					case EffectsEnum.Effect_SubAgility:
					{
						PlayerFields result = PlayerFields.Agility;
						return result;
					}
					case EffectsEnum.Effect_SubIntelligence:
					{
						PlayerFields result = PlayerFields.Intelligence;
						return result;
					}
					case EffectsEnum.Effect_SubWisdom:
					{
						PlayerFields result = PlayerFields.Wisdom;
						return result;
					}
					case EffectsEnum.Effect_SubStrength:
					{
						PlayerFields result = PlayerFields.Strength;
						return result;
					}
					case EffectsEnum.Effect_SubDodgeAPProbability:
					{
						PlayerFields result = PlayerFields.DodgeAPProbability;
						return result;
					}
					case EffectsEnum.Effect_SubDodgeMPProbability:
					{
						PlayerFields result = PlayerFields.DodgeMPProbability;
						return result;
					}
					default:
						if (effect == EffectsEnum.Effect_SubCriticalHit)
						{
							PlayerFields result = PlayerFields.CriticalHit;
							return result;
						}
						break;
					}
				}
				else
				{
					if (effect == EffectsEnum.Effect_SubDamageBonusPercent)
					{
						PlayerFields result = PlayerFields.DamageBonusPercent;
						return result;
					}
					switch (effect)
					{
					case EffectsEnum.Effect_SubDodge:
					{
						PlayerFields result = PlayerFields.TackleEvade;
						return result;
					}
					case EffectsEnum.Effect_SubLock:
					{
						PlayerFields result = PlayerFields.TackleBlock;
						return result;
					}
					}
				}
			}
			throw new System.Exception(string.Format("'{0}' has no binded caracteristic", effect));
		}
	}
}
