using ProtoBuf;
using Stump.Server.BaseServer.IPC.Objects;
using System;
namespace Stump.Server.BaseServer.IPC.Messages
{
	[ProtoContract]
	public class AccountAnswerMessage : IPCMessage
	{
		[ProtoMember(2)]
		public AccountData Account
		{
			get;
			set;
		}
		public AccountAnswerMessage()
		{
		}
		public AccountAnswerMessage(AccountData account)
		{
			this.Account = account;
		}
	}
}
