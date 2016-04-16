









// Generated on 07/24/2015 23:19:57
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class CurrentMapMessage : Message
    {
        public const ushort Id = 220;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public int mapId;
        public string mapKey;
        
        public CurrentMapMessage()
        {
        }
        
        public CurrentMapMessage(int mapId, string mapKey)
        {
            this.mapId = mapId;
            this.mapKey = mapKey;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteInt(mapId);
            writer.WriteUTF(mapKey);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            mapId = reader.ReadInt();
            if (mapId < 0)
                throw new Exception("Forbidden value on mapId = " + mapId + ", it doesn't respect the following condition : mapId < 0");
            mapKey = reader.ReadUTF();
        }
        
    }
    
}