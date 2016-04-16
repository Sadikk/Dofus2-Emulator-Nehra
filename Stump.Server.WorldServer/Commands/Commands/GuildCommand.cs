using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;

namespace Stump.Server.WorldServer.Commands.Commands
{
	internal class GuildCommand : SubCommandContainer
	{
		public GuildCommand()
		{
			base.Aliases = new string[]
			{
				"guild"
			};
			base.RequiredRole = RoleEnum.GameMaster;
			base.Description = "Provides many commands to manage guilds";
		}
	}
}
