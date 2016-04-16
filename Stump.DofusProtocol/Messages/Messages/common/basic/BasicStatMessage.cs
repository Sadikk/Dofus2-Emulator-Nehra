









// Generated on 07/24/2015 23:19:45
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class BasicStatMessage : Message
    {
        public const ushort Id = 6530;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public ushort statId;
        
        public BasicStatMessage()
        {
        }
        
        public BasicStatMessage(ushort statId)
        {
            this.statId = statId;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteVarUhShort(statId);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            statId = reader.ReadVarUhShort();
            if (statId < 0)
                throw new Exception("Forbidden value on statId = " + statId + ", it doesn't respect the following condition : statId < 0");
        }
        
    }
    
}