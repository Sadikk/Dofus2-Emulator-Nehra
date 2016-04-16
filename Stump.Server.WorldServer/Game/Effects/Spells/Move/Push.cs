using Stump.Core.Threading;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Maps.Cells;
using Stump.Server.WorldServer.Game.Spells;
using Stump.Server.WorldServer.Handlers.Actions;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Move
{
	[EffectHandler(EffectsEnum.Effect_PushBack_1103), EffectHandler(EffectsEnum.Effect_PushBack)]
	public class Push : SpellEffectHandler
	{
        public Push(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical)
            : base(effect, caster, spell, targetedCell, critical)
        { }

        public override bool Apply()
        {
            EffectInteger integer = base.GenerateEffect();
            if (integer == null)
            {
                return false;
            }
            foreach (FightActor actor in base.GetAffectedActors())
            {
                MapPoint startCell;
                MapPoint endCell;
                FightActor actorCopy;
                MapPoint point = (base.TargetedCell.Id == actor.Cell.Id) ? new MapPoint(base.CastCell) : base.TargetedPoint;
                if (point.CellId != actor.Position.Cell.Id)
                {
                    DirectionsEnum direction = point.OrientationTo(actor.Position.Point, false);
                    startCell = actor.Position.Point;
                    MapPoint point2 = startCell;
                    for (int i = 0; i < integer.Value; i++)
                    {
                        MapPoint nearestCellInDirection = point2.GetNearestCellInDirection(direction);
                        if (nearestCellInDirection == null)
                        {
                            break;
                        }
                        if (base.Fight.ShouldTriggerOnMove(base.Fight.Map.Cells[nearestCellInDirection.CellId]))
                        {
                            point2 = nearestCellInDirection;
                            break;
                        }
                        if ((nearestCellInDirection == null) || !base.Fight.IsCellFree(base.Map.Cells[nearestCellInDirection.CellId]))
                        {
                            int damage = (8 + (new AsyncRandom().Next(1, 8) * (base.Caster.Level / 50))) * (integer.Value - i);
                            actor.InflictDirectDamage(damage, base.Caster);
                            break;
                        }
                        point2 = nearestCellInDirection;
                    }
                    endCell = point2;
                    actorCopy = actor;
                    base.Fight.ForEach(delegate(Character entry)
                    {
                        ActionsHandler.SendGameActionFightSlideMessage(entry.Client, this.Caster, actorCopy, startCell.CellId, endCell.CellId);
                    });
                    actor.Position.Cell = base.Map.Cells[endCell.CellId];
                }
            }
            return true;
        }

	}
}
