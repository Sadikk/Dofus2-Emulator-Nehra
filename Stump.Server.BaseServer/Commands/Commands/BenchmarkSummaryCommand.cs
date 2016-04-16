using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Benchmark;
using System;
namespace Stump.Server.BaseServer.Commands.Commands
{
	public class BenchmarkSummaryCommand : SubCommand
	{
		public BenchmarkSummaryCommand()
		{
			base.Aliases = new string[]
			{
				"summary",
				"sum"
			};
			base.RequiredRole = RoleEnum.Administrator;
			base.ParentCommand = typeof(BenchmarkCommands);
		}
		public override void Execute(TriggerBase trigger)
		{
			trigger.Reply(Singleton<BenchmarkManager>.Instance.GenerateReport());
		}
	}
}
