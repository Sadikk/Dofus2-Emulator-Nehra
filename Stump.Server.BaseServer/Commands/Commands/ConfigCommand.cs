using Stump.DofusProtocol.Enums;
using System;
namespace Stump.Server.BaseServer.Commands.Commands
{
	public class ConfigCommand : SubCommandContainer
	{
		public ConfigCommand()
		{
			base.Aliases = new string[]
			{
				"config"
			};
			base.Description = "Provide commands to manage the config file";
			base.RequiredRole = RoleEnum.Administrator;
		}
	}
}
