









// Generated on 07/24/2015 23:19:55
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class GameFightStartingMessage : Message
    {
        public const ushort Id = 700;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public sbyte fightType;
        public int attackerId;
        public int defenderId;
        
        public GameFightStartingMessage()
        {
        }
        
        public GameFightStartingMessage(sbyte fightType, int attackerId, int defenderId)
        {
            this.fightType = fightType;
            this.attackerId = attackerId;
            this.defenderId = defenderId;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteSByte(fightType);
            writer.WriteInt(attackerId);
            writer.WriteInt(defenderId);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            fightType = reader.ReadSByte();
            if (fightType < 0)
                throw new Exception("Forbidden value on fightType = " + fightType + ", it doesn't respect the following condition : fightType < 0");
            attackerId = reader.ReadInt();
            defenderId = reader.ReadInt();
        }
        
    }
    
}