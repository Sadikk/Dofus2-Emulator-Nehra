









// Generated on 07/24/2015 23:19:57
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class MapFightCountMessage : Message
    {
        public const ushort Id = 210;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public ushort fightCount;
        
        public MapFightCountMessage()
        {
        }
        
        public MapFightCountMessage(ushort fightCount)
        {
            this.fightCount = fightCount;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteVarUhShort(fightCount);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            fightCount = reader.ReadVarUhShort();
            if (fightCount < 0)
                throw new Exception("Forbidden value on fightCount = " + fightCount + ", it doesn't respect the following condition : fightCount < 0");
        }
        
    }
    
}