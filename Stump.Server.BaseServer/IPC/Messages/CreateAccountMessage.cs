using ProtoBuf;
using Stump.Server.BaseServer.IPC.Objects;
using System;
namespace Stump.Server.BaseServer.IPC.Messages
{
	[ProtoContract]
	public class CreateAccountMessage : IPCMessage
	{
		[ProtoMember(2)]
		public AccountData Account
		{
			get;
			set;
		}
	}
}
