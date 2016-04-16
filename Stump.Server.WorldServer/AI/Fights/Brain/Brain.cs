using NLog;
using Stump.Core.Attributes;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.AI.Fights.Actions;
using Stump.Server.WorldServer.AI.Fights.Spells;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Maps.Pathfinding;
using Stump.Server.WorldServer.Game.Spells;
using TreeSharp;
namespace Stump.Server.WorldServer.AI.Fights.Brain
{
	public class Brain
	{
		public const int MaxMovesTries = 20;
		public const int MaxCastLimit = 20;
		[Variable(true)]
		public static bool DebugMode = false;
		protected static readonly Logger logger = LogManager.GetCurrentClassLogger();
		public AIFighter Fighter
		{
			get;
			private set;
		}
		public SpellSelector SpellSelector
		{
			get;
			private set;
		}
		public EnvironmentAnalyser Environment
		{
			get;
			private set;
		}
		public Brain(AIFighter fighter)
		{
			this.Fighter = fighter;
			this.Environment = new EnvironmentAnalyser(this.Fighter);
			this.SpellSelector = new SpellSelector(this.Fighter, this.Environment);
		}
		public virtual void Play()
		{
			this.SpellSelector.AnalysePossibilities();
			foreach (SpellCast current in this.SpellSelector.EnumerateSpellsCast())
			{
				if (current.MoveBefore != null)
				{
					this.Fighter.Fight.StartSequence(SequenceTypeEnum.SEQUENCE_MOVE);
					bool flag = this.Fighter.StartMove(current.MoveBefore);
					short id = this.Fighter.Cell.Id;
					int num = 0;
					short id2 = current.MoveBefore.EndCell.Id;
					while (flag && this.Fighter.Cell.Id != id2 && this.Fighter.CanMove() && num <= 20)
					{
						Pathfinder pathfinder = new Pathfinder(this.Environment.CellInformationProvider);
						Path path = pathfinder.FindPath(this.Fighter.Position.Cell.Id, id2, false, this.Fighter.MP);
						if (path == null || path.IsEmpty())
						{
							this.Fighter.Fight.EndSequence(SequenceTypeEnum.SEQUENCE_MOVE, false);
							break;
						}
						if (path.MPCost > this.Fighter.MP)
						{
							this.Fighter.Fight.EndSequence(SequenceTypeEnum.SEQUENCE_MOVE, false);
							break;
						}
						flag = this.Fighter.StartMove(path);
						if (this.Fighter.Cell.Id == id)
						{
							this.Fighter.Fight.EndSequence(SequenceTypeEnum.SEQUENCE_MOVE, false);
							break;
						}
						id = this.Fighter.Cell.Id;
						num++;
					}
					this.Fighter.Fight.EndSequence(SequenceTypeEnum.SEQUENCE_MOVE, false);
				}
				int num2 = 0;
				while (this.Fighter.CanCastSpell(current.Spell, current.TargetCell) == SpellCastResult.OK && num2 <= 20 && this.Fighter.CastSpell(current.Spell, current.TargetCell))
				{
					num2++;
				}
			}
			if (this.Fighter.CanMove())
			{
				foreach (RunStatus arg_26E_0 in new MoveNearTo(this.Fighter, this.Environment.GetNearestEnnemy()).Execute(this))
				{
				}
			}
		}
		public void Log(string log, params object[] args)
		{
			Brain.logger.Debug(string.Concat(new object[]
			{
				"Brain ",
				this.Fighter,
				" : ",
				log
			}), args);
			if (Brain.DebugMode)
			{
				this.Fighter.Say(string.Format(log, args));
			}
		}
	}
}
