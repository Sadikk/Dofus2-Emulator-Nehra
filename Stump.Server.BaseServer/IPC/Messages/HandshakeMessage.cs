using ProtoBuf;
using Stump.Server.BaseServer.IPC.Objects;
using System;
namespace Stump.Server.BaseServer.IPC.Messages
{
	[ProtoContract]
	public class HandshakeMessage : IPCMessage
	{
		[ProtoMember(2)]
		public WorldServerData World
		{
			get;
			set;
		}
		public HandshakeMessage()
		{
		}
		public HandshakeMessage(WorldServerData world)
		{
			this.World = world;
		}
	}
}
