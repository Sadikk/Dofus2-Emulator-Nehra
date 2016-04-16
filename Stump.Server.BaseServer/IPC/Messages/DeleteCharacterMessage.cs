using ProtoBuf;
using System;
namespace Stump.Server.BaseServer.IPC.Messages
{
	[ProtoContract]
	public class DeleteCharacterMessage : IPCMessage
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
		public DeleteCharacterMessage()
		{
		}
		public DeleteCharacterMessage(int accountId, int characterId)
		{
			this.AccountId = accountId;
			this.CharacterId = characterId;
		}
	}
}
