using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Fights.Buffs;
using Stump.Server.WorldServer.Game.Spells.Casts;
using Stump.Server.WorldServer.Handlers.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stump.Server.WorldServer.Game.Spells.Sadida
{
    [SpellCastHandler(SpellIdEnum.Tree)]
    class TreeHandler : DefaultSpellCastHandler
    {
        //CONSTANTS
        // Need to find the using of them
        private const int arg1 = 5567; //diceNum
        private const int arg2 = 2; //diceSide
        private const int delay = 1; //value
        //(this._effect as EffectInstanceDice).diceNum = param1;
        //    (this._effect as EffectInstanceDice).diceSide = param2;
        //    (this._effect as EffectInstanceDice).value = param3;

        public TreeHandler(FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(caster, spell, targetedCell, critical)
        {

        }

        public override void Execute()
        {
            //buff = new TriggeredBuff(effect as FightTriggeredEffect,castingSpell,actionId);
            //new TriggerBuff()

            /*
            var id = this.Caster.PopNextBuffId();
                var target = this.Fight.GetOneFighter(this.TargetedCell);
                var effect = new EffectBase { Duration = -1 };
                var actionId = (short)ActionsEnum.ACTION_CARRY_CHARACTER;
                var casterStateId = (uint)SpellStatesEnum.Carrying;
                var targetStateId = (uint)SpellStatesEnum.Carried;
                var casterState = Singleton<SpellManager>.Instance.GetSpellState(casterStateId);
                var targetState = Singleton<SpellManager>.Instance.GetSpellState(targetStateId);

                StateBuff casterBuff = new StateBuff(id, this.Caster, this.Caster, effect, this.Spell, false, actionId, casterState);
                */

            var id = this.Caster.PopNextBuffId();
            var target = this.Caster.Team.AddTree(this.Caster as CharacterFighter, this.TargetedCell);
            var effect = new EffectDice();
            effect.GenerateEffect(EffectGenerationContext.Spell);
            var actionId = (short)ActionsEnum.ACTION_793;
            var buff = new TriggerBuff(id, target, target, effect, base.Spell, false, true, BuffTriggerType.TURN_BEGIN, new TriggerBuffApplyHandler(this.TreeTrigger), actionId);
            ContextHandler.SendGameActionFightDispellableEffectMessage(base.Caster.Fight.Clients, buff);
            base.Execute();
        }

        private void TreeTrigger(TriggerBuff buff, BuffTriggerType trigger, object token)
        {
            //Damage damage = token as Damage;
            //if (damage != null && damage.MarkTrigger is Trap)
            //{
            //    Trap trap = damage.MarkTrigger as Trap;
            //    SpellEffectHandler[] handlers = base.Handlers;
            //    for (int i = 0; i < handlers.Length; i++)
            //    {
            //        SpellEffectHandler spellEffectHandler = handlers[i];
            //        spellEffectHandler.SetAffectedActors(
            //            from x in this.m_affectedActors
            //            where trap.ContainsCell(x.Cell)
            //            select x);
            //        spellEffectHandler.Apply();
            //    }
            //}
        }
    }
}
