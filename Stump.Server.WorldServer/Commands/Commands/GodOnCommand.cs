using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.WorldServer.Commands.Commands.Patterns;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class GodOnCommand : TargetSubCommand
	{
		public GodOnCommand()
		{
			base.Aliases = new string[]
			{
				"on"
			};

			base.RequiredRole = RoleEnum.GameMaster;
			base.ParentCommand = typeof(GodCommand);
			base.Description = "Active le mode dieux";
			base.AddTargetParameter(true, "Defined target");
		}

		public override void Execute(TriggerBase trigger)
		{
			Character[] targets = base.GetTargets(trigger);
			for (int i = 0; i < targets.Length; i++)
			{
				Character character = targets[i];
				character.ToggleGodMode(true);
				trigger.Reply("You're now god");
			}
		}
	}
}
