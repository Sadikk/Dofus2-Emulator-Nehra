using ProtoBuf;
using System;
namespace Stump.Server.BaseServer.IPC.Messages
{
	[ProtoContract]
	public class AddCharacterMessage : IPCMessage
	{
		[ProtoMember(2)]
		public int AccountId
		{
			get;
			set;
		}
		[ProtoMember(3)]
		public int CharacterId
		{
			get;
			set;
		}
		public AddCharacterMessage()
		{
		}
		public AddCharacterMessage(int accountId, int characterId)
		{
			this.AccountId = accountId;
			this.CharacterId = characterId;
		}
	}
}
