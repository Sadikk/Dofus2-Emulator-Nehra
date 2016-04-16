using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.WorldServer.Commands.Commands.Patterns;
using Stump.Server.WorldServer.Commands.Trigger;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class KickCommand : TargetCommand
	{
		public KickCommand()
		{
			base.Aliases = new string[]
			{
				"kick"
			};
			base.RequiredRole = RoleEnum.Administrator;
			base.Description = "Kick a player";
			base.AddTargetParameter(false, "Defined target");
		}
		public override void Execute(TriggerBase trigger)
		{
			Character[] targets = base.GetTargets(trigger);
			for (int i = 0; i < targets.Length; i++)
			{
				Character character = targets[i];
				string text = (trigger is GameTrigger) ? (trigger as GameTrigger).Character.Name : "Server";
				character.SendSystemMessage(18, true, new object[]
				{
					text,
					string.Empty
				});
				character.Client.Disconnect();
				trigger.Reply("You have kicked {0}", new object[]
				{
					character.Name
				});
			}
		}
	}
}
