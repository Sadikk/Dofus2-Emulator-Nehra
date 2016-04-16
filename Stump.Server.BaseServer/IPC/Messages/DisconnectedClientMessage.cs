using ProtoBuf;
using System;
namespace Stump.Server.BaseServer.IPC.Messages
{
	[ProtoContract]
	public class DisconnectedClientMessage : IPCMessage
	{
		[ProtoMember(2)]
		public bool Disconnected
		{
			get;
			set;
		}
		public DisconnectedClientMessage()
		{
		}
		public DisconnectedClientMessage(bool disconnected)
		{
			this.Disconnected = disconnected;
		}
	}
}
