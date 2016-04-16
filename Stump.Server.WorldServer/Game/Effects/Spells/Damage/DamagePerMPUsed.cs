using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Fights.Buffs;
using Stump.Server.WorldServer.Game.Spells;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Damage
{
	[EffectHandler(EffectsEnum.Effect_DamageWaterPerMP), EffectHandler(EffectsEnum.Effect_DamageFirePerMP), EffectHandler(EffectsEnum.Effect_DamageEarthPerMP), EffectHandler(EffectsEnum.Effect_DamageNeutralPerMP), EffectHandler(EffectsEnum.Effect_DamageAirPerMP)]
	public class DamagePerMPUsed : SpellEffectHandler
	{
		public DamagePerMPUsed(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
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
				School = DamagePerMPUsed.GetEffectSchool(buff.Dice.EffectId),
				MarkTrigger = base.MarkTrigger
			};
			damage.BaseMaxDamages = (int)buff.Target.UsedMP * damage.BaseMaxDamages;
			damage.BaseMinDamages = (int)buff.Target.UsedMP * damage.BaseMinDamages;
			buff.Target.InflictDamage(damage);
		}
		private static EffectSchoolEnum GetEffectSchool(EffectsEnum effect)
		{
			EffectSchoolEnum result;
			switch (effect)
			{
			case EffectsEnum.Effect_DamageAirPerMP:
				result = EffectSchoolEnum.Air;
				break;
			case EffectsEnum.Effect_DamageWaterPerMP:
				result = EffectSchoolEnum.Water;
				break;
			case EffectsEnum.Effect_DamageFirePerMP:
				result = EffectSchoolEnum.Fire;
				break;
			case EffectsEnum.Effect_DamageNeutralPerMP:
				result = EffectSchoolEnum.Neutral;
				break;
			case EffectsEnum.Effect_DamageEarthPerMP:
				result = EffectSchoolEnum.Earth;
				break;
			default:
				throw new System.Exception(string.Format("Effect {0} has not associated School Type", effect));
			}
			return result;
		}
	}
}
