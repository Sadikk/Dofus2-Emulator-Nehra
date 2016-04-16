









// Generated on 07/24/2015 23:19:45
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class SelectedServerDataMessage : Message
    {
        public const ushort Id = 42;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public ushort serverId;
        public string address;
        public ushort port;
        public bool canCreateNewCharacter;
        public IEnumerable<byte> ticket;
        
        public SelectedServerDataMessage()
        {
        }
        
        public SelectedServerDataMessage(ushort serverId, string address, ushort port, bool canCreateNewCharacter, IEnumerable<byte> ticket)
        {
            this.serverId = serverId;
            this.address = address;
            this.port = port;
            this.canCreateNewCharacter = canCreateNewCharacter;
            this.ticket = ticket;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteVarUhShort(serverId);
            writer.WriteUTF(address);
            writer.WriteUShort(port);
            writer.WriteBoolean(canCreateNewCharacter);
            writer.WriteVarInt(ticket.Count());
            foreach (var entry in ticket)
            {
                 writer.WriteByte(entry);
            }
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            serverId = reader.ReadVarUhShort();
            if (serverId < 0)
                throw new Exception("Forbidden value on serverId = " + serverId + ", it doesn't respect the following condition : serverId < 0");
            address = reader.ReadUTF();
            port = reader.ReadUShort();
            if (port < 0 || port > 65535)
                throw new Exception("Forbidden value on port = " + port + ", it doesn't respect the following condition : port < 0 || port > 65535");
            canCreateNewCharacter = reader.ReadBoolean();
            var limit = reader.ReadVarInt();
            ticket = new byte[limit];
            for (int i = 0; i < limit; i++)
            {
                 (ticket as byte[])[i] = reader.ReadByte();
            }
        }
        
    }
    
}