using ProtoBuf;
using System;
namespace Stump.Server.BaseServer.IPC.Messages
{
	[ProtoContract]
	public class UnBanAccountMessage : IPCMessage
	{
		[ProtoMember(2)]
		public int? AccountId
		{
			get;
			set;
		}
		[ProtoMember(3)]
		public string AccountName
		{
			get;
			set;
		}
		public UnBanAccountMessage()
		{
		}
		public UnBanAccountMessage(int? accountId)
		{
			this.AccountId = accountId;
		}
		public UnBanAccountMessage(string accountName)
		{
			this.AccountName = accountName;
		}
	}
}
