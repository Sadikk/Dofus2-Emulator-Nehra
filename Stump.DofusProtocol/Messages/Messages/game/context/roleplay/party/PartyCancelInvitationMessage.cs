









// Generated on 07/24/2015 23:20:01
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class PartyCancelInvitationMessage : AbstractPartyMessage
    {
        public const ushort Id = 6254;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public uint guestId;
        
        public PartyCancelInvitationMessage()
        {
        }
        
        public PartyCancelInvitationMessage(uint partyId, uint guestId)
         : base(partyId)
        {
            this.guestId = guestId;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            base.Serialize(writer);
            writer.WriteVarUhInt(guestId);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            base.Deserialize(reader);
            guestId = reader.ReadVarUhInt();
            if (guestId < 0)
                throw new Exception("Forbidden value on guestId = " + guestId + ", it doesn't respect the following condition : guestId < 0");
        }
        
    }
    
}