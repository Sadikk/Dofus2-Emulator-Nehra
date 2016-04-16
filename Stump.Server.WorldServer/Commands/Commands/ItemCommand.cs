using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class ItemCommand : SubCommandContainer
	{
		public ItemCommand()
		{
			base.Aliases = new string[]
			{
				"item"
			};
			base.RequiredRole = RoleEnum.Moderator;
			base.Description = "Provides many commands to manage items";
		}
	}
}
