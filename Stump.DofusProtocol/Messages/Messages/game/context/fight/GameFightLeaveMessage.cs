









// Generated on 07/24/2015 23:19:54
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class GameFightLeaveMessage : Message
    {
        public const ushort Id = 721;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public int charId;
        
        public GameFightLeaveMessage()
        {
        }
        
        public GameFightLeaveMessage(int charId)
        {
            this.charId = charId;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteInt(charId);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            charId = reader.ReadInt();
        }
        
    }
    
}