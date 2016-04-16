using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class GodCommand : SubCommandContainer
	{
		public GodCommand()
		{
			base.Aliases = new string[]
			{
				"god"
			};
			base.RequiredRole = RoleEnum.GameMaster;
			base.Description = "Just to be all powerful.";
		}
	}
}
