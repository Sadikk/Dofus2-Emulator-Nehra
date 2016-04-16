using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;

namespace Stump.Server.WorldServer.Game.Maps.Cells.Shapes
{
	public class Lozenge : IShape
	{
		public uint Surface
		{
			get
			{
				return (uint)((this.Radius + 1) * (this.Radius + 1) + this.Radius * this.Radius);
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
		public Lozenge(byte minRadius, byte radius)
		{
			this.MinRadius = minRadius;
			this.Radius = radius;
		}
		public Cell[] GetCells(Cell centerCell, Map map)
		{
			MapPoint mapPoint = new MapPoint(centerCell);
			System.Collections.Generic.List<Cell> list = new System.Collections.Generic.List<Cell>();
			Cell[] result;
			if (this.Radius == 0)
			{
				if (this.MinRadius == 0)
				{
					list.Add(centerCell);
				}
				result = list.ToArray();
			}
			else
			{
				int i = mapPoint.X - (int)this.Radius;
				int num = 0;
				int num2 = 1;
				while (i <= mapPoint.X + (int)this.Radius)
				{
					for (int j = -num; j <= num; j++)
					{
						if (this.MinRadius == 0 || System.Math.Abs(mapPoint.X - i) + System.Math.Abs(j) >= (int)this.MinRadius)
						{
							Lozenge.AddCellIfValid(i, j + mapPoint.Y, map, list);
						}
					}
					if (num == (int)this.Radius)
					{
						num2 = -num2;
					}
					num += num2;
					i++;
				}
				result = list.ToArray();
			}
			return result;
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
