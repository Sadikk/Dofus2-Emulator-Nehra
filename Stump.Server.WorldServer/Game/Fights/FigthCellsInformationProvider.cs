using Stump.Server.WorldServer.Game.Maps;
using Stump.Server.WorldServer.Game.Maps.Pathfinding;

namespace Stump.Server.WorldServer.Game.Fights
{
	public class FigthCellsInformationProvider : ICellsInformationProvider
	{
		public Fight Fight
		{
			get;
			private set;
		}
		public Map Map
		{
			get
			{
				return this.Fight.Map;
			}
		}
		public FigthCellsInformationProvider(Fight fight)
		{
			this.Fight = fight;
		}
		public bool IsCellWalkable(short cell)
		{
			return this.Fight.IsCellFree(this.Fight.Map.Cells[(int)cell]);
		}
		public virtual CellInformation GetCellInformation(short cell)
		{
			return new CellInformation(this.Fight.Map.Cells[(int)cell], this.IsCellWalkable(cell), true);
		}
	}
}
