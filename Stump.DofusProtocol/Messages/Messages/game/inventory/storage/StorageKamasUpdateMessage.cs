









// Generated on 07/24/2015 23:20:14
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class StorageKamasUpdateMessage : Message
    {
        public const ushort Id = 5645;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public int kamasTotal;
        
        public StorageKamasUpdateMessage()
        {
        }
        
        public StorageKamasUpdateMessage(int kamasTotal)
        {
            this.kamasTotal = kamasTotal;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteInt(kamasTotal);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            kamasTotal = reader.ReadInt();
        }
        
    }
    
}