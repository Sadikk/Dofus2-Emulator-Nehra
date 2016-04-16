









// Generated on 07/24/2015 23:19:58
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class GameRolePlayAttackMonsterRequestMessage : Message
    {
        public const ushort Id = 6191;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public int monsterGroupId;
        
        public GameRolePlayAttackMonsterRequestMessage()
        {
        }
        
        public GameRolePlayAttackMonsterRequestMessage(int monsterGroupId)
        {
            this.monsterGroupId = monsterGroupId;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteInt(monsterGroupId);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            monsterGroupId = reader.ReadInt();
        }
        
    }
    
}