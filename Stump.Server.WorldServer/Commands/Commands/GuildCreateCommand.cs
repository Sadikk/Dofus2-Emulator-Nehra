using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Commands.Commands.Patterns;
using Stump.Server.WorldServer.Commands.Trigger;
using Stump.Server.WorldServer.Game.Dialogs.Guilds;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class GuildCreateCommand : InGameSubCommand
	{
		public GuildCreateCommand()
		{
			base.Aliases = new string[]
			{
				"create"
			};
			base.RequiredRole = RoleEnum.Administrator;
			base.ParentCommand = typeof(GuildCommand);
		}
		public override void Execute(GameTrigger trigger)
		{
			GuildCreationPanel guildCreationPanel = new GuildCreationPanel(trigger.Character);
			guildCreationPanel.Open();
		}
	}
}
