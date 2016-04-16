using Stump.Core.IO;
using Stump.Core.Reflection;
using System;
using System.Collections.Generic;
using System.IO;

namespace Stump.DbSynchronizer.Protocol.Elements
{
    public class Element : Singleton<Element>
    {
        // FIELDS
        private IDataReader m_reader;
        public byte fileVersion;
        public uint elementsCount;
        private Dictionary<int, GraphicalElementData> m_elementsMap;
        private Dictionary<int, long> m_elementsIndex;
        private Dictionary<int, bool> m_jpgMap;
        private bool m_parsed;
        private bool m_failed;

        // PROPERTIES

        // CONSTRUCTORS
        private Element()
        {
            this.m_elementsMap = new Dictionary<int, GraphicalElementData>();
            this.m_elementsIndex = new Dictionary<int, long>();
        }

        // METHODS
        public void FromRaw(IDataReader reader)
        {
            uint skypLen = 0;

            try
            {
                var header = reader.ReadByte();
                if (header != 69)
                {
                    throw new Exception("Unknown file format");
                }
                else
                {
                    this.m_reader = reader;
                    this.fileVersion = reader.ReadByte();
                    this.elementsCount = reader.ReadUInt();
                    for (int i = 0; i < this.elementsCount; i++)
                    {
                        if (this.fileVersion >= 9)
                        {
                            skypLen = reader.ReadUShort();
                        }
                        var edId = reader.ReadInt();
                        if (this.fileVersion <= 8)
                        {
                            this.m_elementsIndex[edId] = reader.Position;
                            this.ReadElement(edId);
                        }
                        else
                        {
                            this.m_elementsIndex[edId] = reader.Position;
                            reader.Seek((int)skypLen - 4, SeekOrigin.Current);
                        }
                    }
                    if (this.fileVersion >= 8)
                    {
                        var gfxCount = reader.ReadInt();
                        this.m_jpgMap = new Dictionary<int, bool>();
                        for (int i = 0; i < gfxCount; i++)
                        {
                            var gfxId = reader.ReadInt();
                            this.m_jpgMap[gfxId] = true;
                        }
                    }
                }
                this.m_parsed = true;
            }
            catch (Exception e)
            {
                this.m_failed = true;
                throw e;
            }
        }

        public GraphicalElementData GetElementData(int param1)
        {
            return this.m_elementsMap.ContainsKey(param1) ? this.m_elementsMap[param1] : this.ReadElement(param1);
        }

        private GraphicalElementData ReadElement(int param1)
        {
            this.m_reader.Seek((int)this.m_elementsIndex[param1], SeekOrigin.Begin);
            var loc2 = this.m_reader.ReadByte();
            var loc3 = GraphicalElementFactory.GetGraphicalElementData(param1, loc2);
            if (loc3 == null)
            {
                return null;
            }
            loc3.FromRaw(this.m_reader, this.fileVersion);
            this.m_elementsMap[param1] = loc3;

            return loc3;
        }
    }
}
