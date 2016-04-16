using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Fights.Buffs;
using Stump.Server.WorldServer.Game.Spells;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Damage
{
	[EffectHandler(EffectsEnum.Effect_DamageEarthPerAP), EffectHandler(EffectsEnum.Effect_DamageWaterPerAP), EffectHandler(EffectsEnum.Effect_DamageNeutralPerAP), EffectHandler(EffectsEnum.Effect_DamageAirPerAP), EffectHandler(EffectsEnum.Effect_DamageFirePerAP)]
	public class DamagePerAPUsed : SpellEffectHandler
	{
		public DamagePerAPUsed(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
		{
		}
		public override bool Apply()
		{
			foreach (FightActor current in base.GetAffectedActors())
			{
				base.AddTriggerBuff(current, true, BuffTriggerType.TURN_END, new TriggerBuffApplyHandler(this.OnBuffTriggered));
			}
			return true;
		}
		private void OnBuffTriggered(TriggerBuff buff, BuffTriggerType trigger, object token)
		{
            Fights.Damage damage = new Fights.Damage(base.Dice)
			{
				Source = buff.Caster,
				Buff = buff,
				IgnoreDamageReduction = true,
				School = DamagePerAPUsed.GetEffectSchool(buff.Dice.EffectId),
				MarkTrigger = base.MarkTrigger
			};
			damage.BaseMaxDamages = (int)buff.Target.UsedAP * damage.BaseMaxDamages;
			damage.BaseMinDamages = (int)buff.Target.UsedAP * damage.BaseMinDamages;
			buff.Target.InflictDamage(damage);
		}
		private static EffectSchoolEnum GetEffectSchool(EffectsEnum effect)
		{
			EffectSchoolEnum result;
			switch (effect)
			{
			case EffectsEnum.Effect_DamageAirPerAP:
				result = EffectSchoolEnum.Air;
				break;
			case EffectsEnum.Effect_DamageWaterPerAP:
				result = EffectSchoolEnum.Water;
				break;
			case EffectsEnum.Effect_DamageFirePerAP:
				result = EffectSchoolEnum.Fire;
				break;
			case EffectsEnum.Effect_DamageNeutralPerAP:
				result = EffectSchoolEnum.Neutral;
				break;
			case EffectsEnum.Effect_DamageEarthPerAP:
				result = EffectSchoolEnum.Earth;
				break;
			default:
				throw new System.Exception(string.Format("Effect {0} has not associated School Type", effect));
			}
			return result;
		}
	}
}
