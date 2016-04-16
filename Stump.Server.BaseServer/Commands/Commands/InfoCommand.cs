using Stump.DofusProtocol.Enums;
using System;
namespace Stump.Server.BaseServer.Commands.Commands
{
	public class InfoCommand : SubCommandContainer
	{
		public InfoCommand()
		{
			base.Aliases = new string[]
			{
				"info"
			};
			base.RequiredRole = RoleEnum.Moderator;
			base.Description = "Display some informations";
		}
		public override void Execute(TriggerBase trigger)
		{
			if (trigger.Args.HasNext)
			{
				base.Execute(trigger);
			}
			else
			{
				trigger.Reply("Uptime : " + trigger.Bold("{0}") + " Players : " + trigger.Bold("{1}"), new object[]
				{
					ServerBase.InstanceAsBase.UpTime.ToString("hh\\:mm\\:ss"),
					ServerBase.InstanceAsBase.ClientManager.Count
				});
			}
		}
	}
}
