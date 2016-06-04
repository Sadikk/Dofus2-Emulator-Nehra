using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using System;
using System.Collections.Generic;
using System.Drawing;
namespace Stump.Server.WorldServer.Game.Maps.Cells
{
	public class MapPoint
	{
		public const uint MapWidth = 14u;
		public const uint MapHeight = 20u;
		public const uint MapSize = 560u;
		private static readonly Point VectorRight = new Point(1, 1);
		private static readonly Point VectorDownRight = new Point(1, 0);
		private static readonly Point VectorDown = new Point(1, -1);
		private static readonly Point VectorDownLeft = new Point(0, -1);
		private static readonly Point VectorLeft = new Point(-1, -1);
		private static readonly Point VectorUpLeft = new Point(-1, 0);
		private static readonly Point VectorUp = new Point(-1, 1);
		private static readonly Point VectorUpRight = new Point(0, 1);
		private static bool m_initialized;
		private static readonly MapPoint[] OrthogonalGridReference = new MapPoint[560];
		private short m_cellId;
		private int m_x;
		private int m_y;
		public short CellId
		{
			get
			{
				return this.m_cellId;
			}
			set
			{
				this.m_cellId = value;
				this.SetFromCellId();
			}
		}
		public int X
		{
			get
			{
				return this.m_x;
			}
			set
			{
				this.m_x = value;
				this.SetFromCoords();
			}
		}
		public int Y
		{
			get
			{
				return this.m_y;
			}
			set
			{
				this.m_y = value;
				this.SetFromCoords();
			}
		}
		public MapPoint(short cellId)
		{
			this.m_cellId = cellId;
			this.SetFromCellId();
		}
		public MapPoint(Cell cell)
		{
			this.m_cellId = cell.Id;
			this.SetFromCellId();
		}
		public MapPoint(int x, int y)
		{
			this.m_x = x;
			this.m_y = y;
			this.SetFromCoords();
		}
		public MapPoint(Point point)
		{
			this.m_x = point.X;
			this.m_y = point.Y;
			this.SetFromCoords();
		}
		private void SetFromCoords()
		{
			if (!MapPoint.m_initialized)
			{
				MapPoint.InitializeStaticGrid();
			}
			this.m_cellId = (short)((long)(this.m_x - this.m_y) * 14L + (long)this.m_y + (long)((this.m_x - this.m_y) / 2));
		}
		private void SetFromCellId()
		{
			if (!MapPoint.m_initialized)
			{
				MapPoint.InitializeStaticGrid();
			}
			if (this.m_cellId < 0 || (long)this.m_cellId > 560L)
			{
				throw new System.IndexOutOfRangeException("Cell identifier out of bounds (" + this.m_cellId + ").");
			}
			MapPoint mapPoint = MapPoint.OrthogonalGridReference[(int)this.m_cellId];
			this.m_x = mapPoint.X;
			this.m_y = mapPoint.Y;
		}
		public uint DistanceTo(MapPoint point)
		{
			return (uint)System.Math.Sqrt((double)((point.X - this.m_x) * (point.X - this.m_x) + (point.Y - this.m_y) * (point.Y - this.m_y)));
		}
		public uint DistanceToCell(MapPoint point)
		{
			return (uint)(System.Math.Abs(this.m_x - point.X) + System.Math.Abs(this.m_y - point.Y));
		}
		public bool IsAdjacentTo(MapPoint point)
		{
			return this.DistanceToCell(point) == 1u;
		}
		public DirectionsEnum OrientationToAdjacent(MapPoint point)
		{
			Point left = new Point
			{
				X = (point.X > this.m_x) ? 1 : ((point.X < this.m_x) ? -1 : 0),
				Y = (point.Y > this.m_y) ? 1 : ((point.Y < this.m_y) ? -1 : 0)
			};
			DirectionsEnum result;
			if (left == MapPoint.VectorRight)
			{
				result = DirectionsEnum.DIRECTION_EAST;
			}
			else
			{
				if (left == MapPoint.VectorDownRight)
				{
					result = DirectionsEnum.DIRECTION_SOUTH_EAST;
				}
				else
				{
					if (left == MapPoint.VectorDown)
					{
						result = DirectionsEnum.DIRECTION_SOUTH;
					}
					else
					{
						if (left == MapPoint.VectorDownLeft)
						{
							result = DirectionsEnum.DIRECTION_SOUTH_WEST;
						}
						else
						{
							if (left == MapPoint.VectorLeft)
							{
								result = DirectionsEnum.DIRECTION_WEST;
							}
							else
							{
								if (left == MapPoint.VectorUpLeft)
								{
									result = DirectionsEnum.DIRECTION_NORTH_WEST;
								}
								else
								{
									if (left == MapPoint.VectorUp)
									{
										result = DirectionsEnum.DIRECTION_NORTH;
									}
									else
									{
										if (left == MapPoint.VectorUpRight)
										{
											result = DirectionsEnum.DIRECTION_NORTH_EAST;
										}
										else
										{
											result = DirectionsEnum.DIRECTION_EAST;
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}
		public DirectionsEnum OrientationTo(MapPoint point, bool diagonal = true)
		{
			int num = point.X - this.m_x;
			int num2 = this.m_y - point.Y;
			double num3 = System.Math.Sqrt((double)(num * num + num2 * num2));
			double num4 = System.Math.Acos((double)num / num3);
			double num5 = num4 * 180.0 / 3.1415926535897931;
			double num6 = num5 * (double)((point.Y > this.m_y) ? -1 : 1);
			double num7 = (!diagonal) ? (System.Math.Round(num6 / 90.0) * 2.0 + 1.0) : (System.Math.Round(num6 / 45.0) + 1.0);
			if (num7 < 0.0)
			{
				num7 += 8.0;
			}
			return (DirectionsEnum)((uint)num7);
		}


        public IEnumerable<MapPoint> GetCellsInDirection(DirectionsEnum direction, short minDistance, short maxDistance)
        {
            for (short distance = minDistance; distance <= maxDistance; distance++)
            {
                MapPoint cell = GetCellInDirection(direction, distance);
                if (cell != null)
                    yield return cell;
            }
        }
        public IEnumerable<int> GetCellsInDirections(IEnumerable<DirectionsEnum> directions, short minDistance, short maxDistance)
        {
            foreach (DirectionsEnum direction in directions)
                foreach (MapPoint cell in GetCellsInDirection(direction, minDistance, maxDistance))
                    yield return cell.CellId;
        }
        public IEnumerable<int> GetAdjacentCells(bool diagonals = false)
        {
            return GetAdjacentCells(IsInMap, diagonals);
        }
        public IEnumerable<MapPoint> GetAllCellsInRange(int minRange, int maxRange, bool ignoreThis, Func<MapPoint, bool> predicate)
        {
            for (int x = X - maxRange; x <= X + maxRange; x++)
                for (int y = Y - maxRange; y <= Y + maxRange; y++)
                    if (!ignoreThis || x != X || y != Y)
                    {
                        int distance = Math.Abs(x - X) + Math.Abs(y - Y);
                        if (IsInMap(x, y) && distance <= maxRange && distance >= minRange)
                        {
                            MapPoint cell = new MapPoint(x, y);
                            if (cell != null && (predicate == null || predicate(cell))) yield return cell;
                        }
                    }
        }
        public IEnumerable<MapPoint> GetAllCellsInRectangle(MapPoint oppositeCell, bool skipStartAndEndCells = true, Func<MapPoint, bool> predicate = null)
        {
            int iteratorVariable0 = Math.Min(oppositeCell.X, this.X);
            int iteratorVariable1 = Math.Min(oppositeCell.Y, this.Y);
            int iteratorVariable2 = Math.Max(oppositeCell.X, this.X);
            int iteratorVariable3 = Math.Max(oppositeCell.Y, this.Y);
            for (int i = iteratorVariable0; i <= iteratorVariable2; i++)
            {
                for (int j = iteratorVariable1; j <= iteratorVariable3; j++)
                {
                    if (!skipStartAndEndCells || (((i != this.X) || (j != this.Y)) && ((i != oppositeCell.X) || (j != oppositeCell.Y))))
                    {
                        MapPoint point = GetPoint(i, j);
                        if ((point != null) && ((predicate == null) || predicate(point)))
                        {
                            yield return point;
                        }
                    }
                }
            }
        }

		public MapPoint[] GetCellsOnLineBetween(MapPoint destination)
		{
			System.Collections.Generic.List<MapPoint> list = new System.Collections.Generic.List<MapPoint>();
			DirectionsEnum direction = this.OrientationTo(destination, true);
			MapPoint mapPoint = this;
			int num = 0;
			while ((long)num < 140L)
			{
				mapPoint = mapPoint.GetCellInDirection(direction, 1);
				if (mapPoint == null || mapPoint.CellId == destination.CellId)
				{
					break;
				}
				list.Add(mapPoint);
				num++;
			}
			return list.ToArray();
		}
        public IEnumerable<MapPoint> GetCellsInLine(MapPoint destination)
        {
            int dx = Math.Abs((int)(destination.X - this.X));
            int dy = Math.Abs((int)(destination.Y - this.Y));
            int x = this.X;
            int y = this.Y;
            int n = (1 + dx) + dy;
            int vectorX = (destination.X > this.X) ? 1 : -1;
            int vectorY = (destination.Y > this.Y) ? 1 : -1;
            int error = dx - dy;
            dx *= 2;
            dy *= 2;
            while (n > 0)
            {
                yield return GetPoint(x, y);
                if (error <= 0)
                {
                    if (error == 0)
                    {
                        x += vectorX;
                        y += vectorY;
                        n--;
                    }
                    else
                    {
                        y += vectorY;
                        error += dx;
                    }
                }
                else
                {
                    x += vectorX;
                    error -= dy;
                }
                n--;
            }
        }

		public MapPoint GetCellInDirection(DirectionsEnum direction, short step)
		{
			MapPoint mapPoint = null;
			switch (direction)
			{
			case DirectionsEnum.DIRECTION_EAST:
				mapPoint = MapPoint.GetPoint(this.m_x + (int)step, this.m_y + (int)step);
				break;
			case DirectionsEnum.DIRECTION_SOUTH_EAST:
				mapPoint = MapPoint.GetPoint(this.m_x + (int)step, this.m_y);
				break;
			case DirectionsEnum.DIRECTION_SOUTH:
				mapPoint = MapPoint.GetPoint(this.m_x + (int)step, this.m_y - (int)step);
				break;
			case DirectionsEnum.DIRECTION_SOUTH_WEST:
				mapPoint = MapPoint.GetPoint(this.m_x, this.m_y - (int)step);
				break;
			case DirectionsEnum.DIRECTION_WEST:
				mapPoint = MapPoint.GetPoint(this.m_x - (int)step, this.m_y - (int)step);
				break;
			case DirectionsEnum.DIRECTION_NORTH_WEST:
				mapPoint = MapPoint.GetPoint(this.m_x - (int)step, this.m_y);
				break;
			case DirectionsEnum.DIRECTION_NORTH:
				mapPoint = MapPoint.GetPoint(this.m_x - (int)step, this.m_y + (int)step);
				break;
			case DirectionsEnum.DIRECTION_NORTH_EAST:
				mapPoint = MapPoint.GetPoint(this.m_x, this.m_y + (int)step);
				break;
			}
			MapPoint result;
			if (mapPoint != null)
			{
				if (MapPoint.IsInMap(mapPoint.X, mapPoint.Y))
				{
					result = mapPoint;
				}
				else
				{
					result = null;
				}
			}
			else
			{
				result = null;
			}
			return result;
		}
        public MapPoint GetNearestCellInDirection(DirectionsEnum direction)
		{
			return this.GetCellInDirection(direction, 1);
		}
        public DirectionsEnum GetOppositeDirection(DirectionsEnum direction)
        {
            return (DirectionsEnum)(((int)direction + 4) % 8);
        }
        public MapPoint GetNearestCellInOppositeDirection(DirectionsEnum direction)
        {
            return this.GetCellInDirection(GetOppositeDirection(direction), 1);
        }

        public IEnumerable<MapPoint> GetAdjacentCells(Func<short, bool> predicate, bool diagonal = false)
		{
            MapPoint northEast = new MapPoint(X, Y + 1);
            if (northEast != null && IsInMap(northEast.X, northEast.Y) && predicate(northEast.CellId))
                yield return northEast;
            MapPoint northWest = new MapPoint(X - 1, Y);
            if (northWest != null && IsInMap(northWest.X, northWest.Y) && predicate(northWest.CellId))
                yield return northWest;

            MapPoint southEast = new MapPoint(X + 1, Y);
            if (southEast != null && IsInMap(southEast.X, southEast.Y) && predicate(southEast.CellId))
                yield return southEast;

            MapPoint southWest = new MapPoint(X, Y - 1);
            if (southWest != null && IsInMap(southWest.X, southWest.Y) && predicate(southWest.CellId))
                yield return southWest;

            if (diagonal)
            {
                MapPoint north = new MapPoint(X - 1, Y + 1);
                if (north != null && IsInMap(north.X, north.Y) && predicate(north.CellId))
                    yield return north;

                MapPoint east = new MapPoint(X + 1, Y + 1);
                if (east != null && IsInMap(east.X, east.Y) && predicate(east.CellId))
                    yield return east;

                MapPoint south = new MapPoint(X + 1, Y - 1);
                if (south != null && IsInMap(south.X, south.Y) && predicate(south.CellId))
                    yield return south;

                MapPoint west = new MapPoint(X - 1, Y - 1);
                if (west != null && IsInMap(west.X, west.Y) && predicate(west.CellId))
                    yield return west;
            }
        }
        public IEnumerable<int> GetAdjacentCells(Func<MapPoint, bool> predicate, bool diagonal = false)
        {
            MapPoint northEast = new MapPoint(X, Y + 1);
            if (northEast != null && IsInMap(northEast.X, northEast.Y) && predicate(northEast))
                yield return northEast.CellId;
            MapPoint northWest = new MapPoint(X - 1, Y);
            if (northWest != null && IsInMap(northWest.X, northWest.Y) && predicate(northWest))
                yield return northWest.CellId;

            MapPoint southEast = new MapPoint(X + 1, Y);
            if (southEast != null && IsInMap(southEast.X, southEast.Y) && predicate(southEast))
                yield return southEast.CellId;

            MapPoint southWest = new MapPoint(X, Y - 1);
            if (southWest != null && IsInMap(southWest.X, southWest.Y) && predicate(southWest))
                yield return southWest.CellId;

            if (diagonal)
            {
                MapPoint north = new MapPoint(X - 1, Y + 1);
                if (north != null && IsInMap(north.X, north.Y) && predicate(north))
                    yield return north.CellId;

                MapPoint east = new MapPoint(X + 1, Y + 1);
                if (east != null && IsInMap(east.X, east.Y) && predicate(east))
                    yield return east.CellId;

                MapPoint south = new MapPoint(X + 1, Y - 1);
                if (south != null && IsInMap(south.X, south.Y) && predicate(south))
                    yield return south.CellId;

                MapPoint west = new MapPoint(X - 1, Y - 1);
                if (west != null && IsInMap(west.X, west.Y) && predicate(west))
                    yield return west.CellId;
            }
        }

        public MapPoint[] GetCellsBetween(MapPoint cell, bool includeVertex = true)
        {
            int dx = cell.X - X;
            int dy = cell.Y - Y;

            double distance = Math.Sqrt(dx * dx + dy * dy);
            double vx = dx / distance;
            double vy = dy / distance;
            int roundedDistance = (int)distance;

            var result = new MapPoint[includeVertex ? roundedDistance + 1 : roundedDistance - 1];
            int i = 0;
            if (includeVertex)
                result[i++] = this;

            double x = X + vx;
            double y = Y + vx;

            while (i < roundedDistance)
            {
                x += vx;
                y += vy;
                result[i++] = new MapPoint((int)x, (int)y);
            }

            if (includeVertex)
                result[i] = cell;

            return result;
        }
        public static bool IsInMap(int x, int y)
		{
			return x + y >= 0 && x - y >= 0 && (long)(x - y) < 40L && (long)(x + y) < 28L;
		}
        public static bool IsInMap(MapPoint cell)
        {
            return IsInMap(cell.X, cell.Y);
        }

        /// <summary>
        /// Returns the symetric cell from a cell using this point as symmetric center
        /// </summary>
        /// <param name="from">Target cell</param>
        /// <returns>The symmetric cell</returns>
        public MapPoint GetSymmetricCell(MapPoint from)
        {
            int x = this.X - (from.X - this.X);
            int y = this.Y - (from.Y - this.Y);
            return new MapPoint(x, y);
        }

		public static uint CoordToCellId(int x, int y)
		{
			if (!MapPoint.m_initialized)
			{
				MapPoint.InitializeStaticGrid();
			}
			return (uint)((long)(x - y) * 14L + (long)y + (long)((x - y) / 2));
		}
		public static Point CellIdToCoord(uint cellId)
		{
			if (!MapPoint.m_initialized)
			{
				MapPoint.InitializeStaticGrid();
			}
			MapPoint point = MapPoint.GetPoint((short)cellId);
			return new Point(point.X, point.Y);
		}
		private static void InitializeStaticGrid()
		{
			MapPoint.m_initialized = true;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			while ((long)num4 < 20L)
			{
				int num5 = 0;
				while ((long)num5 < 14L)
				{
					MapPoint.OrthogonalGridReference[num3++] = new MapPoint(num + num5, num2 + num5);
					num5++;
				}
				num++;
				num5 = 0;
				while ((long)num5 < 14L)
				{
					MapPoint.OrthogonalGridReference[num3++] = new MapPoint(num + num5, num2 + num5);
					num5++;
				}
				num2--;
				num4++;
			}
		}
		public static MapPoint GetPoint(int x, int y)
		{
			return new MapPoint(x, y);
		}
		public static MapPoint GetPoint(short cell)
		{
			return MapPoint.OrthogonalGridReference[(int)cell];
		}
		public static MapPoint GetPoint(Cell cell)
		{
			return MapPoint.GetPoint(cell.Id);
		}
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"[MapPoint(x:",
				this.m_x,
				", y:",
				this.m_y,
				", id:",
				this.m_cellId,
				")]"
			});
		}
		public override bool Equals(object obj)
		{
			return !object.ReferenceEquals(null, obj) && (object.ReferenceEquals(this, obj) || (!(obj.GetType() != typeof(MapPoint)) && this.Equals((MapPoint)obj)));
		}
		public bool Equals(MapPoint other)
		{
			return !object.ReferenceEquals(null, other) && (object.ReferenceEquals(this, other) || (other.m_cellId == this.m_cellId && other.m_x == this.m_x && other.m_y == this.m_y));
		}
		public override int GetHashCode()
		{
			int num = (int)this.m_cellId;
			num = (num * 397 ^ this.m_x);
			return num * 397 ^ this.m_y;
		}
	}
}
