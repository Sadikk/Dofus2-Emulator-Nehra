









// Generated on 07/24/2015 23:20:09
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class ExchangeCraftPaymentModifiedMessage : Message
    {
        public const ushort Id = 6578;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public uint goldSum;
        
        public ExchangeCraftPaymentModifiedMessage()
        {
        }
        
        public ExchangeCraftPaymentModifiedMessage(uint goldSum)
        {
            this.goldSum = goldSum;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteVarUhInt(goldSum);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            goldSum = reader.ReadVarUhInt();
            if (goldSum < 0)
                throw new Exception("Forbidden value on goldSum = " + goldSum + ", it doesn't respect the following condition : goldSum < 0");
        }
        
    }
    
}