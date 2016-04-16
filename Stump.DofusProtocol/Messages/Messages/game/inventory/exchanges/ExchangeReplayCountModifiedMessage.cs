









// Generated on 07/24/2015 23:20:10
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class ExchangeReplayCountModifiedMessage : Message
    {
        public const ushort Id = 6023;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public int count;
        
        public ExchangeReplayCountModifiedMessage()
        {
        }
        
        public ExchangeReplayCountModifiedMessage(int count)
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