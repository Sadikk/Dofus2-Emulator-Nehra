using Stump.DofusProtocol.Enums;
using System;
namespace Stump.Server.BaseServer.Commands.Commands
{
	public class BenchmarkCommands : SubCommandContainer
	{
		public BenchmarkCommands()
		{
			base.Aliases = new string[]
			{
				"benchmark",
				"bench"
			};
			base.RequiredRole = RoleEnum.Administrator;
		}
	}
}
