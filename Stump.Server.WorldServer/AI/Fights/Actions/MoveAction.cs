using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Maps.Cells;
using Stump.Server.WorldServer.Game.Maps.Pathfinding;
using TreeSharp;
namespace Stump.Server.WorldServer.AI.Fights.Actions
{
	public class MoveAction : AIAction
	{
		public const int MaxMovesTries = 20;
		public Cell DestinationCell
		{
			get;
			private set;
		}
		public MapPoint Destination
		{
			get;
			private set;
		}
		public short DestinationId
		{
			get
			{
				return (this.Destination == null) ? this.DestinationCell.Id : this.Destination.CellId;
			}
		}
		public MoveAction(AIFighter fighter, Cell destinationCell) : base(fighter)
		{
			this.DestinationCell = destinationCell;
		}
		public MoveAction(AIFighter fighter, MapPoint destination) : base(fighter)
		{
			this.Destination = destination;
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
				if (this.DestinationId == base.Fighter.Cell.Id)
				{
					result = RunStatus.Success;
				}
				else
				{
					Pathfinder pathfinder = new Pathfinder(new AIFightCellsInformationProvider(base.Fighter.Fight, base.Fighter));
					Path path = pathfinder.FindPath(base.Fighter.Position.Cell.Id, this.DestinationId, false, base.Fighter.MP);
					if (path == null || path.IsEmpty())
					{
						result = RunStatus.Failure;
					}
					else
					{
						if (path.MPCost > base.Fighter.MP)
						{
							result = RunStatus.Failure;
						}
						else
						{
							base.Fighter.Fight.StartSequence(SequenceTypeEnum.SEQUENCE_MOVE);
							bool flag = base.Fighter.StartMove(path);
							short id = base.Fighter.Cell.Id;
							int num = 0;
							while (flag && base.Fighter.Cell.Id != this.DestinationId && base.Fighter.CanMove() && num <= 20)
							{
								path = pathfinder.FindPath(base.Fighter.Position.Cell.Id, this.DestinationId, false, base.Fighter.MP);
								if (path == null || path.IsEmpty())
								{
									base.Fighter.Fight.EndSequence(SequenceTypeEnum.SEQUENCE_MOVE, false);
									result = RunStatus.Failure;
									return result;
								}
								if (path.MPCost > base.Fighter.MP)
								{
									base.Fighter.Fight.EndSequence(SequenceTypeEnum.SEQUENCE_MOVE, false);
									result = RunStatus.Failure;
									return result;
								}
								flag = base.Fighter.StartMove(path);
								if (base.Fighter.Cell.Id == id)
								{
									base.Fighter.Fight.EndSequence(SequenceTypeEnum.SEQUENCE_MOVE, false);
									result = RunStatus.Failure;
									return result;
								}
								id = base.Fighter.Cell.Id;
								num++;
							}
							base.Fighter.Fight.EndSequence(SequenceTypeEnum.SEQUENCE_MOVE, false);
							result = (flag ? RunStatus.Success : RunStatus.Failure);
						}
					}
				}
			}
			return result;
		}
	}
}
