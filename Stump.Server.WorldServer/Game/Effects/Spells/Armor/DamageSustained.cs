using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Fights.Buffs;
using Stump.Server.WorldServer.Game.Spells;
using Stump.Server.WorldServer.Game.Fights;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Armor
{
    [EffectHandler(EffectsEnum.Effect_DamageSustained)]
    public class DamageSustained : SpellEffectHandler
    {
        public DamageSustained(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
        {
        }
        public override bool Apply()
        {
            bool result;
            foreach (FightActor current in base.GetAffectedActors())
            {
                EffectInteger left = this.Effect.GenerateEffect(EffectGenerationContext.Spell, EffectGenerationType.Normal) as EffectInteger;
                if (left == null)
                {
                    result = false;
                    return result;
                }
                if (this.Effect.Duration <= 0)
                {
                    result = false;
                    return result;
                }
                base.AddTriggerBuff(current, true, BuffTriggerType.BUFF_ADDED, new TriggerBuffApplyHandler(DamageSustained.ApplyArmorBuff), new TriggerBuffRemoveHandler(DamageSustained.RemoveArmorBuff));
            }
            result = true;
            return result;
        }
        public static void ApplyArmorBuff(TriggerBuff buff, BuffTriggerType trigger, object token)
        {
            EffectInteger effectInteger = buff.GenerateEffect();
            if (!(effectInteger == null))
            {
                buff.Target.DamageMultiplicator = effectInteger.Value / 100;
            }
        }

        public static void RemoveArmorBuff(TriggerBuff buff)
        {
            buff.Target.DamageMultiplicator = 1.0;
        }
    }
}
