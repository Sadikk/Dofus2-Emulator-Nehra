using System.Linq;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Spells;
using Stump.Server.WorldServer.Handlers.Actions;
using Stump.Server.WorldServer.Game.Maps.Cells;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Move
{
    [EffectHandler(EffectsEnum.Effect_TPPreviousPosition)]
    public class TPPreviousPosition : SpellEffectHandler
    {
        public TPPreviousPosition(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
        {
        }
        public override bool Apply()
        {
            FightActor fightActor = base.GetAffectedActors().FirstOrDefault<FightActor>();

            if (fightActor != null)
            {
                if (fightActor.PreviousPosition != null)
                {
                    Cell destCell = this.Fight.Cells[fightActor.Position.Point.GetNearestCellInOppositeDirection(fightActor.Position.Direction).CellId];
                    //Cell destCell = fightActor.PreviousPosition.Cell;
                    FightActor oldFighter = base.Fight.GetOneFighter(destCell);
                    if (oldFighter != null)
                    {
                        //if there was a fighter on the cell we are going to tp on, we need to move him at our old position
                        oldFighter.Position.Cell = base.Caster.Position.Cell;
                        oldFighter.AddTelefragState(base.Caster, base.Spell);
                        base.Caster.AddTelefragState(base.Caster, base.Spell);
                        ActionsHandler.SendGameActionFightTeleportOnSameMapMessage(base.Fight.Clients, base.Caster, oldFighter, base.Caster.Cell);
                    }
                    fightActor.Position.Cell = destCell;
                    ActionsHandler.SendGameActionFightTeleportOnSameMapMessage(base.Fight.Clients, base.Caster, fightActor, destCell);
                }
            }
            return true;
        }
    }
}
