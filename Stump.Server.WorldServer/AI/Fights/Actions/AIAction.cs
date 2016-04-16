using Stump.Server.WorldServer.Game.Actors.Fight;
using TreeSharp;
namespace Stump.Server.WorldServer.AI.Fights.Actions
{
	public abstract class AIAction : TreeSharp.Action
	{
		public AIFighter Fighter
		{
			get;
			private set;
		}
		protected AIAction(AIFighter fighter)
		{
			this.Fighter = fighter;
		}
		public RunStatus YieldExecute(object context)
		{
			RunStatus result;
			if (this.Fighter.IsDead())
			{
				result = RunStatus.Failure;
			}
			else
			{
				result = this.Run(context);
			}
			return result;
		}
		protected override RunStatus Run(object context)
		{
			return RunStatus.Failure;
		}
	}
}
