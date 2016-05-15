using System;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Fights.Buffs.Customs;
using Stump.Server.WorldServer.Game.Spells;
using Stump.Server.WorldServer.Handlers.Actions;
using Stump.DofusProtocol.Enums.HomeMade;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Debuffs
{
    [EffectHandler(EffectsEnum.Effect_SubAPTelefrag)]
    public class APDebuffTelefrag : SpellEffectHandler
    {
        //AP Debuff only when the applied only when the target has the telefrag state
        public APDebuffTelefrag(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
        {
        }
        public override bool Apply()
        {
            
            bool result;
            foreach (FightActor current in base.GetAffectedActors())
            {
                //if (current.HasState((int)SpellStatesEnum.Telefrag))
                //{
                EffectInteger effectInteger = base.GenerateEffect();
                if (effectInteger == null)
                {
                    result = false;
                    return result;
                }
                FightActor fightActor = current;
                SpellReflectionBuff bestReflectionBuff = current.GetBestReflectionBuff();
                if (bestReflectionBuff != null && bestReflectionBuff.ReflectedLevel >= (int)base.Spell.CurrentLevel && base.Spell.Template.Id != 0)
                {
                    this.NotifySpellReflected(current);
                    current.RemoveAndDispellBuff(bestReflectionBuff);
                    fightActor = base.Caster;
                }
                if (this.Effect.Duration > 1)
                {
                    base.AddStatBuff(fightActor, Convert.ToInt16(-effectInteger.Value), PlayerFields.AP, true, 168);
                }
                else
                {
                    fightActor.LostAP(effectInteger.Value);
                }
                //}

            }
            result = true;
            return result;
        }
        private void NotifySpellReflected(FightActor source)
        {
            ActionsHandler.SendGameActionFightReflectSpellMessage(base.Fight.Clients, source, base.Caster);
        }
    }
}