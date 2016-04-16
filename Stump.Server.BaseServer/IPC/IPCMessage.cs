using ProtoBuf;
using System;
namespace Stump.Server.BaseServer.IPC
{
	[ProtoContract]
	public abstract class IPCMessage
	{
		[ProtoMember(1)]
		public Guid RequestGuid
		{
			get;
			set;
		}
	}
}
