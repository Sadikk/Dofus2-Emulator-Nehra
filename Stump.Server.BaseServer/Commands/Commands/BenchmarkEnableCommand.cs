using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Benchmark;
using System;
namespace Stump.Server.BaseServer.Commands.Commands
{
	public class BenchmarkEnableCommand : SubCommand
	{
		public BenchmarkEnableCommand()
		{
			base.Aliases = new string[]
			{
				"enable",
				"on"
			};
			base.RequiredRole = RoleEnum.Administrator;
			base.ParentCommand = typeof(BenchmarkCommands);
		}
		public override void Execute(TriggerBase trigger)
		{
			BenchmarkManager.Enable = true;
		}
	}
}
