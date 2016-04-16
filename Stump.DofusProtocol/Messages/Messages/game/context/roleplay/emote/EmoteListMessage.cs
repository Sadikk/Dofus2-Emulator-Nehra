









// Generated on 07/24/2015 23:19:58
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class EmoteListMessage : Message
    {
        public const ushort Id = 5689;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public IEnumerable<byte> emoteIds;
        
        public EmoteListMessage()
        {
        }
        
        public EmoteListMessage(IEnumerable<byte> emoteIds)
        {
            this.emoteIds = emoteIds;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteUShort((ushort)emoteIds.Count());
            foreach (var entry in emoteIds)
            {
                 writer.WriteByte(entry);
            }
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            var limit = reader.ReadShort();
            emoteIds = new byte[limit];
            for (int i = 0; i < limit; i++)
            {
                 (emoteIds as byte[])[i] = reader.ReadByte();
            }
        }
        
    }
    
}