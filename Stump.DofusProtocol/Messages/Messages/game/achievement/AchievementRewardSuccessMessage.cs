









// Generated on 07/24/2015 23:19:46
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class AchievementRewardSuccessMessage : Message
    {
        public const ushort Id = 6376;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public short achievementId;
        
        public AchievementRewardSuccessMessage()
        {
        }
        
        public AchievementRewardSuccessMessage(short achievementId)
        {
            this.achievementId = achievementId;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteShort(achievementId);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            achievementId = reader.ReadShort();
        }
        
    }
    
}