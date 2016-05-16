using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Maps.Cells;
using Stump.Server.WorldServer.Game.Spells.Casts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stump.Server.WorldServer.Game.Spells.Sadida
{
    [SpellCastHandler(SpellIdEnum.Earthquake)]
    class EarthquakeHandler : DefaultSpellCastHandler
    {
        //CONST
        private const int LINE_RANGE = 3;
        private const int DIAGONAL_RANGE = 2;
        
        public EarthquakeHandler(FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(caster, spell, targetedCell, critical)
        {
        }

        public override void Execute()
        {
            var trees = from entry
                        in this.Caster.GetSummons()
                        where (entry as SummonedMonster).IsLeafyTree
                        select entry;

            foreach (var tree in trees)
            {
                //var directions = new DirectionsEnum[]
                //{
                //    DirectionsEnum.DIRECTION_NORTH,
                //    DirectionsEnum.DIRECTION_NORTH_EAST,
                //    DirectionsEnum.DIRECTION_EAST,
                //    DirectionsEnum.DIRECTION_SOUTH_EAST,
                //    DirectionsEnum.DIRECTION_SOUTH,
                //    DirectionsEnum.DIRECTION_SOUTH_WEST,
                //    DirectionsEnum.DIRECTION_WEST,
                //    DirectionsEnum.DIRECTION_NORTH_WEST
                //};

                //var northCells = tree.Position.Point.GetCellsInDirection(DirectionsEnum.DIRECTION_NORTH, 1, DIAGONAL_RANGE).OrderBy(cell => cell.DistanceTo(Caster.Position.Point));
                //var eastCells = tree.Position.Point.GetCellsInDirection(DirectionsEnum.DIRECTION_EAST, 1, DIAGONAL_RANGE);
                //var southCells = tree.Position.Point.GetCellsInDirection(DirectionsEnum.DIRECTION_SOUTH, 1, DIAGONAL_RANGE);
                //var westCells = tree.Position.Point.GetCellsInDirection(DirectionsEnum.DIRECTION_WEST, 1, DIAGONAL_RANGE);

                //var northEastCells = tree.Position.Point.GetCellsInDirection(DirectionsEnum.DIRECTION_NORTH_EAST, 1, LINE_RANGE);
                //var southEastCells = tree.Position.Point.GetCellsInDirection(DirectionsEnum.DIRECTION_SOUTH_EAST, 1, LINE_RANGE);
                //var southWestCells = tree.Position.Point.GetCellsInDirection(DirectionsEnum.DIRECTION_SOUTH_WEST, 1, LINE_RANGE);
                //var northWestCells = tree.Position.Point.GetCellsInDirection(DirectionsEnum.DIRECTION_NORTH_WEST, 1, LINE_RANGE);

                var affectedMapPoints = this.Caster.Position.Point.GetAllCellsInRange(0, LINE_RANGE, false, null);
                foreach(var mapPoint in affectedMapPoints)
                {
                    var targetedCell = this.Fight.Cells.Where(cell => cell.Id == mapPoint.CellId).First();
                    var target = this.Fight.GetOneFighter(targetedCell);
                    if(target != null)
                    {
                        var orientation = target.Position.Point.OrientationTo(this.Caster.Position.Point);
                        var newCellId = target.Position.Point.GetNearestCellInDirection(orientation).CellId;
                        var newCell = this.Fight.Cells.Where(cell => cell.Id == newCellId).First();
                        target.Cell = newCell;
                    }
                }
            }
        }
    }
}
