using System;
namespace Stump.Server.BaseServer.IPC
{
	public class IPCErrorTimeoutMessage : IPCErrorMessage
	{
		public IPCErrorTimeoutMessage()
		{
		}
		public IPCErrorTimeoutMessage(string message) : base(message)
		{
		}
		public IPCErrorTimeoutMessage(string message, string stackTrace) : base(message, stackTrace)
		{
		}
	}
}
