using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Commands.Commands.Patterns;
using Stump.Server.WorldServer.Commands.Trigger;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class AdminChatCommand : InGameCommand
	{
		public AdminChatCommand()
		{
			base.Aliases = new string[]
			{
				"admin"
			};
			base.RequiredRole = RoleEnum.Moderator;
			base.Description = "Enable/disable admin chat mode";
		}
		public override void Execute(GameTrigger trigger)
		{
			trigger.Reply("Admin chat mode is : {0}", new object[]
			{
				trigger.Bold((trigger.Character.AdminMessagesEnabled = !trigger.Character.AdminMessagesEnabled) ? "Enabled" : "Disabled")
			});
		}
	}
}
