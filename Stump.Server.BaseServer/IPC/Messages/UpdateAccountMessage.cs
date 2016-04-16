using ProtoBuf;
using Stump.Server.BaseServer.IPC.Objects;
using System;
namespace Stump.Server.BaseServer.IPC.Messages
{
	[ProtoContract]
	public class UpdateAccountMessage : IPCMessage
	{
		[ProtoMember(2)]
		public AccountData Account
		{
			get;
			set;
		}
		public UpdateAccountMessage()
		{
		}
		public UpdateAccountMessage(AccountData account)
		{
			this.Account = account;
		}
	}
}
