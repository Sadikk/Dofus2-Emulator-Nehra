using ProtoBuf;
using System;
namespace Stump.Server.BaseServer.IPC.Messages
{
	[ProtoContract]
	public class BanAccountMessage : IPCMessage
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
		[ProtoMember(4)]
		public DateTime? BanEndDate
		{
			get;
			set;
		}
		[ProtoMember(5)]
		public string BanReason
		{
			get;
			set;
		}
		[ProtoMember(6)]
		public int? BannerAccountId
		{
			get;
			set;
		}
		[ProtoMember(7)]
		public bool Jailed
		{
			get;
			set;
		}
	}
}
