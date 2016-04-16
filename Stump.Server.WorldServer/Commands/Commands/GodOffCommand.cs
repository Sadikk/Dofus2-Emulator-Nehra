using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.WorldServer.Commands.Commands.Patterns;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class GodOffCommand : TargetSubCommand
	{
		public GodOffCommand()
		{
			base.Aliases = new string[]
			{
				"off"
			};
			base.RequiredRole = RoleEnum.Administrator;
			base.ParentCommand = typeof(GodCommand);
			base.Description = "Disable god mode";
			base.AddTargetParameter(true, "Defined target");
		}
		public override void Execute(TriggerBase trigger)
		{
			Character[] targets = base.GetTargets(trigger);
			for (int i = 0; i < targets.Length; i++)
			{
				Character character = targets[i];
				character.ToggleGodMode(false);
				trigger.Reply("You're not god anymore");
			}
		}
	}
}
