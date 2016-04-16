using ProtoBuf;
using System;
namespace Stump.Server.BaseServer.IPC.Objects
{
	[ProtoContract]
	public class WorldCharacterData
	{
		[ProtoMember(1)]
		public int CharacterId
		{
			get;
			set;
		}
		[ProtoMember(2)]
		public int WorldId
		{
			get;
			set;
		}
		public WorldCharacterData()
		{
		}
		public WorldCharacterData(int characterId, int worldId)
		{
			this.CharacterId = characterId;
			this.WorldId = worldId;
		}
	}
}
