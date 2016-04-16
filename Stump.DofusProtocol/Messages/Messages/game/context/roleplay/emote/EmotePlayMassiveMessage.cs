









// Generated on 07/24/2015 23:19:58
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class EmotePlayMassiveMessage : EmotePlayAbstractMessage
    {
        public const ushort Id = 5691;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public IEnumerable<int> actorIds;
        
        public EmotePlayMassiveMessage()
        {
        }
        
        public EmotePlayMassiveMessage(byte emoteId, double emoteStartTime, IEnumerable<int> actorIds)
         : base(emoteId, emoteStartTime)
        {
            this.actorIds = actorIds;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            base.Serialize(writer);
            writer.WriteUShort((ushort)actorIds.Count());
            foreach (var entry in actorIds)
            {
                 writer.WriteInt(entry);
            }
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            base.Deserialize(reader);
            var limit = reader.ReadShort();
            actorIds = new int[limit];
            for (int i = 0; i < limit; i++)
            {
                 (actorIds as int[])[i] = reader.ReadInt();
            }
        }
        
    }
    
}