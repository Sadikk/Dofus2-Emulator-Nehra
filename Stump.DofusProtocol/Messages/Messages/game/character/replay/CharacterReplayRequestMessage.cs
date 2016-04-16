









// Generated on 07/24/2015 23:19:52
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class CharacterReplayRequestMessage : Message
    {
        public const ushort Id = 167;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public int characterId;
        
        public CharacterReplayRequestMessage()
        {
        }
        
        public CharacterReplayRequestMessage(int characterId)
        {
            this.characterId = characterId;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteInt(characterId);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            characterId = reader.ReadInt();
            if (characterId < 0)
                throw new Exception("Forbidden value on characterId = " + characterId + ", it doesn't respect the following condition : characterId < 0");
        }
        
    }
    
}