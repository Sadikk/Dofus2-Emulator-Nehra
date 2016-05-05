using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Network;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Maps.Cells;
using Stump.Server.WorldServer.Game.Spells;
using Stump.Server.WorldServer.Handlers.Actions;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Move
{
	[EffectHandler(EffectsEnum.Effect_PullForward)]
	public class Pull : SpellEffectHandler
	{
        public Pull(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical)
            : base(effect, caster, spell, targetedCell, critical)
        { }

        public override bool Apply()
        {
            EffectInteger effectInteger = this.GenerateEffect();
            bool flag;
            if (effectInteger == null)
            {
                flag = false;
            }
            else
            {
                foreach (FightActor target in this.GetAffectedActors())
                {
                    if (target.CanBeMove())
                    {
                        MapPoint point1 = (int)this.TargetedCell.Id != (int)target.Cell.Id ? this.TargetedPoint : new MapPoint(this.CastCell);
                        if ((int)point1.CellId != (int)target.Position.Cell.Id)
                        {
                            DirectionsEnum direction = target.Position.Point.OrientationTo(point1, false);
                            MapPoint point2 = target.Position.Point;
                            MapPoint mapPoint1 = point2;
                            for (int index = 0; index < (int)effectInteger.Value; ++index)
                            {
                                MapPoint nearestCellInDirection = mapPoint1.GetNearestCellInDirection(direction);
                                if (nearestCellInDirection != null)
                                {
                                    if (!this.Fight.ShouldTriggerOnMove(this.Fight.Map.Cells[(int)nearestCellInDirection.CellId]))
                                    {
                                        if (this.Fight.IsCellFree(this.Map.Cells[(int)nearestCellInDirection.CellId]))
                                            mapPoint1 = nearestCellInDirection;
                                        else
                                            break;
                                    }
                                    else
                                    {
                                        mapPoint1 = nearestCellInDirection;
                                        break;
                                    }
                                }
                                else
                                    break;
                            }
                            MapPoint mapPoint2 = mapPoint1;
                            target.Position.Cell = this.Map.Cells[(int)mapPoint2.CellId];
                            ActionsHandler.SendGameActionFightSlideMessage((IPacketReceiver)this.Fight.Clients, this.Caster, target, point2.CellId, mapPoint2.CellId);
                        }
                    }
                }
                flag = true;
            }
            return flag;
        }
    }
}
