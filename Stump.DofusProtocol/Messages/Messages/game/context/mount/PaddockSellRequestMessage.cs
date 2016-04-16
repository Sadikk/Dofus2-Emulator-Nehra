









// Generated on 07/24/2015 23:19:56
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class PaddockSellRequestMessage : Message
    {
        public const ushort Id = 5953;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public uint price;
        
        public PaddockSellRequestMessage()
        {
        }
        
        public PaddockSellRequestMessage(uint price)
        {
            this.price = price;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteVarUhInt(price);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            price = reader.ReadVarUhInt();
            if (price < 0)
                throw new Exception("Forbidden value on price = " + price + ", it doesn't respect the following condition : price < 0");
        }
        
    }
    
}