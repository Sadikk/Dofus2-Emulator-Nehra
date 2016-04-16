using Stump.Core.IO;
using Stump.DbSynchronizer.Protocol.DLM.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stump.DbSynchronizer.Protocol.DLM
{
    public class Cell
    {
        // FIELDS
        public short cellId;
        public short elementsCount;
        public List<BasicElement> elements;
        private Layer m_layer;

        // PROPERTIES

        // CONSTRUCTORS
        public Cell(Layer parent)
        {
            this.m_layer = parent;

        }

        // METHODS
        public void FromRaw(IDataReader reader, int mapVersion)
        {
            try
            {
                this.cellId = reader.ReadShort();
                this.elementsCount = reader.ReadShort();
                this.elements = new List<BasicElement>();
                for (int i = 0; i < this.elementsCount; i++)
                {
                    var be = BasicElement.GetElementFromType(reader.ReadByte(), this);
                    be.FromRaw(reader, mapVersion);
                    this.elements.Add(be);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
