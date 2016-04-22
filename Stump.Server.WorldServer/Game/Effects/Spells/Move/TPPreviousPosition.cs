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
                //todo : get previous position (-1 deplacement)
                //MapPoint destPoint = fightActor.Position.Point.GetSymmetricCell(base.Caster.Position.Point);
                //Cell destCell = base.Caster.Map.GetCell(destPoint.CellId);
                //FightActor oldFighter = base.Fight.GetOneFighter(destCell);
                //if (oldFighter != null)
                //{
                //    //if there was a fighter on the cell we are going to tp on, we need to move him at our old position
                //    //todo add telefrag state 
                //    oldFighter.Position.Cell = base.Caster.Position.Cell;
                //    ActionsHandler.SendGameActionFightTeleportOnSameMapMessage(base.Fight.Clients, base.Caster, oldFighter, base.Caster.Cell);
                //}
                //// not sure if it is the right client !! todo : check
                //base.Caster.Position.Cell = destCell;
                //ActionsHandler.SendGameActionFightTeleportOnSameMapMessage(base.Fight.Clients, base.Caster, base.Caster, destCell);
            }
            return true;
        }
    }
}
