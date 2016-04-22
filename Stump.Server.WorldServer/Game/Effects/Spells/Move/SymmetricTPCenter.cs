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
    [EffectHandler(EffectsEnum.Effect_SymmetricTPCenter)]
    public class Effect_SymmetricTPCenter : SpellEffectHandler
    {
        public Effect_SymmetricTPCenter(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
        {
        }
        public override bool Apply()
        {
            //symmetric tp from the center of the cell ex: temporal dust
            //todo : it applies only when the target has the state "Telefrag"
            foreach (FightActor fightActor in base.GetAffectedActors())
            {
                
                if (fightActor != null)
                {
                    MapPoint destPoint = new MapPoint(base.TargetedCell).GetSymmetricCell(fightActor.Position.Point);
                    Cell destCell = base.Caster.Map.GetCell(destPoint.CellId);
                    FightActor oldFighter = base.Fight.GetOneFighter(destCell);
                    if (oldFighter != null)
                    {
                        //if there was a fighter on the cell we are going to tp on, we need to move him at our old position
                        //todo add telefrag state 
                        oldFighter.Position.Cell = fightActor.Position.Cell;
                        ActionsHandler.SendGameActionFightTeleportOnSameMapMessage(base.Fight.Clients, base.Caster, oldFighter, fightActor.Position.Cell);
                    }
                    // not sure if it is the right client !! todo : check
                    fightActor.Position.Cell = destCell;
                    ActionsHandler.SendGameActionFightTeleportOnSameMapMessage(base.Fight.Clients, base.Caster, fightActor, destCell);
                }
                
            }
            return true;
        }
    }
}
