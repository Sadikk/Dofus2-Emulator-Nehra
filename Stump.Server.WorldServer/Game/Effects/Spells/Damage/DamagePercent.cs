using Stump.DofusProtocol.Enums;
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
	[EffectHandler(EffectsEnum.Effect_DamagePercentFire), EffectHandler(EffectsEnum.Effect_DamagePercentAir), EffectHandler(EffectsEnum.Effect_DamagePercentNeutral), EffectHandler(EffectsEnum.Effect_DamagePercentEarth), EffectHandler(EffectsEnum.Effect_DamagePercentWater)]
	public class DamagePercent : SpellEffectHandler
	{
		public DamagePercent(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
		{
		}
		public override bool Apply()
		{
			foreach (FightActor current in base.GetAffectedActors())
			{
				if (this.Effect.Duration > 0)
				{
					base.AddTriggerBuff(current, true, BuffTriggerType.TURN_BEGIN, new TriggerBuffApplyHandler(DamagePercent.DamageBuffTrigger));
				}
				else
				{
                    Fights.Damage damage = new Fights.Damage(base.Dice, DamagePercent.GetEffectSchool(base.Dice.EffectId), base.Caster, base.Spell);
					damage.GenerateDamages();
					damage.Amount = (int)((double)current.MaxLifePoints * ((double)damage.Amount / 100.0));
					damage.IgnoreDamageBoost = true;
					damage.MarkTrigger = base.MarkTrigger;
					SpellReflectionBuff bestReflectionBuff = current.GetBestReflectionBuff();
					if (bestReflectionBuff != null && bestReflectionBuff.ReflectedLevel >= (int)base.Spell.CurrentLevel && base.Spell.Template.Id != 0)
					{
						this.NotifySpellReflected(current);
						damage.Source = current;
						damage.ReflectedDamages = true;
						base.Caster.InflictDamage(damage);
						current.RemoveAndDispellBuff(bestReflectionBuff);
					}
					else
					{
						current.InflictDamage(damage);
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
			EffectInteger left = buff.GenerateEffect();
			if (!(left == null))
			{
                Fights.Damage damage = new Fights.Damage(buff.Dice, DamagePercent.GetEffectSchool(buff.Dice.EffectId), buff.Caster, buff.Spell)
				{
					Buff = buff
				};
				damage.GenerateDamages();
				damage.Amount = (int)((double)buff.Target.MaxLifePoints * ((double)damage.Amount / 100.0));
				damage.IgnoreDamageBoost = true;
				buff.Target.InflictDamage(damage);
			}
		}
		private static EffectSchoolEnum GetEffectSchool(EffectsEnum effect)
		{
			EffectSchoolEnum result;
			switch (effect)
			{
			case EffectsEnum.Effect_DamagePercentWater:
				result = EffectSchoolEnum.Fire;
				break;
			case EffectsEnum.Effect_DamagePercentEarth:
				result = EffectSchoolEnum.Earth;
				break;
			case EffectsEnum.Effect_DamagePercentAir:
				result = EffectSchoolEnum.Water;
				break;
			case EffectsEnum.Effect_DamagePercentFire:
				result = EffectSchoolEnum.Air;
				break;
			case EffectsEnum.Effect_DamagePercentNeutral:
				result = EffectSchoolEnum.Neutral;
				break;
			default:
				throw new System.Exception(string.Format("Effect {0} has not associated School Type", effect));
			}
			return result;
		}
	}
}
