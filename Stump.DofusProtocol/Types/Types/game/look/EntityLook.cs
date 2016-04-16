


















// Generated on 07/24/2015 23:20:25
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Types
{

    public class EntityLook
    {

        public const short Id = 55;
        public virtual short TypeId
        {
            get { return Id; }
        }

        public ushort bonesId;
        public IEnumerable<ushort> skins;
        public IEnumerable<int> indexedColors;
        public IEnumerable<short> scales;
        public IEnumerable<Types.SubEntity> subentities;


        public EntityLook()
        {
        }

        public EntityLook(ushort bonesId, IEnumerable<ushort> skins, IEnumerable<int> indexedColors, IEnumerable<short> scales, IEnumerable<Types.SubEntity> subentities)
        {
            this.bonesId = bonesId;
            this.skins = skins;
            this.indexedColors = indexedColors;
            this.scales = scales;
            this.subentities = subentities;
        }


        public virtual void Serialize(ICustomDataOutput writer)
        {

            writer.WriteVarUhShort(bonesId);
            writer.WriteUShort((ushort)skins.Count());
            foreach (var entry in skins)
            {
                writer.WriteVarUhShort(entry);
            }
            writer.WriteUShort((ushort)indexedColors.Count());
            foreach (var entry in indexedColors)
            {
                writer.WriteInt(entry);
            }
            writer.WriteUShort((ushort)scales.Count());
            foreach (var entry in scales)
            {
                writer.WriteVarShort(entry);
            }
            writer.WriteUShort((ushort)subentities.Count());
            foreach (var entry in subentities)
            {
                entry.Serialize(writer);
            }


        }

        public virtual void Deserialize(ICustomDataInput reader)
        {

            bonesId = reader.ReadVarUhShort();
            if (bonesId < 0)
                throw new Exception("Forbidden value on bonesId = " + bonesId + ", it doesn't respect the following condition : bonesId < 0");
            var limit = reader.ReadShort();
            skins = new ushort[limit];
            for (int i = 0; i < limit; i++)
            {
                (skins as ushort[])[i] = reader.ReadVarUhShort();
            }
            limit = reader.ReadShort();
            indexedColors = new int[limit];
            for (int i = 0; i < limit; i++)
            {
                (indexedColors as int[])[i] = reader.ReadInt();
            }
            limit = reader.ReadShort();
            scales = new short[limit];
            for (int i = 0; i < limit; i++)
            {
                (scales as short[])[i] = reader.ReadVarShort();
            }
            limit = reader.ReadShort();
            subentities = new Types.SubEntity[limit];
            for (int i = 0; i < limit; i++)
            {
                (subentities as Types.SubEntity[])[i] = new Types.SubEntity();
                (subentities as Types.SubEntity[])[i].Deserialize(reader);
            }
        }
    }
}