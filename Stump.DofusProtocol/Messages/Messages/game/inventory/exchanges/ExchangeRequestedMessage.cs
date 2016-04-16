









// Generated on 07/24/2015 23:20:11
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class ExchangeRequestedMessage : Message
    {
        public const ushort Id = 5522;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public sbyte exchangeType;
        
        public ExchangeRequestedMessage()
        {
        }
        
        public ExchangeRequestedMessage(sbyte exchangeType)
        {
            this.exchangeType = exchangeType;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteSByte(exchangeType);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            exchangeType = reader.ReadSByte();
        }
        
    }
    
}