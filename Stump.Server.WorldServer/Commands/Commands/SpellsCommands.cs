using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class SpellsCommands : SubCommandContainer
	{
		public SpellsCommands()
		{
			base.Aliases = new string[]
			{
				"spell"
			};
			base.RequiredRole = RoleEnum.GameMaster;
			base.Description = "Manage spells";
		}
	}
}
