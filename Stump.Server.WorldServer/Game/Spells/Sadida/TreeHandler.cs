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
        public const int BONES_ID = 3164;
        public const int DURATION = 1;
        public const int LIFE_RATIO = 2;

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
                var effect = new EffectDice() { Duration = DURATION }; //TODO : Add EffectId
                var actionId = (short)ActionsEnum.ACTION_793;
                var buff = new TriggerBuff(id, target, target, effect, base.Spell, false, true, BuffTriggerType.TURN_END, new TriggerBuffApplyHandler(TreeTrigger), actionId);

                target.AddAndApplyBuff(buff);
                ContextHandler.SendGameActionFightDispellableEffectMessage(base.Caster.Fight.Clients, buff);
            }
        }

        public static void TreeTrigger(TriggerBuff buff, BuffTriggerType trigger, object token)
        {
            var summonedMonster = buff.Target as SummonedMonster;
            if (summonedMonster != null && !summonedMonster.IsLeafyTree)
            {
                var actorLook = buff.Target.Look.Clone();
                actorLook.BonesID = BONES_ID;
                buff.Target.Look = actorLook;

                var state = Singleton<SpellManager>.Instance.GetSpellState((uint)SpellStatesEnum.Leafy);
                buff.Target.AddState(state);
            }
        }
    }
}
