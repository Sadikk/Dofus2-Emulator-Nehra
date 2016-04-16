









// Generated on 07/24/2015 23:20:04
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class TreasureHuntLegendaryRequestMessage : Message
    {
        public const ushort Id = 6499;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public ushort legendaryId;
        
        public TreasureHuntLegendaryRequestMessage()
        {
        }
        
        public TreasureHuntLegendaryRequestMessage(ushort legendaryId)
        {
            this.legendaryId = legendaryId;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteVarUhShort(legendaryId);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            legendaryId = reader.ReadVarUhShort();
            if (legendaryId < 0)
                throw new Exception("Forbidden value on legendaryId = " + legendaryId + ", it doesn't respect the following condition : legendaryId < 0");
        }
        
    }
    
}