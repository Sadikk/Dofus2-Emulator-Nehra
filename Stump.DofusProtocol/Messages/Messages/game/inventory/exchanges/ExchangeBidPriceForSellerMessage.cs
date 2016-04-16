









// Generated on 07/24/2015 23:20:09
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class ExchangeBidPriceForSellerMessage : ExchangeBidPriceMessage
    {
        public const ushort Id = 6464;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public bool allIdentical;
        public IEnumerable<uint> minimalPrices;
        
        public ExchangeBidPriceForSellerMessage()
        {
        }
        
        public ExchangeBidPriceForSellerMessage(ushort genericId, int averagePrice, bool allIdentical, IEnumerable<uint> minimalPrices)
         : base(genericId, averagePrice)
        {
            this.allIdentical = allIdentical;
            this.minimalPrices = minimalPrices;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            base.Serialize(writer);
            writer.WriteBoolean(allIdentical);
            writer.WriteUShort((ushort)minimalPrices.Count());
            foreach (var entry in minimalPrices)
            {
                 writer.WriteVarUhInt(entry);
            }
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            base.Deserialize(reader);
            allIdentical = reader.ReadBoolean();
            var limit = reader.ReadShort();
            minimalPrices = new uint[limit];
            for (int i = 0; i < limit; i++)
            {
                 (minimalPrices as uint[])[i] = reader.ReadVarUhInt();
            }
        }
        
    }
    
}