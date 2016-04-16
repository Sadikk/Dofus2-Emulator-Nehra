using Stump.Core.Timers;
using System;
namespace Stump.Server.BaseServer.IPC
{
	public interface IIPCRequest
	{
		Guid Guid
		{
			get;
			set;
		}
		TimerEntry TimeoutTimer
		{
			get;
			set;
		}
		IPCMessage RequestMessage
		{
			get;
			set;
		}
		bool ProcessMessage(IPCMessage message);
	}
}
