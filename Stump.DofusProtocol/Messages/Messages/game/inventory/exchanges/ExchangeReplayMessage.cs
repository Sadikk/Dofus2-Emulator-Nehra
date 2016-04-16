









// Generated on 07/24/2015 23:20:10
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class ExchangeReplayMessage : Message
    {
        public const ushort Id = 6002;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public int count;
        
        public ExchangeReplayMessage()
        {
        }
        
        public ExchangeReplayMessage(int count)
        {
            this.count = count;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteVarInt(count);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            count = reader.ReadVarInt();
        }
        
    }
    
}