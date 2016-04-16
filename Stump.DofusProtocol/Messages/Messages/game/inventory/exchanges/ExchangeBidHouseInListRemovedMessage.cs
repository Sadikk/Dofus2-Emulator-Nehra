









// Generated on 07/24/2015 23:20:09
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class ExchangeBidHouseInListRemovedMessage : Message
    {
        public const ushort Id = 5950;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public int itemUID;
        
        public ExchangeBidHouseInListRemovedMessage()
        {
        }
        
        public ExchangeBidHouseInListRemovedMessage(int itemUID)
        {
            this.itemUID = itemUID;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteInt(itemUID);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            itemUID = reader.ReadInt();
        }
        
    }
    
}