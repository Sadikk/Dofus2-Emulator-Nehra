using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class AlignmentCommands : SubCommandContainer
	{
		public AlignmentCommands()
		{
			base.Aliases = new string[]
			{
				"alignment",
				"align"
			};
			base.RequiredRole = RoleEnum.Moderator;
			base.Description = "Provides many commands to manage player alignment";
		}
	}
}
