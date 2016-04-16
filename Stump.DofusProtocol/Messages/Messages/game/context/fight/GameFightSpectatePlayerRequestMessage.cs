









// Generated on 07/24/2015 23:19:55
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class GameFightSpectatePlayerRequestMessage : Message
    {
        public const ushort Id = 6474;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public int playerId;
        
        public GameFightSpectatePlayerRequestMessage()
        {
        }
        
        public GameFightSpectatePlayerRequestMessage(int playerId)
        {
            this.playerId = playerId;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteInt(playerId);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            playerId = reader.ReadInt();
        }
        
    }
    
}