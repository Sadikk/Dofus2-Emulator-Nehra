using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.WorldServer.Commands.Commands.Patterns;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class UnMuteCommand : TargetCommand
	{
		public UnMuteCommand()
		{
			base.Aliases = new string[]
			{
				"unmute"
			};
			base.RequiredRole = RoleEnum.Moderator;
			base.AddTargetParameter(false, "Defined target");
		}
		public override void Execute(TriggerBase trigger)
		{
			Character[] targets = base.GetTargets(trigger);
			for (int i = 0; i < targets.Length; i++)
			{
				Character character = targets[i];
				character.UnMute();
				trigger.Reply("{0} unmuted", new object[]
				{
					character.Name
				});
			}
		}
	}
}
