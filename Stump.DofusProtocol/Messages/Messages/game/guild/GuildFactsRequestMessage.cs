









// Generated on 07/24/2015 23:20:06
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class GuildFactsRequestMessage : Message
    {
        public const ushort Id = 6404;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public uint guildId;
        
        public GuildFactsRequestMessage()
        {
        }
        
        public GuildFactsRequestMessage(uint guildId)
        {
            this.guildId = guildId;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteVarUhInt(guildId);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            guildId = reader.ReadVarUhInt();
            if (guildId < 0)
                throw new Exception("Forbidden value on guildId = " + guildId + ", it doesn't respect the following condition : guildId < 0");
        }
        
    }
    
}