









// Generated on 07/24/2015 23:20:09
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class ExchangeBidHouseTypeMessage : Message
    {
        public const ushort Id = 5803;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public uint type;
        
        public ExchangeBidHouseTypeMessage()
        {
        }
        
        public ExchangeBidHouseTypeMessage(uint type)
        {
            this.type = type;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteVarUhInt(type);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            type = reader.ReadVarUhInt();
            if (type < 0)
                throw new Exception("Forbidden value on type = " + type + ", it doesn't respect the following condition : type < 0");
        }
        
    }
    
}