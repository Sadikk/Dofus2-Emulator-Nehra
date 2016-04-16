









// Generated on 07/24/2015 23:20:09
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class ExchangeIsReadyMessage : Message
    {
        public const ushort Id = 5509;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public uint id;
        public bool ready;
        
        public ExchangeIsReadyMessage()
        {
        }
        
        public ExchangeIsReadyMessage(uint id, bool ready)
        {
            this.id = id;
            this.ready = ready;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteVarUhInt(id);
            writer.WriteBoolean(ready);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            id = reader.ReadVarUhInt();
            if (id < 0)
                throw new Exception("Forbidden value on id = " + id + ", it doesn't respect the following condition : id < 0");
            ready = reader.ReadBoolean();
        }
        
    }
    
}