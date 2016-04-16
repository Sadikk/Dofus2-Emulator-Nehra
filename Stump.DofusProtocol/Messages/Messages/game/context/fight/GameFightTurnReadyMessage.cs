









// Generated on 07/24/2015 23:19:55
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class GameFightTurnReadyMessage : Message
    {
        public const ushort Id = 716;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public bool isReady;
        
        public GameFightTurnReadyMessage()
        {
        }
        
        public GameFightTurnReadyMessage(bool isReady)
        {
            this.isReady = isReady;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteBoolean(isReady);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            isReady = reader.ReadBoolean();
        }
        
    }
    
}