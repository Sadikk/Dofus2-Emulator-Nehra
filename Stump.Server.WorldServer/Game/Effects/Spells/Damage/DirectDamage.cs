using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Enums.HomeMade;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Fights.Buffs;
using Stump.Server.WorldServer.Game.Fights.Buffs.Customs;
using Stump.Server.WorldServer.Game.Spells;
using Stump.Server.WorldServer.Handlers.Actions;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Damage
{
	[EffectHandler(EffectsEnum.Effect_DamageNeutral), EffectHandler(EffectsEnum.Effect_DamageAir), EffectHandler(EffectsEnum.Effect_DamageWater), EffectHandler(EffectsEnum.Effect_DamageFire), EffectHandler(EffectsEnum.Effect_DamageEarth)]
	public class DirectDamage : SpellEffectHandler
	{
		public BuffTriggerType BuffTriggerType
		{
			get;
			set;
		}

		public DirectDamage(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
		{
			this.BuffTriggerType = BuffTriggerType.TURN_BEGIN;
		}
		public override bool Apply()
		{
            if (this.Spell.SpellType.Id == (int)SpellTypesEnum.Sadida)
            {
                var isTargetEnnemy = this.Fight.GetOneFighter(this.TargetedCell).Team != this.Caster.Team;
                if (isTargetEnnemy)
                {
                    var affectedActors = this.Fight.GetAllFighters((x) => x.Team != this.Caster.Team && x.HasState(SpellStatesEnum.Infected));
                    this.SetAffectedActors(affectedActors);
                }
            }
            foreach (FightActor current in base.GetAffectedActors())
			{
                
				if (this.Effect.Duration > 0)
				{
					base.AddTriggerBuff(current, true, this.BuffTriggerType, new TriggerBuffApplyHandler(DirectDamage.DamageBuffTrigger));
				}
				else
				{
					SpellReflectionBuff bestReflectionBuff = current.GetBestReflectionBuff();
					if (bestReflectionBuff != null && bestReflectionBuff.ReflectedLevel >= (int)base.Spell.CurrentLevel && base.Spell.Template.Id != 0)
					{
						this.NotifySpellReflected(current);
                        base.Caster.InflictDamage(new Fights.Damage(base.Dice, DirectDamage.GetEffectSchool(base.Dice.EffectId), current, base.Spell)
						{
							ReflectedDamages = true,
							MarkTrigger = base.MarkTrigger
						});
						current.RemoveAndDispellBuff(bestReflectionBuff);
					}
					else
					{
                        current.InflictDamage(new Fights.Damage(base.Dice, DirectDamage.GetEffectSchool(base.Dice.EffectId), base.Caster, base.Spell)
						{
							MarkTrigger = base.MarkTrigger
						});
					}
				}
			}
			return true;
		}
		private void NotifySpellReflected(FightActor source)
		{
			ActionsHandler.SendGameActionFightReflectSpellMessage(base.Fight.Clients, source, base.Caster);
		}
		private static void DamageBuffTrigger(TriggerBuff buff, BuffTriggerType trigger, object token)
		{
            Fights.Damage damage = new Fights.Damage(buff.Dice, DirectDamage.GetEffectSchool(buff.Dice.EffectId), buff.Caster, buff.Spell)
			{
				Buff = buff
			};
			buff.Target.InflictDamage(damage);
		}
		private static EffectSchoolEnum GetEffectSchool(EffectsEnum effect)
		{
			EffectSchoolEnum result;
			switch (effect)
			{
			case EffectsEnum.Effect_DamageWater:
				result = EffectSchoolEnum.Water;
				break;
			case EffectsEnum.Effect_DamageEarth:
				result = EffectSchoolEnum.Earth;
				break;
			case EffectsEnum.Effect_DamageAir:
				result = EffectSchoolEnum.Air;
				break;
			case EffectsEnum.Effect_DamageFire:
				result = EffectSchoolEnum.Fire;
				break;
			case EffectsEnum.Effect_DamageNeutral:
				result = EffectSchoolEnum.Neutral;
				break;
			default:
				throw new System.Exception(string.Format("Effect {0} has not associated School Type", effect));
			}
			return result;
		}
	}
}
