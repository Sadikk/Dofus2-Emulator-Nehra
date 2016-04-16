









// Generated on 07/24/2015 23:20:06
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class GuildInformationsMembersMessage : Message
    {
        public const ushort Id = 5558;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public IEnumerable<Types.GuildMember> members;
        
        public GuildInformationsMembersMessage()
        {
        }
        
        public GuildInformationsMembersMessage(IEnumerable<Types.GuildMember> members)
        {
            this.members = members;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteUShort((ushort)members.Count());
            foreach (var entry in members)
            {
                 entry.Serialize(writer);
            }
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            var limit = reader.ReadShort();
            members = new Types.GuildMember[limit];
            for (int i = 0; i < limit; i++)
            {
                 (members as Types.GuildMember[])[i] = new Types.GuildMember();
                 (members as Types.GuildMember[])[i].Deserialize(reader);
            }
        }
        
    }
    
}