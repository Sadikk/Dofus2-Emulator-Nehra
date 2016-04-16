using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Maps.Cells.Shapes;
using System.Linq;
using TreeSharp;
namespace Stump.Server.WorldServer.AI.Fights.Actions
{
	public class RandomMove : AIAction
	{
		public RandomMove(AIFighter fighter) : base(fighter)
		{
		}
		protected override RunStatus Run(object context)
		{
			RunStatus result;
			if (!base.Fighter.CanMove())
			{
				result = RunStatus.Failure;
			}
			else
			{
				Lozenge lozenge = new Lozenge(0, (byte)base.Fighter.MP);
				Cell[] array = (
					from entry in lozenge.GetCells(base.Fighter.Cell, base.Fighter.Fight.Map)
					where base.Fighter.Fight.IsCellFree(entry)
					select entry).ToArray<Cell>();
				if (array.Length == 0)
				{
					result = RunStatus.Failure;
				}
				else
				{
					System.Random random = new System.Random();
					Cell destinationCell = array[random.Next(array.Length)];
					MoveAction moveAction = new MoveAction(base.Fighter, destinationCell);
					result = moveAction.YieldExecute(context);
				}
			}
			return result;
		}
	}
}
