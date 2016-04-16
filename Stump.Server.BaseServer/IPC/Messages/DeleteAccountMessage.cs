using ProtoBuf;
using System;
namespace Stump.Server.BaseServer.IPC.Messages
{
	[ProtoContract]
	public class DeleteAccountMessage : IPCMessage
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
	}
}
