using Stump.Core.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stump.DbSynchronizer.Protocol.DLM
{
    public class Layer
    {
        // FIELDS
        public const uint LAYER_GROUND = 0;
        public const uint LAYER_ADDITIONAL_GROUND = 1;
        public const uint LAYER_DECOR = 2;
        public const uint LAYER_ADDITIONAL_DECOR = 3;
    
        // PROPERTIES
        public int layerId;
        public int refCell = 0;
        public short cellsCount;
        public List<Cell> cells;
        private Map m_map;

        // CONSTRUCTORS
        public Layer(Map parent)
        {
            this.m_map = parent;
        }

        // METHODS
        public void FromRaw(IDataReader reader, int mapVersion)
        {
            try
            {
                this.layerId = reader.ReadInt();
                this.cellsCount = reader.ReadShort();
                this.cells = new List<Cell>();
                for (int i = 0; i < this.cellsCount; i++)
                {
                    var c = new Cell(this);
                    c.FromRaw(reader, mapVersion);
                    this.cells.Add(c);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
