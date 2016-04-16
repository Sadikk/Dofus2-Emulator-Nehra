using ProtoBuf;
using Stump.DofusProtocol.Enums;
using System;
namespace Stump.Server.BaseServer.IPC.Objects
{
	[ProtoContract]
	public class WorldServerData
	{
		[ProtoMember(1)]
		public int Id
		{
			get;
			set;
		}
		[ProtoMember(2)]
		public string Address
		{
			get;
			set;
		}
		[ProtoMember(3)]
		public ushort Port
		{
			get;
			set;
		}
		[ProtoMember(4)]
		public string Name
		{
			get;
			set;
		}
		[ProtoMember(5)]
		public int Capacity
		{
			get;
			set;
		}
		[ProtoMember(6)]
		public RoleEnum RequiredRole
		{
			get;
			set;
		}
		[ProtoMember(7)]
		public bool RequireSubscription
		{
			get;
			set;
		}
		public string AddressString
		{
			get
			{
				return this.Address + ":" + this.Port;
			}
		}
	}
}
