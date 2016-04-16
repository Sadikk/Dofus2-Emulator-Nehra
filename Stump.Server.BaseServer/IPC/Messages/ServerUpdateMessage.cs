using ProtoBuf;
using System;
namespace Stump.Server.BaseServer.IPC.Messages
{
	[ProtoContract]
	public class ServerUpdateMessage : IPCMessage
	{
		[ProtoMember(2)]
		public int CharsCount
		{
			get;
			set;
		}
		public ServerUpdateMessage()
		{
		}
		public ServerUpdateMessage(int charsCount)
		{
			this.CharsCount = charsCount;
		}
	}
}
