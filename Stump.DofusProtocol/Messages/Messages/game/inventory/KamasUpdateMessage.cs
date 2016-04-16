









// Generated on 07/24/2015 23:20:08
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class KamasUpdateMessage : Message
    {
        public const ushort Id = 5537;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public int kamasTotal;
        
        public KamasUpdateMessage()
        {
        }
        
        public KamasUpdateMessage(int kamasTotal)
        {
            this.kamasTotal = kamasTotal;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteVarInt(kamasTotal);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            kamasTotal = reader.ReadVarInt();
        }
        
    }
    
}