









// Generated on 07/24/2015 23:20:09
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class ExchangeBidHouseItemRemoveOkMessage : Message
    {
        public const ushort Id = 5946;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public int sellerId;
        
        public ExchangeBidHouseItemRemoveOkMessage()
        {
        }
        
        public ExchangeBidHouseItemRemoveOkMessage(int sellerId)
        {
            this.sellerId = sellerId;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteInt(sellerId);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            sellerId = reader.ReadInt();
        }
        
    }
    
}