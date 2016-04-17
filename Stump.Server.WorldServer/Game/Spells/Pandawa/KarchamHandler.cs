using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Enums.HomeMade;
using Stump.DofusProtocol.Messages;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Effects.Spells;
using Stump.Server.WorldServer.Game.Effects.Spells.States;
using Stump.Server.WorldServer.Game.Fights.Buffs;
using Stump.Server.WorldServer.Game.Spells.Casts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stump.Server.WorldServer.Game.Spells.Pandawa
{
    [SpellCastHandler(SpellIdEnum.Karcham)]
    public class KarchamHandler : DefaultSpellCastHandler
    {
        public KarchamHandler(FightActor caster, Spell spell, Cell targetedCell, bool critical)  : base(caster, spell, targetedCell, critical)
        {
        }

        public override void Execute()
        {
            //Made this code because it miss the state for caster and target
            //Another solution : add the effect in the database because isn't in d2o files.
            var id = this.Caster.PopNextBuffId();
            var target = this.Fight.GetOneFighter(this.TargetedCell);
            var effect = new EffectBase { Duration = -1 };
            var actionId = (short)ActionsEnum.ACTION_CARRY_CHARACTER;
            var casterStateId = (uint)SpellStatesEnum.Carrying;
            var targetStateId = (uint)SpellStatesEnum.Carried;
            var casterState = Singleton<SpellManager>.Instance.GetSpellState(casterStateId);
            var targetState = Singleton<SpellManager>.Instance.GetSpellState(targetStateId);

            StateBuff casterBuff = new StateBuff(id, target, this.Caster, effect, this.Spell, false, actionId, casterState);
            StateBuff targetBuff = new StateBuff(id, target, this.Caster, effect, this.Spell, false, actionId, targetState);

            this.Caster.AddAndApplyBuff(casterBuff);
            target.AddAndApplyBuff(targetBuff);

            this.OnExecute(actionId, target);
            base.Execute();
        }
        private void OnExecute(short actionId, FightActor target)
        {
            this.Fight.Clients.Send(new GameActionFightCarryCharacterMessage((ushort)actionId, this.Caster.Id, target.Id, target.Cell.Id));
        }
    }
}
