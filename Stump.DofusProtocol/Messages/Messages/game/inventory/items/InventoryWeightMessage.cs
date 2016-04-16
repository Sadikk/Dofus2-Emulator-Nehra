









// Generated on 07/24/2015 23:20:12
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class InventoryWeightMessage : Message
    {
        public const ushort Id = 3009;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public uint weight;
        public uint weightMax;
        
        public InventoryWeightMessage()
        {
        }
        
        public InventoryWeightMessage(uint weight, uint weightMax)
        {
            this.weight = weight;
            this.weightMax = weightMax;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteVarUhInt(weight);
            writer.WriteVarUhInt(weightMax);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            weight = reader.ReadVarUhInt();
            if (weight < 0)
                throw new Exception("Forbidden value on weight = " + weight + ", it doesn't respect the following condition : weight < 0");
            weightMax = reader.ReadVarUhInt();
            if (weightMax < 0)
                throw new Exception("Forbidden value on weightMax = " + weightMax + ", it doesn't respect the following condition : weightMax < 0");
        }
        
    }
    
}