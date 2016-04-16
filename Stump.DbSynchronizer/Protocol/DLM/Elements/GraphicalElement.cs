using Stump.Core.IO;
using Stump.DofusProtocol.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stump.DbSynchronizer.Protocol.DLM.Elements
{
    public class ColorMultiplicator
    {
        // FIELDS
        public double red;
        public double green;
        public double blue;
        private bool m_isOne;

        // PROPERTIES
        
        // CONSTRUCTORS
        public ColorMultiplicator(int param1, int param2, int param3, bool param4 = false)
        {
            this.red = param1;
            this.green = param2;
            this.blue = param3;
            if (!param4 && param1 + param2 + param3 == 0)
            {
                this.m_isOne = true;
            }
        }

        // METHODS
    }

    public class GraphicalElement : BasicElement
    {
        // FIELDS
        public const double CELL_WIDTH = 86;
        public const double CELL_HALF_WIDTH = 43;
        public const double CELL_HEIGHT = 43;
        public const double CELL_HALF_HEIGHT = 21.5;

        public uint elementId;
        public ColorMultiplicator hue;
        public ColorMultiplicator shadow;
        public Point offset;
        public Point pixelOffset;
        public byte altitude;
        public uint identifier;

        // PROPERTIES
        public override int ElementType
        {
            get
            {
                return (int)ElementTypesEnum.GRAPHICAL;
            }
        }

        // CONSTRUCTORS
        public GraphicalElement(Cell parent)
            : base(parent)
        { }

        // METHODS
        private void CalculateFinalTeint()
        {

        }

        public override void FromRaw(IDataReader reader, int mapVersion)
        {
            try
            {
                this.elementId = reader.ReadUInt();
                this.hue = new ColorMultiplicator(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                this.shadow = new ColorMultiplicator(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                this.offset = new Point();
                this.pixelOffset = new Point();
                if (mapVersion <= 4)
                {
                    this.offset.x = reader.ReadByte();
                    this.offset.y = reader.ReadByte();
                    this.pixelOffset.x = (int)(this.offset.x * GraphicalElement.CELL_HALF_WIDTH);
                    this.pixelOffset.y = (int)(this.offset.y * GraphicalElement.CELL_HALF_HEIGHT);
                }
                else
                {
                    this.pixelOffset.x = reader.ReadShort();
                    this.pixelOffset.y = reader.ReadShort();
                    this.offset.x = (int)(this.pixelOffset.x / GraphicalElement.CELL_HALF_WIDTH);
                    this.offset.y = (int)(this.pixelOffset.y / GraphicalElement.CELL_HALF_HEIGHT);
                }
                this.altitude = reader.ReadByte();
                this.identifier = reader.ReadUInt();
                this.CalculateFinalTeint();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
