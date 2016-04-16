









// Generated on 07/24/2015 23:19:45
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class SelectedServerDataExtendedMessage : SelectedServerDataMessage
    {
        public const ushort Id = 6469;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public IEnumerable<ushort> serverIds;
        
        public SelectedServerDataExtendedMessage()
        {
        }
        
        public SelectedServerDataExtendedMessage(ushort serverId, string address, ushort port, bool canCreateNewCharacter, IEnumerable<byte> ticket, IEnumerable<ushort> serverIds)
         : base(serverId, address, port, canCreateNewCharacter, ticket)
        {
            this.serverIds = serverIds;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            base.Serialize(writer);
            writer.WriteUShort((ushort)serverIds.Count());
            foreach (var entry in serverIds)
            {
                 writer.WriteVarUhShort(entry);
            }
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            base.Deserialize(reader);
            var limit = reader.ReadShort();
            serverIds = new ushort[limit];
            for (int i = 0; i < limit; i++)
            {
                 (serverIds as ushort[])[i] = reader.ReadVarUhShort();
            }
        }
        
    }
    
}