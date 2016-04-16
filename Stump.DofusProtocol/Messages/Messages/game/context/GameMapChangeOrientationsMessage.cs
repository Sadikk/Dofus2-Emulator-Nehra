









// Generated on 07/24/2015 23:19:54
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class GameMapChangeOrientationsMessage : Message
    {
        public const ushort Id = 6155;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public IEnumerable<Types.ActorOrientation> orientations;
        
        public GameMapChangeOrientationsMessage()
        {
        }
        
        public GameMapChangeOrientationsMessage(IEnumerable<Types.ActorOrientation> orientations)
        {
            this.orientations = orientations;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteUShort((ushort)orientations.Count());
            foreach (var entry in orientations)
            {
                 entry.Serialize(writer);
            }
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            var limit = reader.ReadShort();
            orientations = new Types.ActorOrientation[limit];
            for (int i = 0; i < limit; i++)
            {
                 (orientations as Types.ActorOrientation[])[i] = new Types.ActorOrientation();
                 (orientations as Types.ActorOrientation[])[i].Deserialize(reader);
            }
        }
        
    }
    
}