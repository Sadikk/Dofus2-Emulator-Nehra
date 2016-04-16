









// Generated on 07/24/2015 23:20:06
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class GuildInvitationByNameMessage : Message
    {
        public const ushort Id = 6115;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public string name;
        
        public GuildInvitationByNameMessage()
        {
        }
        
        public GuildInvitationByNameMessage(string name)
        {
            this.name = name;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteUTF(name);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            name = reader.ReadUTF();
        }
        
    }
    
}