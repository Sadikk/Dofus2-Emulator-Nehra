using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.WorldServer.Commands.Commands.Patterns;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class InvisibleCommand : TargetCommand
	{
		public InvisibleCommand()
		{
			base.Aliases = new string[]
			{
				"invisible",
				"setinv"
			};
			base.RequiredRole = RoleEnum.Administrator;
			base.Description = "Toggle invisible state";
			base.AddTargetParameter(true, "Defined target");
		}
		public override void Execute(TriggerBase trigger)
		{
			Character[] targets = base.GetTargets(trigger);
			for (int i = 0; i < targets.Length; i++)
			{
				Character character = targets[i];
				trigger.Reply(character.ToggleInvisibility() ? "{0} is now invisible" : "{0} is now visible", new object[]
				{
					character
				});
			}
		}
	}
}
