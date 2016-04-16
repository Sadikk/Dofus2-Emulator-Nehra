









// Generated on 07/24/2015 23:20:18
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class KrosmasterInventoryErrorMessage : Message
    {
        public const ushort Id = 6343;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public sbyte reason;
        
        public KrosmasterInventoryErrorMessage()
        {
        }
        
        public KrosmasterInventoryErrorMessage(sbyte reason)
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
            if (reason < 0)
                throw new Exception("Forbidden value on reason = " + reason + ", it doesn't respect the following condition : reason < 0");
        }
        
    }
    
}