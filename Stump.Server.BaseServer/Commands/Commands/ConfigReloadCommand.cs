using Stump.DofusProtocol.Enums;
using System;
namespace Stump.Server.BaseServer.Commands.Commands
{
	public class ConfigReloadCommand : SubCommand
	{
		public ConfigReloadCommand()
		{
			base.ParentCommand = typeof(ConfigCommand);
			base.Aliases = new string[]
			{
				"reload"
			};
			base.RequiredRole = RoleEnum.Administrator;
			base.Description = "Reload the config file";
		}
		public override void Execute(TriggerBase trigger)
		{
			ServerBase.InstanceAsBase.Config.Reload();
			trigger.Reply("Config reloaded");
		}
	}
}
