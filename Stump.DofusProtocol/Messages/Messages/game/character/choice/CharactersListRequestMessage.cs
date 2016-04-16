









// Generated on 07/24/2015 23:19:51
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class CharactersListRequestMessage : Message
    {
        public const ushort Id = 150;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        
        public CharactersListRequestMessage()
        {
        }
        
        
        public override void Serialize(ICustomDataOutput writer)
        {
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
        }
        
    }
    
}