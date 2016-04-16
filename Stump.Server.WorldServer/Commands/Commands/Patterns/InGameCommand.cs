using Stump.Server.BaseServer.Commands;
using Stump.Server.WorldServer.Commands.Trigger;

namespace Stump.Server.WorldServer.Commands.Commands.Patterns
{
	public abstract class InGameCommand : CommandBase
	{
		public override void Execute(TriggerBase trigger)
		{
			if (!(trigger is GameTrigger))
			{
				trigger.ReplyError("This command can only be executed in game");
			}
			else
			{
				this.Execute(trigger as GameTrigger);
			}
		}
		public abstract void Execute(GameTrigger trigger);
	}
}
