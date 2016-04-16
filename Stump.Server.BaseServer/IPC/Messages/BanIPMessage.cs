using ProtoBuf;
using System;
namespace Stump.Server.BaseServer.IPC.Messages
{
	[ProtoContract]
	public class BanIPMessage : IPCMessage
	{
		[ProtoMember(2)]
		public string IPRange
		{
			get;
			set;
		}
		[ProtoMember(3)]
		public DateTime? BanEndDate
		{
			get;
			set;
		}
		[ProtoMember(4)]
		public string BanReason
		{
			get;
			set;
		}
		[ProtoMember(5)]
		public int? BannerAccountId
		{
			get;
			set;
		}
	}
}
