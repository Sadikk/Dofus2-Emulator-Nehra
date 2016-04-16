using ProtoBuf;
using System;
namespace Stump.Server.BaseServer.IPC.Messages
{
	[ProtoContract]
	public class AccountRequestMessage : IPCMessage
	{
		[ProtoMember(2)]
		public string Ticket
		{
			get;
			set;
		}
		[ProtoMember(3)]
		public string Nickname
		{
			get;
			set;
		}
		[ProtoMember(4)]
		public string Login
		{
			get;
			set;
		}
		[ProtoMember(5)]
		public int? Id
		{
			get;
			set;
		}
		[ProtoMember(6)]
		public int? CharacterId
		{
			get;
			set;
		}
	}
}
