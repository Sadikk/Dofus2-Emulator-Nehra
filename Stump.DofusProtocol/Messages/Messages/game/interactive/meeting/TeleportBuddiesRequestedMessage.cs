









// Generated on 07/24/2015 23:20:08
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class TeleportBuddiesRequestedMessage : Message
    {
        public const ushort Id = 6302;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public ushort dungeonId;
        public uint inviterId;
        public IEnumerable<uint> invalidBuddiesIds;
        
        public TeleportBuddiesRequestedMessage()
        {
        }
        
        public TeleportBuddiesRequestedMessage(ushort dungeonId, uint inviterId, IEnumerable<uint> invalidBuddiesIds)
        {
            this.dungeonId = dungeonId;
            this.inviterId = inviterId;
            this.invalidBuddiesIds = invalidBuddiesIds;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteVarUhShort(dungeonId);
            writer.WriteVarUhInt(inviterId);
            writer.WriteUShort((ushort)invalidBuddiesIds.Count());
            foreach (var entry in invalidBuddiesIds)
            {
                 writer.WriteVarUhInt(entry);
            }
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            dungeonId = reader.ReadVarUhShort();
            if (dungeonId < 0)
                throw new Exception("Forbidden value on dungeonId = " + dungeonId + ", it doesn't respect the following condition : dungeonId < 0");
            inviterId = reader.ReadVarUhInt();
            if (inviterId < 0)
                throw new Exception("Forbidden value on inviterId = " + inviterId + ", it doesn't respect the following condition : inviterId < 0");
            var limit = reader.ReadShort();
            invalidBuddiesIds = new uint[limit];
            for (int i = 0; i < limit; i++)
            {
                 (invalidBuddiesIds as uint[])[i] = reader.ReadVarUhInt();
            }
        }
        
    }
    
}