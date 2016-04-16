using Stump.Core.IO;
using Stump.DofusProtocol.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stump.DbSynchronizer.Protocol.DLM
{
    public class Fixture
    {
        // FIELDS
        private Map m_map;
        public int fixtureId;
        public Point offset;
        public short rotation;
        public short xScale;
        public short yScale;
        public byte redMultiplier;
        public byte greenMultiplier;
        public byte blueMultiplier;
        public int hue;
        public byte alpha;
        // PROPERTIES

        // CONSTRUCTORS
        public Fixture(Map parent)
        {
            this.m_map = parent;
        }

        // METHODS
        public void FromRaw(IDataReader reader)
        {
            try
            {
                this.fixtureId = reader.ReadInt();
                this.offset = new Point();
                this.offset.x = reader.ReadShort();
                this.offset.y = reader.ReadShort();
                this.rotation = reader.ReadShort();
                this.xScale = reader.ReadShort();
                this.yScale = reader.ReadShort();
                this.redMultiplier = reader.ReadByte();
                this.greenMultiplier = reader.ReadByte();
                this.blueMultiplier = reader.ReadByte();
                this.hue = this.redMultiplier | this.greenMultiplier | this.blueMultiplier;
                this.alpha = reader.ReadByte();
            }
            catch (Exception e)
            {
                
                throw e;
            }
        }
    }
}
