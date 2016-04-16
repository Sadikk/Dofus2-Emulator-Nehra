









// Generated on 07/24/2015 23:20:11
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class ExchangeStartedBidSellerMessage : Message
    {
        public const ushort Id = 5905;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public Types.SellerBuyerDescriptor sellerDescriptor;
        public IEnumerable<Types.ObjectItemToSellInBid> objectsInfos;
        
        public ExchangeStartedBidSellerMessage()
        {
        }
        
        public ExchangeStartedBidSellerMessage(Types.SellerBuyerDescriptor sellerDescriptor, IEnumerable<Types.ObjectItemToSellInBid> objectsInfos)
        {
            this.sellerDescriptor = sellerDescriptor;
            this.objectsInfos = objectsInfos;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            sellerDescriptor.Serialize(writer);
            writer.WriteUShort((ushort)objectsInfos.Count());
            foreach (var entry in objectsInfos)
            {
                 entry.Serialize(writer);
            }
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            sellerDescriptor = new Types.SellerBuyerDescriptor();
            sellerDescriptor.Deserialize(reader);
            var limit = reader.ReadShort();
            objectsInfos = new Types.ObjectItemToSellInBid[limit];
            for (int i = 0; i < limit; i++)
            {
                 (objectsInfos as Types.ObjectItemToSellInBid[])[i] = new Types.ObjectItemToSellInBid();
                 (objectsInfos as Types.ObjectItemToSellInBid[])[i].Deserialize(reader);
            }
        }
        
    }
    
}