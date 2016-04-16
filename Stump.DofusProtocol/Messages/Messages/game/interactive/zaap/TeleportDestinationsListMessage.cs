









// Generated on 07/24/2015 23:20:08
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class TeleportDestinationsListMessage : Message
    {
        public const ushort Id = 5960;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public sbyte teleporterType;
        public IEnumerable<int> mapIds;
        public IEnumerable<ushort> subAreaIds;
        public IEnumerable<ushort> costs;
        public IEnumerable<sbyte> destTeleporterType;
        
        public TeleportDestinationsListMessage()
        {
        }
        
        public TeleportDestinationsListMessage(sbyte teleporterType, IEnumerable<int> mapIds, IEnumerable<ushort> subAreaIds, IEnumerable<ushort> costs, IEnumerable<sbyte> destTeleporterType)
        {
            this.teleporterType = teleporterType;
            this.mapIds = mapIds;
            this.subAreaIds = subAreaIds;
            this.costs = costs;
            this.destTeleporterType = destTeleporterType;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteSByte(teleporterType);
            writer.WriteUShort((ushort)mapIds.Count());
            foreach (var entry in mapIds)
            {
                 writer.WriteInt(entry);
            }
            writer.WriteUShort((ushort)subAreaIds.Count());
            foreach (var entry in subAreaIds)
            {
                 writer.WriteVarUhShort(entry);
            }
            writer.WriteUShort((ushort)costs.Count());
            foreach (var entry in costs)
            {
                 writer.WriteVarUhShort(entry);
            }
            writer.WriteUShort((ushort)destTeleporterType.Count());
            foreach (var entry in destTeleporterType)
            {
                 writer.WriteSByte(entry);
            }
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            teleporterType = reader.ReadSByte();
            if (teleporterType < 0)
                throw new Exception("Forbidden value on teleporterType = " + teleporterType + ", it doesn't respect the following condition : teleporterType < 0");
            var limit = reader.ReadShort();
            mapIds = new int[limit];
            for (int i = 0; i < limit; i++)
            {
                 (mapIds as int[])[i] = reader.ReadInt();
            }
            limit = reader.ReadShort();
            subAreaIds = new ushort[limit];
            for (int i = 0; i < limit; i++)
            {
                 (subAreaIds as ushort[])[i] = reader.ReadVarUhShort();
            }
            limit = reader.ReadShort();
            costs = new ushort[limit];
            for (int i = 0; i < limit; i++)
            {
                 (costs as ushort[])[i] = reader.ReadVarUhShort();
            }
            limit = reader.ReadShort();
            destTeleporterType = new sbyte[limit];
            for (int i = 0; i < limit; i++)
            {
                 (destTeleporterType as sbyte[])[i] = reader.ReadSByte();
            }
        }
        
    }
    
}