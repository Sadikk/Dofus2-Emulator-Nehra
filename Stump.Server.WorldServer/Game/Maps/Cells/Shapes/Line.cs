using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;

namespace Stump.Server.WorldServer.Game.Maps.Cells.Shapes
{
	public class Line : IShape
	{
		public uint Surface
		{
			get
			{
				return (uint)(this.Radius + 1);
			}
		}
		public byte MinRadius
		{
			get;
			set;
		}
		public DirectionsEnum Direction
		{
			get;
			set;
		}
		public byte Radius
		{
			get;
			set;
		}
		public Line(byte radius)
		{
			this.Radius = radius;
			this.Direction = DirectionsEnum.DIRECTION_SOUTH_EAST;
		}
		public Cell[] GetCells(Cell centerCell, Map map)
		{
			MapPoint mapPoint = new MapPoint(centerCell);
			System.Collections.Generic.List<Cell> list = new System.Collections.Generic.List<Cell>();
			for (int i = (int)this.MinRadius; i <= (int)this.Radius; i++)
			{
				switch (this.Direction)
				{
				case DirectionsEnum.DIRECTION_EAST:
					Line.AddCellIfValid(mapPoint.X + i, mapPoint.Y + i, map, list);
					break;
				case DirectionsEnum.DIRECTION_SOUTH_EAST:
					Line.AddCellIfValid(mapPoint.X + i, mapPoint.Y, map, list);
					break;
				case DirectionsEnum.DIRECTION_SOUTH:
					Line.AddCellIfValid(mapPoint.X + i, mapPoint.Y - i, map, list);
					break;
				case DirectionsEnum.DIRECTION_SOUTH_WEST:
					Line.AddCellIfValid(mapPoint.X, mapPoint.Y - i, map, list);
					break;
				case DirectionsEnum.DIRECTION_WEST:
					Line.AddCellIfValid(mapPoint.X - i, mapPoint.Y - i, map, list);
					break;
				case DirectionsEnum.DIRECTION_NORTH_WEST:
					Line.AddCellIfValid(mapPoint.X - i, mapPoint.Y, map, list);
					break;
				case DirectionsEnum.DIRECTION_NORTH:
					Line.AddCellIfValid(mapPoint.X - i, mapPoint.Y + i, map, list);
					break;
				case DirectionsEnum.DIRECTION_NORTH_EAST:
					Line.AddCellIfValid(mapPoint.X, mapPoint.Y + i, map, list);
					break;
				}
			}
			return list.ToArray();
		}
		private static void AddCellIfValid(int x, int y, Map map, System.Collections.Generic.IList<Cell> container)
		{
			if (MapPoint.IsInMap(x, y))
			{
				container.Add(map.Cells[(int)((System.UIntPtr)MapPoint.CoordToCellId(x, y))]);
			}
		}
	}
}
