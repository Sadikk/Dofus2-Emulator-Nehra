using ProtoBuf;
using System;
namespace Stump.Server.BaseServer.IPC.Messages
{
	[ProtoContract]
	public class UnBanIPMessage : IPCMessage
	{
		[ProtoMember(2)]
		public string IPRange
		{
			get;
			set;
		}
		public UnBanIPMessage()
		{
		}
		public UnBanIPMessage(string ipRange)
		{
			this.IPRange = ipRange;
		}
	}
}
