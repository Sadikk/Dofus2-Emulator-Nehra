using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class NpcsCommands : SubCommandContainer
	{
		public NpcsCommands()
		{
			base.Aliases = new string[]
			{
				"npcs"
			};
			base.RequiredRole = RoleEnum.GameMaster;
			base.Description = "Manage npcs";
		}
	}
}
