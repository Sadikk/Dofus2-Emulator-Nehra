using Stump.DofusProtocol.Enums;
using System;
namespace Stump.Server.BaseServer.Commands.Commands
{
	public class ConfigSaveCommand : SubCommand
	{
		public ConfigSaveCommand()
		{
			base.ParentCommand = typeof(ConfigCommand);
			base.Aliases = new string[]
			{
				"save"
			};
			base.RequiredRole = RoleEnum.Administrator;
			base.Description = "Save the config file";
		}
		public override void Execute(TriggerBase trigger)
		{
			ServerBase.InstanceAsBase.Config.Save();
			trigger.Reply("Config saved");
		}
	}
}
