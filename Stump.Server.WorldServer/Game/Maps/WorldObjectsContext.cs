using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Maps.Cells;
using System.Collections.Generic;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Maps
{
	public abstract class WorldObjectsContext
	{
		protected abstract IReadOnlyCollection<WorldObject> Objects
		{
			get;
		}
		public abstract Cell[] Cells
		{
			get;
		}
		public bool CanBeSeen(Cell from, Cell to, bool throughEntities = false)
		{
			bool result;
			if (from == null || to == null)
			{
				result = false;
			}
			else
			{
				if (from == to)
				{
					result = true;
				}
				else
				{
					short[] array = new short[0];
					if (!throughEntities)
					{
						array = (
							from x in this.Objects
							where x.BlockSight
							select x.Cell.Id).ToArray<short>();
					}
					System.Collections.Generic.IEnumerable<MapPoint> cellsInLine = MapPoint.GetPoint(from).GetCellsInLine(MapPoint.GetPoint(to));
					foreach (MapPoint current in cellsInLine.Skip(1))
					{
						if (to.Id != current.CellId)
						{
							Cell cell = this.Cells[(int)current.CellId];
							if (!cell.LineOfSight || (!throughEntities && System.Array.IndexOf<short>(array, current.CellId) != -1))
							{
								result = false;
								return result;
							}
						}
					}
					result = true;
				}
			}
			return result;
		}
	}
}
