using Stump.Core.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stump.DbSynchronizer.Protocol.DLM.Elements
{
    public class SoundElement : BasicElement
    {
        // FIELDS
        public int soundId;
        public short baseVolume;
        public int fullVolumeDistance;
        public int nullVolumeDistance;
        public short minDelayBetweenLoops;
        public short maxDelayBetweenLoops;

        // PROPERTIES
        public override int ElementType
        {
            get
            {
                return (int)ElementTypesEnum.SOUND;
            }
        }

        // CONSTRUCTORS
        public SoundElement(Cell parent)
            : base(parent)
        { }

        // METHODS
        public override void FromRaw(IDataReader reader, int mapVersion)
        {
            try
            {
                this.soundId = reader.ReadInt();
                this.baseVolume = reader.ReadShort();
                this.fullVolumeDistance = reader.ReadInt();
                this.nullVolumeDistance = reader.ReadInt();
                this.minDelayBetweenLoops = reader.ReadShort();
                this.maxDelayBetweenLoops = reader.ReadShort();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}