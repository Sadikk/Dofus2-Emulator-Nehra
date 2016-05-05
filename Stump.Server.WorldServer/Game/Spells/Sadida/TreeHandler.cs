using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Enums.HomeMade;
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
        private const int arg2 = 2; //diceSide or DiceFace?
        private const int delay = 1; //value
        //(this._effect as EffectInstanceDice).diceNum = param1;
        //    (this._effect as EffectInstanceDice).diceSide = param2;
        //    (this._effect as EffectInstanceDice).value = param3;
        private const int BONES_ID = 3164;
        private const int DURATION = 1;
        private const int IMPROVEMENT_LIFE_RATIO = 2;

        public TreeHandler(FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(caster, spell, targetedCell, critical)
        {

        }

        public override void Execute()
        {
            base.Execute();

            var target = this.Caster.Team.GetOneTree(this.TargetedCell);
            if (target != null)
            {
                var id = target.PopNextBuffId();
                var effect = new EffectDice() { Duration = DURATION };
                var actionId = (short)ActionsEnum.ACTION_793;
                var buff = new TriggerBuff(id, target, target, effect, base.Spell, false, true, BuffTriggerType.BUFF_ADDED, new TriggerBuffApplyHandler(this.TreeTrigger), actionId);

                target.AddAndApplyBuff(buff);
                ContextHandler.SendGameActionFightDispellableEffectMessage(base.Caster.Fight.Clients, buff);
            }
        }

        private void TreeTrigger(TriggerBuff buff, BuffTriggerType trigger, object token)
        {
            var num = buff.Target.Stats.Health.Context;
            buff.Target.Stats.Health.Base *= IMPROVEMENT_LIFE_RATIO;
            buff.Target.Look.BonesID = BONES_ID;

            var state = Singleton<SpellManager>.Instance.GetSpellState((uint)SpellStatesEnum.Leafy);
            buff.Target.AddState(state);
        }
    }
}
