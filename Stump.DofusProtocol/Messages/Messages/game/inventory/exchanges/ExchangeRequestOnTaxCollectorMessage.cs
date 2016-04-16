









// Generated on 07/24/2015 23:20:11
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class ExchangeRequestOnTaxCollectorMessage : Message
    {
        public const ushort Id = 5779;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public int taxCollectorId;
        
        public ExchangeRequestOnTaxCollectorMessage()
        {
        }
        
        public ExchangeRequestOnTaxCollectorMessage(int taxCollectorId)
        {
            this.taxCollectorId = taxCollectorId;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteInt(taxCollectorId);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            taxCollectorId = reader.ReadInt();
        }
        
    }
    
}