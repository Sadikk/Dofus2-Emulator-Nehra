using ProtoBuf;
using System;
namespace Stump.Server.BaseServer.IPC.Messages
{
	[ProtoContract]
	public class DisconnectClientMessage : IPCMessage
	{
		[ProtoMember(2)]
		public int AccountId
		{
			get;
			set;
		}
		public DisconnectClientMessage()
		{
		}
		public DisconnectClientMessage(int id)
		{
			this.AccountId = id;
		}
	}
}
