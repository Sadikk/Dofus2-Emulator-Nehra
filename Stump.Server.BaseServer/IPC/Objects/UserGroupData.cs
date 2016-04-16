using ProtoBuf;
using Stump.DofusProtocol.Enums;
using System;
using System.Collections.Generic;
namespace Stump.Server.BaseServer.IPC.Objects
{
	[ProtoContract]
	public class UserGroupData
	{
		[ProtoMember(1)]
		public int Id
		{
			get;
			set;
		}
		[ProtoMember(2)]
		public string Name
		{
			get;
			set;
		}
		[ProtoMember(3)]
		public RoleEnum Role
		{
			get;
			set;
		}
		[ProtoMember(4)]
		public bool IsGameMaster
		{
			get;
			set;
		}
		[ProtoMember(5)]
		public IList<int> Servers
		{
			get;
			set;
		}
	}
}
