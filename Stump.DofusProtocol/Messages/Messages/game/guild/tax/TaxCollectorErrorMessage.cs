









// Generated on 07/24/2015 23:20:07
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class TaxCollectorErrorMessage : Message
    {
        public const ushort Id = 5634;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public sbyte reason;
        
        public TaxCollectorErrorMessage()
        {
        }
        
        public TaxCollectorErrorMessage(sbyte reason)
        {
            this.reason = reason;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteSByte(reason);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            reason = reader.ReadSByte();
        }
        
    }
    
}