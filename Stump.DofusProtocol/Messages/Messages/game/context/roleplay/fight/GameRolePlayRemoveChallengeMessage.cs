









// Generated on 07/24/2015 23:19:58
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class GameRolePlayRemoveChallengeMessage : Message
    {
        public const ushort Id = 300;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public int fightId;
        
        public GameRolePlayRemoveChallengeMessage()
        {
        }
        
        public GameRolePlayRemoveChallengeMessage(int fightId)
        {
            this.fightId = fightId;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteInt(fightId);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            fightId = reader.ReadInt();
        }
        
    }
    
}