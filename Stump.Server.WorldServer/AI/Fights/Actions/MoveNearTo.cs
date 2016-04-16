using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Maps.Cells;
using System;
using System.Linq;
using TreeSharp;
namespace Stump.Server.WorldServer.AI.Fights.Actions
{
	public class MoveNearTo : AIAction
	{
		public FightActor Target
		{
			get;
			private set;
		}
		public MoveNearTo(AIFighter fighter, FightActor target) : base(fighter)
		{
			this.Target = target;
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
				if (this.Target == null)
				{
					result = RunStatus.Failure;
				}
				else
				{
					AIFightCellsInformationProvider @object = new AIFightCellsInformationProvider(base.Fighter.Fight, base.Fighter);
					if (base.Fighter.Position.Point.IsAdjacentTo(this.Target.Position.Point))
					{
						result = RunStatus.Success;
					}
					else
					{
						MapPoint mapPoint = (
							from entry in this.Target.Position.Point.GetAdjacentCells(new Func<short, bool>(@object.IsCellWalkable))
							orderby entry.DistanceToCell(base.Fighter.Position.Point)
							select entry).FirstOrDefault<MapPoint>();
						if (mapPoint == null)
						{
							result = RunStatus.Failure;
						}
						else
						{
							MoveAction moveAction = new MoveAction(base.Fighter, mapPoint);
							result = moveAction.YieldExecute(context);
						}
					}
				}
			}
			return result;
		}
	}
}
