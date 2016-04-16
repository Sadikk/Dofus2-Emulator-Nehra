









// Generated on 07/24/2015 23:20:05
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class GuildMemberSetWarnOnConnectionMessage : Message
    {
        public const ushort Id = 6159;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public bool enable;
        
        public GuildMemberSetWarnOnConnectionMessage()
        {
        }
        
        public GuildMemberSetWarnOnConnectionMessage(bool enable)
        {
            this.enable = enable;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteBoolean(enable);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            enable = reader.ReadBoolean();
        }
        
    }
    
}