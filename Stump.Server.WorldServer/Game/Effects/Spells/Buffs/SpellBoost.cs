using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Fights.Buffs;
using Stump.Server.WorldServer.Game.Spells;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Buffs
{
	[EffectHandler(EffectsEnum.Effect_SpellBoost)]
	public class SpellBoost : SpellEffectHandler
	{
        public SpellBoost(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical)
            : base(effect, caster, spell, targetedCell, critical)
        {
            if (effect.DiceNum == 171 && effect.Targets != SpellTargetType.ONLY_SELF)
            {
                effect.Targets = SpellTargetType.ONLY_SELF;
            }
        }

		public override bool Apply()
		{
			bool result;
            foreach (FightActor current in base.GetAffectedActors())
            {
                EffectInteger left = base.GenerateEffect();
                if (left == null)
                {
                    result = false;
                }
                else
                {
                    Spell spell = current.GetSpell((int)base.Dice.DiceNum);
                    if (spell == null)
                    {
                        result = false;
                    }
                    else
                    {
                        Buff buff;
                        if (base.Effect.Delay > 0)
                        {
                            buff = new TriggerBuff(current.PopNextBuffId(), current, base.Caster, base.Dice, base.Spell, base.Critical, false, BuffTriggerType.TURN_BEGIN, new TriggerBuffApplyHandler(this.BoostSpellTrigger))
                            {
                                Duration = (short)base.Effect.Duration
                            };
                        }
                        else
                        {
                            buff = new SpellBuff(current.PopNextBuffId(), current, base.Caster, base.Dice, base.Spell, spell, base.Dice.Value, false, true);
                        }
                        current.AddAndApplyBuff(buff, true);
                    }
                }
            }
			result = true;
			return result;
		}

        private void BoostSpellTrigger(TriggerBuff buff, BuffTriggerType type, object token)
        {
            if (type == BuffTriggerType.TURN_BEGIN)
            {
                Spell boostedSpell = buff.Target.GetSpell((int)buff.Dice.DiceNum);
                var spellBuff = new SpellBuff(buff.Target.PopNextBuffId(), buff.Target, buff.Caster, buff.Dice, buff.Spell, boostedSpell, buff.Dice.Value, false, true);

                buff.Target.AddAndApplyBuff(spellBuff, true);
            }
        }
	}
}
