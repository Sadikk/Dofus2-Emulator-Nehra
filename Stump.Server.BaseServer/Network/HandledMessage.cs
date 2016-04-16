using NLog;
using Stump.Core.Reflection;
using Stump.Core.Threading;
using Stump.DofusProtocol.Messages;
using Stump.Server.BaseServer.Benchmark;
using Stump.Server.BaseServer.Exceptions;
using System;
using System.Diagnostics;
namespace Stump.Server.BaseServer.Network
{
	public class HandledMessage<T> : Message3<object, T, Stump.DofusProtocol.Messages.Message> where T : BaseClient
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		public HandledMessage(Action<object, T, Stump.DofusProtocol.Messages.Message> callback, T client, Stump.DofusProtocol.Messages.Message message) : base(null, client, message, callback)
		{
		}
		public override void Execute()
		{
			try
			{
				Stopwatch stopwatch = Stopwatch.StartNew();
				base.Execute();
                base.Parameter2.LastMessage = base.Parameter3;
				stopwatch.Stop();
				if (BenchmarkManager.Enable)
				{
					Singleton<BenchmarkManager>.Instance.RegisterEntry(BenchmarkEntry.Create(stopwatch.Elapsed, base.Parameter3));
				}
			}
			catch (Exception ex)
			{
				HandledMessage<T>.logger.Error<Stump.DofusProtocol.Messages.Message, T, Exception>("[Handler : {0}] Force disconnection of client {1} : {2}", base.Parameter3, base.Parameter2, ex);
				T parameter = base.Parameter2;
				parameter.Disconnect();
				Singleton<ExceptionManager>.Instance.RegisterException(ex);
			}
		}
	}
}
