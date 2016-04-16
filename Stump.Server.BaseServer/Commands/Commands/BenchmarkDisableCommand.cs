using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Benchmark;
using System;
namespace Stump.Server.BaseServer.Commands.Commands
{
	public class BenchmarkDisableCommand : SubCommand
	{
		public BenchmarkDisableCommand()
		{
			base.Aliases = new string[]
			{
				"disable",
				"off"
			};
			base.RequiredRole = RoleEnum.Administrator;
			base.ParentCommand = typeof(BenchmarkCommands);
		}
		public override void Execute(TriggerBase trigger)
		{
			BenchmarkManager.Enable = false;
		}
	}
}
