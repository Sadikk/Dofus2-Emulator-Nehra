using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Fights.Buffs;
using Stump.Server.WorldServer.Game.Spells;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Buffs
{
    [EffectHandler(EffectsEnum.Effect_AddAP_111)]
    public class DelayedAPBuff : SpellEffectHandler
    {
        public DelayedAPBuff(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
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
                //var entry = new TriggerBuffApplyHandler(BuffHandler);
                base.AddTriggerBuff(current, false, BuffTriggerType.TURN_BEGIN, new Fights.Buffs.TriggerBuffApplyHandler(BuffHandler));
                //Effect.Delay;
                //if (this.Effect.Duration > 0)
                //{
                //    base.AddStatBuff(current, effectInteger.Value, PlayerFields.AP, true);
                //}
                //else
                //{
                //    current.RegainAP(effectInteger.Value);
                //}
            }
            result = true;
            return result;
        }

        public static void BuffHandler(TriggerBuff buff, BuffTriggerType type, object token)
        {
            EffectInteger effectInteger = buff.GenerateEffect();
            if (!(effectInteger == null))
            {
                int id = buff.Target.PopNextBuffId();
                StatBuff statBuff = new StatBuff(id, buff.Target, buff.Caster, buff.Effect, buff.Spell, effectInteger.Value, PlayerFields.AP, false, false);
                buff.Target.AddAndApplyBuff(statBuff, true);
            }
        }
    }

   
}

