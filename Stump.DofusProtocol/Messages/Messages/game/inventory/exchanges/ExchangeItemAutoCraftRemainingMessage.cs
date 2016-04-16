









// Generated on 07/24/2015 23:20:09
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class ExchangeItemAutoCraftRemainingMessage : Message
    {
        public const ushort Id = 6015;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public uint count;
        
        public ExchangeItemAutoCraftRemainingMessage()
        {
        }
        
        public ExchangeItemAutoCraftRemainingMessage(uint count)
        {
            this.count = count;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteVarUhInt(count);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            count = reader.ReadVarUhInt();
            if (count < 0)
                throw new Exception("Forbidden value on count = " + count + ", it doesn't respect the following condition : count < 0");
        }
        
    }
    
}