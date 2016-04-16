









// Generated on 07/24/2015 23:20:06
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class GuildMemberLeavingMessage : Message
    {
        public const ushort Id = 5923;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public bool kicked;
        public int memberId;
        
        public GuildMemberLeavingMessage()
        {
        }
        
        public GuildMemberLeavingMessage(bool kicked, int memberId)
        {
            this.kicked = kicked;
            this.memberId = memberId;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteBoolean(kicked);
            writer.WriteInt(memberId);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            kicked = reader.ReadBoolean();
            memberId = reader.ReadInt();
        }
        
    }
    
}