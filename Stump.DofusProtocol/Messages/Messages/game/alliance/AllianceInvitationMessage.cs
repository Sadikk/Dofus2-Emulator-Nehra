









// Generated on 07/24/2015 23:19:49
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class AllianceInvitationMessage : Message
    {
        public const ushort Id = 6395;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public uint targetId;
        
        public AllianceInvitationMessage()
        {
        }
        
        public AllianceInvitationMessage(uint targetId)
        {
            this.targetId = targetId;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteVarUhInt(targetId);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            targetId = reader.ReadVarUhInt();
            if (targetId < 0)
                throw new Exception("Forbidden value on targetId = " + targetId + ", it doesn't respect the following condition : targetId < 0");
        }
        
    }
    
}