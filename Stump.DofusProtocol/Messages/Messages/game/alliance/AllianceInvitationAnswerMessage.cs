









// Generated on 07/24/2015 23:19:49
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class AllianceInvitationAnswerMessage : Message
    {
        public const ushort Id = 6401;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public bool accept;
        
        public AllianceInvitationAnswerMessage()
        {
        }
        
        public AllianceInvitationAnswerMessage(bool accept)
        {
            this.accept = accept;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteBoolean(accept);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            accept = reader.ReadBoolean();
        }
        
    }
    
}