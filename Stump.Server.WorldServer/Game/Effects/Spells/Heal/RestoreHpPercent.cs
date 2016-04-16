using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Fights.Buffs;
using Stump.Server.WorldServer.Game.Spells;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Heal
{
	[EffectHandler(EffectsEnum.Effect_RestoreHPPercent)]
	public class RestoreHpPercent : SpellEffectHandler
	{
		public RestoreHpPercent(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
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
					base.AddTriggerBuff(current, true, BuffTriggerType.TURN_BEGIN, new TriggerBuffApplyHandler(this.OnBuffTriggered));
				}
				else
				{
					this.HealHpPercent(current, (int)effectInteger.Value);
				}
			}
			result = true;
			return result;
		}
		private void OnBuffTriggered(TriggerBuff buff, BuffTriggerType trigger, object token)
		{
			EffectInteger effectInteger = base.GenerateEffect();
			if (!(effectInteger == null))
			{
				this.HealHpPercent(buff.Target, (int)effectInteger.Value);
			}
		}
		private void HealHpPercent(FightActor actor, int percent)
		{
			int healPoints = (int)((double)actor.MaxLifePoints * ((double)percent / 100.0));
			actor.Heal(healPoints, base.Caster, false);
		}
	}
}
