using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using TreeSharp;
namespace Stump.Server.WorldServer.AI.Fights.Actions
{
	public class FleeAction : AIAction
	{
		public FleeAction(AIFighter fighter) : base(fighter)
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
				Cell cellToFlee = base.Fighter.Brain.Environment.GetCellToFlee();
				if (cellToFlee == null)
				{
					result = RunStatus.Failure;
				}
				else
				{
					if (cellToFlee.Id == base.Fighter.Cell.Id)
					{
						result = RunStatus.Success;
					}
					else
					{
						MoveAction moveAction = new MoveAction(base.Fighter, cellToFlee);
						result = moveAction.YieldExecute(context);
					}
				}
			}
			return result;
		}
	}
}
