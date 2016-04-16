using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class MonsterCommands : SubCommandContainer
	{
		public MonsterCommands()
		{
			base.Aliases = new string[]
			{
				"monster"
			};
			base.RequiredRole = RoleEnum.GameMaster;
			base.Description = "Manage monsters";
		}
	}
}
