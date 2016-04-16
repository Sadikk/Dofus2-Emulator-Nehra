using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Fights.Buffs;
using Stump.Server.WorldServer.Game.Spells;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Heal
{
	[EffectHandler(EffectsEnum.Effect_HealHP_81), EffectHandler(EffectsEnum.Effect_HealHP_143), EffectHandler(EffectsEnum.Effect_HealHP_108)]
	public class Heal : SpellEffectHandler
	{
		public Heal(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
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
					base.AddTriggerBuff(current, true, BuffTriggerType.TURN_BEGIN, new TriggerBuffApplyHandler(Heal.HealBuffTrigger));
				}
				else
				{
					current.Heal((int)effectInteger.Value, base.Caster, true);
				}
			}
			result = true;
			return result;
		}
		private static void HealBuffTrigger(TriggerBuff buff, BuffTriggerType trigger, object token)
		{
			EffectInteger effectInteger = buff.GenerateEffect();
			if (!(effectInteger == null))
			{
				buff.Target.Heal((int)effectInteger.Value, buff.Caster, true);
			}
		}
	}
}
