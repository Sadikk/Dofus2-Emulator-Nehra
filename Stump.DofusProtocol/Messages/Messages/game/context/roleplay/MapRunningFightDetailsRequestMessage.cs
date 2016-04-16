









// Generated on 07/24/2015 23:19:57
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class MapRunningFightDetailsRequestMessage : Message
    {
        public const ushort Id = 5750;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public int fightId;
        
        public MapRunningFightDetailsRequestMessage()
        {
        }
        
        public MapRunningFightDetailsRequestMessage(int fightId)
        {
            this.fightId = fightId;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteInt(fightId);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            fightId = reader.ReadInt();
            if (fightId < 0)
                throw new Exception("Forbidden value on fightId = " + fightId + ", it doesn't respect the following condition : fightId < 0");
        }
        
    }
    
}