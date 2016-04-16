using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Fights;
using Stump.Server.WorldServer.Game.Maps.Pathfinding;

namespace Stump.Server.WorldServer.AI.Fights.Actions
{
	public class AIFightCellsInformationProvider : FigthCellsInformationProvider
	{
		public AIFighter Fighter
		{
			get;
			private set;
		}
		public AIFightCellsInformationProvider(Fight fight, AIFighter fighter) : base(fight)
		{
			this.Fighter = fighter;
		}
		public override CellInformation GetCellInformation(short cell)
		{
			return new CellInformation(base.Fight.Map.Cells[(int)cell], base.IsCellWalkable(cell), true, true, 1, null, null);
		}
	}
}
