using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;

namespace Stump.Server.WorldServer.Game.Maps.Cells.Shapes
{
	public class HalfLozenge : IShape
	{
		public uint Surface
		{
			get
			{
				return (uint)(this.Radius * 2 + 1);
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
		public HalfLozenge(byte minRadius, byte radius)
		{
			this.MinRadius = minRadius;
			this.Radius = radius;
			this.Direction = DirectionsEnum.DIRECTION_NORTH;
		}
		public Cell[] GetCells(Cell centerCell, Map map)
		{
			MapPoint mapPoint = new MapPoint(centerCell);
			System.Collections.Generic.List<Cell> list = new System.Collections.Generic.List<Cell>();
			if (this.MinRadius == 0)
			{
				list.Add(centerCell);
			}
			for (int i = 1; i <= (int)this.Radius; i++)
			{
				switch (this.Direction)
				{
				case DirectionsEnum.DIRECTION_SOUTH_EAST:
					HalfLozenge.AddCellIfValid(mapPoint.X - i, mapPoint.Y + i, map, list);
					HalfLozenge.AddCellIfValid(mapPoint.X - i, mapPoint.Y - i, map, list);
					break;
				case DirectionsEnum.DIRECTION_SOUTH_WEST:
					HalfLozenge.AddCellIfValid(mapPoint.X - i, mapPoint.Y + i, map, list);
					HalfLozenge.AddCellIfValid(mapPoint.X + i, mapPoint.Y + i, map, list);
					break;
				case DirectionsEnum.DIRECTION_NORTH_WEST:
					HalfLozenge.AddCellIfValid(mapPoint.X + i, mapPoint.Y + i, map, list);
					HalfLozenge.AddCellIfValid(mapPoint.X + i, mapPoint.Y - i, map, list);
					break;
				case DirectionsEnum.DIRECTION_NORTH_EAST:
					HalfLozenge.AddCellIfValid(mapPoint.X - i, mapPoint.Y - i, map, list);
					HalfLozenge.AddCellIfValid(mapPoint.X + i, mapPoint.Y - i, map, list);
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
