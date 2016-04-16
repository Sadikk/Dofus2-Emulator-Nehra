using ProtoBuf;
using Stump.Server.BaseServer.IPC.Objects;
using System;
using System.Collections.Generic;
namespace Stump.Server.BaseServer.IPC.Messages
{
	[ProtoContract]
	public class GroupsListMessage : IPCMessage
	{
		[ProtoMember(1)]
		public IList<UserGroupData> Groups
		{
			get;
			set;
		}
		public GroupsListMessage()
		{
		}
		public GroupsListMessage(List<UserGroupData> groups)
		{
			this.Groups = groups;
		}
	}
}
