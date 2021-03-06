









// Generated on 07/24/2015 23:19:53
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class MoodSmileyUpdateMessage : Message
    {
        public const ushort Id = 6388;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public int accountId;
        public uint playerId;
        public sbyte smileyId;
        
        public MoodSmileyUpdateMessage()
        {
        }
        
        public MoodSmileyUpdateMessage(int accountId, uint playerId, sbyte smileyId)
        {
            this.accountId = accountId;
            this.playerId = playerId;
            this.smileyId = smileyId;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteInt(accountId);
            writer.WriteVarUhInt(playerId);
            writer.WriteSByte(smileyId);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            accountId = reader.ReadInt();
            if (accountId < 0)
                throw new Exception("Forbidden value on accountId = " + accountId + ", it doesn't respect the following condition : accountId < 0");
            playerId = reader.ReadVarUhInt();
            if (playerId < 0)
                throw new Exception("Forbidden value on playerId = " + playerId + ", it doesn't respect the following condition : playerId < 0");
            smileyId = reader.ReadSByte();
        }
        
    }
    
}