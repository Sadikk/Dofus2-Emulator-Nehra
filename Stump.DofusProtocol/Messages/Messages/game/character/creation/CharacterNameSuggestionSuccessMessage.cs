









// Generated on 07/24/2015 23:19:52
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class CharacterNameSuggestionSuccessMessage : Message
    {
        public const ushort Id = 5544;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public string suggestion;
        
        public CharacterNameSuggestionSuccessMessage()
        {
        }
        
        public CharacterNameSuggestionSuccessMessage(string suggestion)
        {
            this.suggestion = suggestion;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteUTF(suggestion);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            suggestion = reader.ReadUTF();
        }
        
    }
    
}