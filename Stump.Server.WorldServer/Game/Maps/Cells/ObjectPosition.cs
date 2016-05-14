using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;

namespace Stump.Server.WorldServer.Game.Maps.Cells
{
	public class ObjectPosition
	{
		private DirectionsEnum m_direction;
		private Cell m_cell;
		private Map m_map;
		private MapPoint m_point;
		public event System.Action<ObjectPosition> PositionChanged;
		public bool IsValid
		{
			get
			{
				return this.m_cell.Id > 0 && (long)this.m_cell.Id < 560L && this.m_direction > DirectionsEnum.DIRECTION_EAST && this.m_direction < DirectionsEnum.DIRECTION_NORTH_EAST && this.m_map != null;
			}
		}
		public DirectionsEnum Direction
		{
			get
			{
				return this.m_direction;
			}
			set
			{
				this.m_direction = value;
				this.NotifyPositionChanged();
			}
		}
		public Cell Cell
		{
			get
			{
				return this.m_cell;
			}
			set
			{
				this.m_cell = value;
				this.m_point = null;
				this.NotifyPositionChanged();
			}
		}
		public Map Map
		{
			get
			{
				return this.m_map;
			}
			set
			{
				this.m_map = value;
				this.NotifyPositionChanged();
			}
		}
		public MapPoint Point
		{
			get
			{
				MapPoint result;
				if ((result = this.m_point) == null)
				{
                    result = (this.m_point = MapPoint.GetPoint(this.Cell));
				}
				return result;
			}
		}
		private void NotifyPositionChanged()
		{
			System.Action<ObjectPosition> positionChanged = this.PositionChanged;
			if (positionChanged != null)
			{
				positionChanged(this);
			}
		}
		public ObjectPosition(ObjectPosition position)
		{
			this.m_map = position.Map;
			this.m_cell = position.Cell;
			this.m_direction = position.Direction;
		}
		public ObjectPosition(Map map, Cell cell)
		{
			this.m_map = map;
			this.m_cell = cell;
			this.m_direction = DirectionsEnum.DIRECTION_EAST;
		}
		public ObjectPosition(Map map, short cellId)
		{
			this.m_map = map;
			this.m_cell = map.Cells[(int)cellId];
			this.m_direction = DirectionsEnum.DIRECTION_EAST;
		}
		public ObjectPosition(Map map, Cell cell, DirectionsEnum direction)
		{
			this.m_map = map;
			this.m_cell = cell;
			this.m_direction = direction;
		}
		public ObjectPosition(Map map, short cellId, DirectionsEnum direction)
		{
			this.m_map = map;
			this.m_cell = map.Cells[(int)cellId];
			this.m_direction = direction;
		}
		public ObjectPosition Clone()
		{
			return new ObjectPosition(this.Map, this.Cell, this.Direction);
		}
	}
}
