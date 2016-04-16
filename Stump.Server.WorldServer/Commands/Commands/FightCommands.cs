using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class FightCommands : SubCommandContainer
	{
		public FightCommands()
		{
			base.Aliases = new string[]
			{
				"fight"
			};
			base.Description = "Provides commands to manage fights";
			base.RequiredRole = RoleEnum.GameMaster;
		}
	}
}
