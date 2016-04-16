using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Fights.Buffs;
using Stump.Server.WorldServer.Game.Spells;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Steals
{
	[EffectHandler(EffectsEnum.Effect_StealHPAir), EffectHandler(EffectsEnum.Effect_StealHPNeutral), EffectHandler(EffectsEnum.Effect_StealHPWater), EffectHandler(EffectsEnum.Effect_StealHPFire), EffectHandler(EffectsEnum.Effect_StealHPEarth)]
	public class HPSteal : SpellEffectHandler
	{
		public HPSteal(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
		{
		}
		public override bool Apply()
		{
			foreach (FightActor current in base.GetAffectedActors())
			{
				if (this.Effect.Duration > 0)
				{
					base.AddTriggerBuff(current, true, BuffTriggerType.TURN_BEGIN, new TriggerBuffApplyHandler(HPSteal.StealHpBuffTrigger));
				}
				else
				{
                    int num = current.InflictDamage(new Fights.Damage(base.Dice, HPSteal.GetEffectSchool(this.Effect.EffectId), base.Caster, base.Spell));
					if (num / 2 > 0)
					{
						base.Caster.HealDirect((int)((short)((double)num / 2.0)), current);
					}
				}
			}
			return true;
		}
		private static void StealHpBuffTrigger(TriggerBuff buff, BuffTriggerType trigger, object token)
		{
			EffectInteger effectInteger = buff.GenerateEffect();
			if (!(effectInteger == null))
			{
				buff.Target.Heal((int)effectInteger.Value, buff.Caster, true);
			}
		}
		private static EffectSchoolEnum GetEffectSchool(EffectsEnum effect)
		{
			EffectSchoolEnum result;
			switch (effect)
			{
			case EffectsEnum.Effect_StealHPWater:
				result = EffectSchoolEnum.Water;
				break;
			case EffectsEnum.Effect_StealHPEarth:
				result = EffectSchoolEnum.Earth;
				break;
			case EffectsEnum.Effect_StealHPAir:
				result = EffectSchoolEnum.Air;
				break;
			case EffectsEnum.Effect_StealHPFire:
				result = EffectSchoolEnum.Fire;
				break;
			case EffectsEnum.Effect_StealHPNeutral:
				result = EffectSchoolEnum.Neutral;
				break;
			default:
				throw new System.Exception(string.Format("Effect {0} has not associated School Type", effect));
			}
			return result;
		}
	}
}
