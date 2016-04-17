using System;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Messages;
using Stump.Server.BaseServer.Network;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Spells;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Others
{
    [EffectHandler(EffectsEnum.Effect_Carry)]
    class Carry : SpellEffectHandler
    {
        public Carry(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
        {
        }
        public override bool Apply()
        {
            FightActor target = this.Fight.GetOneFighter(this.TargetedCell);

            if(target == null)
            {
                return false;
            }

            this.Caster.Carry(target);
            this.OnApply(target.Id);

            return true;
        }
        private void OnApply(int targetId)
        {
            foreach (var client in this.Fight.Clients)
            {
                var actionId = (ushort)ActionsEnum.ACTION_CARRY_CHARACTER;
                client.Send(new GameActionFightCarryCharacterMessage(actionId, this.Caster.Id, targetId, this.TargetedCell.Id));
            }
        }
    }
}
