using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Maps.Cells.Shapes
{
	public class Cross : IShape
	{
		public bool Diagonal
		{
			get;
			set;
		}
		public System.Collections.Generic.List<DirectionsEnum> DisabledDirections
		{
			get;
			set;
		}
		public bool OnlyPerpendicular
		{
			get;
			set;
		}
		public bool AllDirections
		{
			get;
			set;
		}
		public uint Surface
		{
			get
			{
				return (uint)(this.Radius * 4 + 1);
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
		public Cross(byte minRadius, byte radius)
		{
			this.MinRadius = minRadius;
			this.Radius = radius;
			this.DisabledDirections = new System.Collections.Generic.List<DirectionsEnum>();
		}
		public Cell[] GetCells(Cell centerCell, Map map)
		{
			System.Collections.Generic.List<Cell> list = new System.Collections.Generic.List<Cell>();
			if (this.MinRadius == 0)
			{
				list.Add(centerCell);
			}
			System.Collections.Generic.List<DirectionsEnum> list2 = this.DisabledDirections.ToList<DirectionsEnum>();
			if (this.OnlyPerpendicular)
			{
				switch (this.Direction)
				{
				case DirectionsEnum.DIRECTION_EAST:
				case DirectionsEnum.DIRECTION_WEST:
					list2.Add(DirectionsEnum.DIRECTION_EAST);
					list2.Add(DirectionsEnum.DIRECTION_WEST);
					break;
				case DirectionsEnum.DIRECTION_SOUTH_EAST:
				case DirectionsEnum.DIRECTION_NORTH_WEST:
					list2.Add(DirectionsEnum.DIRECTION_SOUTH_EAST);
					list2.Add(DirectionsEnum.DIRECTION_NORTH_WEST);
					break;
				case DirectionsEnum.DIRECTION_SOUTH:
				case DirectionsEnum.DIRECTION_NORTH:
					list2.Add(DirectionsEnum.DIRECTION_SOUTH);
					list2.Add(DirectionsEnum.DIRECTION_NORTH);
					break;
				case DirectionsEnum.DIRECTION_SOUTH_WEST:
				case DirectionsEnum.DIRECTION_NORTH_EAST:
					list2.Add(DirectionsEnum.DIRECTION_NORTH_EAST);
					list2.Add(DirectionsEnum.DIRECTION_SOUTH_WEST);
					break;
				}
			}
			MapPoint mapPoint = new MapPoint(centerCell);
			for (int i = (int)this.Radius; i > 0; i--)
			{
				if (i >= (int)this.MinRadius)
				{
					if (!this.Diagonal)
					{
						if (!list2.Contains(DirectionsEnum.DIRECTION_SOUTH_EAST))
						{
							Cross.AddCellIfValid(mapPoint.X + i, mapPoint.Y, map, list);
						}
						if (!list2.Contains(DirectionsEnum.DIRECTION_NORTH_WEST))
						{
							Cross.AddCellIfValid(mapPoint.X - i, mapPoint.Y, map, list);
						}
						if (!list2.Contains(DirectionsEnum.DIRECTION_NORTH_EAST))
						{
							Cross.AddCellIfValid(mapPoint.X, mapPoint.Y + i, map, list);
						}
						if (!list2.Contains(DirectionsEnum.DIRECTION_SOUTH_WEST))
						{
							Cross.AddCellIfValid(mapPoint.X, mapPoint.Y - i, map, list);
						}
					}
					if (this.Diagonal || this.AllDirections)
					{
						if (!list2.Contains(DirectionsEnum.DIRECTION_SOUTH))
						{
							Cross.AddCellIfValid(mapPoint.X + i, mapPoint.Y - i, map, list);
						}
						if (!list2.Contains(DirectionsEnum.DIRECTION_NORTH))
						{
							Cross.AddCellIfValid(mapPoint.X - i, mapPoint.Y + i, map, list);
						}
						if (!list2.Contains(DirectionsEnum.DIRECTION_EAST))
						{
							Cross.AddCellIfValid(mapPoint.X + i, mapPoint.Y + i, map, list);
						}
						if (!list2.Contains(DirectionsEnum.DIRECTION_WEST))
						{
							Cross.AddCellIfValid(mapPoint.X - i, mapPoint.Y - i, map, list);
						}
					}
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
