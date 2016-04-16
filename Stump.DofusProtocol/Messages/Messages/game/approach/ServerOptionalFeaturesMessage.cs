









// Generated on 07/24/2015 23:19:50
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class ServerOptionalFeaturesMessage : Message
    {
        public const ushort Id = 6305;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public IEnumerable<sbyte> features;
        
        public ServerOptionalFeaturesMessage()
        {
        }
        
        public ServerOptionalFeaturesMessage(IEnumerable<sbyte> features)
        {
            this.features = features;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteUShort((ushort)features.Count());
            foreach (var entry in features)
            {
                 writer.WriteSByte(entry);
            }
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            var limit = reader.ReadShort();
            features = new sbyte[limit];
            for (int i = 0; i < limit; i++)
            {
                 (features as sbyte[])[i] = reader.ReadSByte();
            }
        }
        
    }
    
}