using ProtoBuf;
using System;
namespace Stump.Server.BaseServer.IPC
{
	public class IPCErrorMessage : IPCMessage
	{
		[ProtoMember(2)]
		public string Message
		{
			get;
			set;
		}
		[ProtoMember(3)]
		public string StackTrace
		{
			get;
			set;
		}
		public IPCErrorMessage()
		{
		}
		public IPCErrorMessage(string message)
		{
			this.Message = message;
		}
		public IPCErrorMessage(string message, string stackTrace)
		{
			this.Message = message;
			this.StackTrace = stackTrace;
		}
	}
}
