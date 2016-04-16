









// Generated on 07/24/2015 23:19:59
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class HouseSellRequestMessage : Message
    {
        public const ushort Id = 5697;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public uint amount;
        
        public HouseSellRequestMessage()
        {
        }
        
        public HouseSellRequestMessage(uint amount)
        {
            this.amount = amount;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteVarUhInt(amount);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            amount = reader.ReadVarUhInt();
            if (amount < 0)
                throw new Exception("Forbidden value on amount = " + amount + ", it doesn't respect the following condition : amount < 0");
        }
        
    }
    
}