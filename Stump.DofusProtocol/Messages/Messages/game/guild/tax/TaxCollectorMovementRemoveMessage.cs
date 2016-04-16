









// Generated on 07/24/2015 23:20:07
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class TaxCollectorMovementRemoveMessage : Message
    {
        public const ushort Id = 5915;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public int collectorId;
        
        public TaxCollectorMovementRemoveMessage()
        {
        }
        
        public TaxCollectorMovementRemoveMessage(int collectorId)
        {
            this.collectorId = collectorId;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteInt(collectorId);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            collectorId = reader.ReadInt();
        }
        
    }
    
}