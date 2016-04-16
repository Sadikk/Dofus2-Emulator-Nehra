using ProtoBuf;
using Stump.DofusProtocol.Enums;
using System;
namespace Stump.Server.BaseServer.IPC.Messages
{
	[ProtoContract]
	public class ChangeStateMessage : IPCMessage
	{
		[ProtoMember(2)]
		public ServerStatusEnum State
		{
			get;
			set;
		}
	}
}
