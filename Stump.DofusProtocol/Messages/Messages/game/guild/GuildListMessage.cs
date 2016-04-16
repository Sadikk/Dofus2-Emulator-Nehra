









// Generated on 07/24/2015 23:20:06
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class GuildListMessage : Message
    {
        public const ushort Id = 6413;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public IEnumerable<Types.GuildInformations> guilds;
        
        public GuildListMessage()
        {
        }
        
        public GuildListMessage(IEnumerable<Types.GuildInformations> guilds)
        {
            this.guilds = guilds;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteUShort((ushort)guilds.Count());
            foreach (var entry in guilds)
            {
                 entry.Serialize(writer);
            }
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            var limit = reader.ReadShort();
            guilds = new Types.GuildInformations[limit];
            for (int i = 0; i < limit; i++)
            {
                 (guilds as Types.GuildInformations[])[i] = new Types.GuildInformations();
                 (guilds as Types.GuildInformations[])[i].Deserialize(reader);
            }
        }
        
    }
    
}