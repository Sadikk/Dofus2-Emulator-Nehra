using Stump.DofusProtocol.Enums;
using System;
namespace Stump.Server.BaseServer.Commands.Commands
{
	public class DebugCommand : SubCommandContainer
	{
		public DebugCommand()
		{
			base.Aliases = new string[]
			{
				"debug"
			};
			base.RequiredRole = RoleEnum.Administrator;
			base.Description = "Provides command to debug things";
		}
	}
}
