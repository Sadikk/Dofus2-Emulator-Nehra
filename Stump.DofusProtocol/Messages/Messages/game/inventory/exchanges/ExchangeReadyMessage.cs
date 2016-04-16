









// Generated on 07/24/2015 23:20:10
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class ExchangeReadyMessage : Message
    {
        public const ushort Id = 5511;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public bool ready;
        public ushort step;
        
        public ExchangeReadyMessage()
        {
        }
        
        public ExchangeReadyMessage(bool ready, ushort step)
        {
            this.ready = ready;
            this.step = step;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteBoolean(ready);
            writer.WriteVarUhShort(step);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            ready = reader.ReadBoolean();
            step = reader.ReadVarUhShort();
            if (step < 0)
                throw new Exception("Forbidden value on step = " + step + ", it doesn't respect the following condition : step < 0");
        }
        
    }
    
}