using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Maps.Cells;
using TreeSharp;
namespace Stump.Server.WorldServer.AI.Fights.Actions
{
	public class MoveFarFrom : AIAction
	{
		public FightActor From
		{
			get;
			private set;
		}
		public MoveFarFrom(AIFighter fighter, FightActor from) : base(fighter)
		{
			this.From = from;
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
				if (this.From == null)
				{
					result = RunStatus.Failure;
				}
				else
				{
					DirectionsEnum direction = this.From.Position.Point.OrientationTo(base.Fighter.Position.Point, true);
					MapPoint cellInDirection = base.Fighter.Position.Point.GetCellInDirection(direction, (short)base.Fighter.MP);
					if (cellInDirection == null)
					{
						result = RunStatus.Failure;
					}
					else
					{
						MoveAction moveAction = new MoveAction(base.Fighter, cellInDirection);
						result = moveAction.YieldExecute(context);
					}
				}
			}
			return result;
		}
	}
}
