









// Generated on 07/24/2015 23:19:46
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class FriendGuildSetWarnOnAchievementCompleteMessage : Message
    {
        public const ushort Id = 6382;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public bool enable;
        
        public FriendGuildSetWarnOnAchievementCompleteMessage()
        {
        }
        
        public FriendGuildSetWarnOnAchievementCompleteMessage(bool enable)
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