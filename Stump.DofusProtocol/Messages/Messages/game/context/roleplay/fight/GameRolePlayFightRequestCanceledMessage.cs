









// Generated on 07/24/2015 23:19:58
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class GameRolePlayFightRequestCanceledMessage : Message
    {
        public const ushort Id = 5822;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public int fightId;
        public uint sourceId;
        public int targetId;
        
        public GameRolePlayFightRequestCanceledMessage()
        {
        }
        
        public GameRolePlayFightRequestCanceledMessage(int fightId, uint sourceId, int targetId)
        {
            this.fightId = fightId;
            this.sourceId = sourceId;
            this.targetId = targetId;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteInt(fightId);
            writer.WriteVarUhInt(sourceId);
            writer.WriteInt(targetId);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            fightId = reader.ReadInt();
            sourceId = reader.ReadVarUhInt();
            if (sourceId < 0)
                throw new Exception("Forbidden value on sourceId = " + sourceId + ", it doesn't respect the following condition : sourceId < 0");
            targetId = reader.ReadInt();
        }
        
    }
    
}