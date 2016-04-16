using Stump.Core.Threading;
using Stump.Server.WorldServer.AI.Fights.Actions;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Fights;
using Stump.Server.WorldServer.Game.Maps.Cells;
using Stump.Server.WorldServer.Game.Maps.Cells.Shapes;
using Stump.Server.WorldServer.Game.Spells;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Stump.Server.WorldServer.AI.Fights.Brain
{
	public class EnvironmentAnalyser
	{
		public AIFightCellsInformationProvider CellInformationProvider
		{
			get;
			private set;
		}
		public AIFighter Fighter
		{
			get;
			private set;
		}
		public Fight Fight
		{
			get
			{
				return this.Fighter.Fight;
			}
		}
		public EnvironmentAnalyser(AIFighter fighter)
		{
			this.Fighter = fighter;
			this.CellInformationProvider = new AIFightCellsInformationProvider(this.Fighter.Fight, this.Fighter);
		}
		public Cell GetFreeAdjacentCell()
		{
			MapPoint mapPoint = this.Fighter.Position.Point.GetAdjacentCells(new Func<short, bool>(this.CellInformationProvider.IsCellWalkable)).FirstOrDefault<MapPoint>();
			Cell result;
			if (mapPoint != null)
			{
				result = this.CellInformationProvider.GetCellInformation(mapPoint.CellId).Cell;
			}
			else
			{
				result = null;
			}
			return result;
		}
		public Cell GetCellToCastSpell(FightActor target, Spell spell)
		{
			MapPoint mapPoint = (
				from entry in target.Position.Point.GetAdjacentCells(new Func<short, bool>(this.CellInformationProvider.IsCellWalkable))
				orderby entry.DistanceToCell(this.Fighter.Position.Point)
				select entry).FirstOrDefault<MapPoint>();
			Cell result;
			if (mapPoint == null)
			{
				result = null;
			}
			else
			{
				result = this.CellInformationProvider.GetCellInformation(mapPoint.CellId).Cell;
			}
			return result;
		}
        public Cell GetCellToFlee()
        {
            AsyncRandom asyncRandom = new AsyncRandom();
            Cell[] movementsCells = this.GetMovementCells();
            IEnumerable<FightActor> allFighters = this.Fight.GetAllFighters((Predicate<FightActor>)(entry => entry.IsEnnemyWith((FightActor)this.Fighter)));
            long num1 = Enumerable.Sum<FightActor>(allFighters, (Func<FightActor, long>)(entry => (long)entry.Position.Point.DistanceToCell(this.Fighter.Position.Point)));
            Cell cell = (Cell)null;
            long num2 = 0L;
            for (int i = 0; i < movementsCells.Length; ++i)
            {
                if (this.CellInformationProvider.IsCellWalkable(movementsCells[i].Id))
                {
                    long num3 = Enumerable.Sum<FightActor>(allFighters, (Func<FightActor, long>)(entry => (long)entry.Position.Point.DistanceToCell(new MapPoint(movementsCells[i]))));
                    if (num2 < num3)
                    {
                        num2 = num3;
                        cell = movementsCells[i];
                    }
                    else if ((num2 != num3 ? 1 : (asyncRandom.Next(2) != 0 ? 1 : 0)) == 0)
                    {
                        num2 = num3;
                        cell = movementsCells[i];
                    }
                }
            }
            return num1 != num2 ? cell : this.Fighter.Cell;
        }
		public Cell[] GetMovementCells()
		{
			return this.GetMovementCells(this.Fighter.MP);
		}
		public Cell[] GetMovementCells(int mp)
		{
			Cell[] result;
			if (mp <= 0)
			{
				result = new Cell[0];
			}
			else
			{
				if (mp > 63)
				{
					result = this.Fight.Map.Cells;
				}
				else
				{
					Lozenge lozenge = new Lozenge(0, (byte)mp);
					result = lozenge.GetCells(this.Fighter.Cell, this.Fight.Map);
				}
			}
			return result;
		}
		public FightActor GetNearestFighter()
		{
			return this.GetNearestFighter((FightActor entry) => true);
		}
		public FightActor GetNearestAlly()
		{
			return this.GetNearestFighter((FightActor entry) => entry.IsFriendlyWith(this.Fighter));
		}
		public FightActor GetNearestEnnemy()
		{
			return this.GetNearestFighter((FightActor entry) => entry.IsEnnemyWith(this.Fighter));
		}
		public FightActor GetNearestFighter(System.Predicate<FightActor> predicate)
		{
			return (
				from entry in this.Fight.GetAllFighters((FightActor entry) => predicate(entry) && this.Fighter.CanSee(entry))
				orderby entry.Position.Point.DistanceToCell(this.Fighter.Position.Point)
				select entry).FirstOrDefault<FightActor>();
		}
		public bool IsReachable(FightActor actor)
		{
			System.Collections.Generic.IEnumerable<MapPoint> adjacentCells = actor.Position.Point.GetAdjacentCells((short entry) => this.Fight.Map.Cells[(int)entry].Walkable && !this.Fight.Map.Cells[(int)entry].NonWalkableDuringFight && this.Fight.IsCellFree(this.Fight.Map.Cells[(int)entry]));
			return adjacentCells.Any<MapPoint>();
		}
	}
}
