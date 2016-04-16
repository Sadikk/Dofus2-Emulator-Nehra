









// Generated on 07/24/2015 23:19:57
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class MapRunningFightListMessage : Message
    {
        public const ushort Id = 5743;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public IEnumerable<Types.FightExternalInformations> fights;
        
        public MapRunningFightListMessage()
        {
        }
        
        public MapRunningFightListMessage(IEnumerable<Types.FightExternalInformations> fights)
        {
            this.fights = fights;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteUShort((ushort)fights.Count());
            foreach (var entry in fights)
            {
                 entry.Serialize(writer);
            }
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            var limit = reader.ReadShort();
            fights = new Types.FightExternalInformations[limit];
            for (int i = 0; i < limit; i++)
            {
                 (fights as Types.FightExternalInformations[])[i] = new Types.FightExternalInformations();
                 (fights as Types.FightExternalInformations[])[i].Deserialize(reader);
            }
        }
        
    }
    
}