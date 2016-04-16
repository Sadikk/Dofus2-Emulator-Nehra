using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.WorldServer.Commands.Trigger;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class SaveCommand : SubCommandContainer
	{
		public SaveCommand()
		{
			base.Aliases = new string[]
			{
				"save"
			};
			base.Description = "Save the player";
			base.RequiredRole = RoleEnum.GameMaster;
		}
		public override void Execute(TriggerBase trigger)
		{
			string value = trigger.Args.PeekNextWord();
			if (string.IsNullOrEmpty(value))
			{
				if (trigger is GameTrigger)
				{
					(trigger as GameTrigger).Character.SaveLater();
					trigger.Reply("Player saved");
				}
			}
			else
			{
				base.Execute(trigger);
			}
		}
	}
}
