









// Generated on 07/24/2015 23:19:57
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class GameRolePlayPlayerLifeStatusMessage : Message
    {
        public const ushort Id = 5996;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public sbyte state;
        
        public GameRolePlayPlayerLifeStatusMessage()
        {
        }
        
        public GameRolePlayPlayerLifeStatusMessage(sbyte state)
        {
            this.state = state;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteSByte(state);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            state = reader.ReadSByte();
            if (state < 0)
                throw new Exception("Forbidden value on state = " + state + ", it doesn't respect the following condition : state < 0");
        }
        
    }
    
}