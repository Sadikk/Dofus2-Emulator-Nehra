using Stump.Core.IO;
using Stump.Core.Reflection;
using Stump.DbSynchronizer.Protocol.DLM.Elements;
using Stump.DbSynchronizer.Protocol.Elements;
using Stump.DbSynchronizer.Protocol.Elements.Subtypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stump.DbSynchronizer.Protocol.DLM
{
    public class Map
    {
        // FIELDS
        public const string DESCRIPTION_KEY = "649ae451ca33ec53bbcbcc33becf15f4";
        public const int MAP_CELLS_COUNT = 560;

        public byte mapVersion;
        public uint id;
        public uint relativeId;
        public bool encrypted;
        public byte encryptionVersion;
        public byte mapType;
        public int subareaId;
        public int topNeighbourId;
        public int bottomNeighbourId;
        public int leftNeighbourId;
        public int rightNeighbourId;
        public int shadowBonusOnEntities;
        public byte backgroundRed;
        public byte backgroundGreen;
        public byte backgroundBlue;
        public int backgroundColor;
        public int zoomScale;
        public short zoomOffsetX;
        public short zoomOffsetY;
        public bool useLowPassFilter;
        public bool useReverb;
        public int presetId;
        public byte backgroundsCount;
        public List<Fixture> backgroundFixtures;
        public byte foregroundsCount;
        public List<Fixture> foregroundFixtures;
        public int cellsCount;
        public int groundCRC;
        public byte layersCount;
        public List<Layer> layers;
        public List<CellData> cells;
        public List<int> topArrowCell;
        public List<int> leftArrowCell;
        public List<int> bottomArrowCell;
        public List<int> rightArrowCell;
        public bool isUsingNewMovementSystem;

        private Dictionary<int, GraphicalElementData> m_gfxList;
        private Dictionary<int, int> m_gfxCount;
        private bool m_parsed;
        private bool m_failed;
        
        // PROPERTIES

        // CONSTRUCTORS
        public Map()
        {
            this.topArrowCell = new List<int>();
            this.leftArrowCell = new List<int>();
            this.bottomArrowCell = new List<int>();
            this.rightArrowCell = new List<int>();
        }

        // METHODS        
        public Dictionary<int, GraphicalElementData> GetGfxList(bool param1 = false)
        {
            if (this.m_gfxList == null)
            {
                this.ComputeGfxList(param1);
            }
            return this.m_gfxList;
        }

        public void FromRaw(IDataReader reader)
        {
            uint _oldMvtSystem = 0;
            try
            {
                reader.Seek(0, SeekOrigin.Begin);

                var header = reader.ReadByte();
                if (header != 77)
                {
                    throw new Exception("Unknown file format");
                }
                this.mapVersion = reader.ReadByte();
                this.id = reader.ReadUInt();
                if (this.mapVersion >= 7)
                {
                    this.encrypted = reader.ReadBoolean();
                    this.encryptionVersion = reader.ReadByte();
                    var dataLen = reader.ReadInt();
                    if (this.encrypted)
                    {
                        var encryptedData = reader.ReadBytes(dataLen);
                        for (int i = 0; i < encryptedData.Length; i++)
                        {
                            encryptedData[i] = (byte)(encryptedData[i] ^ DESCRIPTION_KEY[i % DESCRIPTION_KEY.Length]);
                        }
                        reader = new BigEndianReader(encryptedData);
                    }
                }
                this.relativeId = reader.ReadUInt();
                this.mapType = reader.ReadByte();
                this.subareaId = reader.ReadInt();
                this.topNeighbourId = reader.ReadInt();
                this.bottomNeighbourId = reader.ReadInt();
                this.leftNeighbourId = reader.ReadInt();
                this.rightNeighbourId = reader.ReadInt();
                this.shadowBonusOnEntities = reader.ReadInt();
                if (this.mapVersion >= 3)
                {
                    this.backgroundRed = reader.ReadByte();
                    this.backgroundGreen = reader.ReadByte();
                    this.backgroundBlue = reader.ReadByte(); 
                    this.backgroundColor = (this.backgroundRed & 255) << 16 | (this.backgroundGreen & 255) << 8 | this.backgroundBlue & 255;
                }
                if (this.mapVersion >= 4)
                {
                    this.zoomScale = reader.ReadUShort() / 100;
                    this.zoomOffsetX = reader.ReadShort();
                    this.zoomOffsetY = reader.ReadShort();
                    if (this.zoomScale < 1)
                    {
                        this.zoomScale = 1;
                        this.zoomOffsetY = 0;
                        this.zoomOffsetX = 0;
                    }
                }
                this.useLowPassFilter = reader.ReadBoolean();
                this.useReverb = reader.ReadBoolean();
                if (this.useReverb)
                {
                    this.presetId = reader.ReadInt();
                }
                else
                {
                    this.presetId = -1;
                }
                this.backgroundsCount = reader.ReadByte();
                this.backgroundFixtures = new List<Fixture>();
                for (int i = 0; i < this.backgroundsCount; i++)
                {
                    var bg = new Fixture(this);
                    bg.FromRaw(reader);
                    this.backgroundFixtures.Add(bg);
                }
                this.foregroundsCount = reader.ReadByte();
                this.foregroundFixtures = new List<Fixture>();
                for (int i = 0; i < this.foregroundsCount; i++)
                {
                    var fg = new Fixture(this);
                    fg.FromRaw(reader);
                    this.foregroundFixtures.Add(fg);
                }
                this.cellsCount = MAP_CELLS_COUNT;
                reader.ReadInt();
                this.groundCRC = reader.ReadInt();
                this.layersCount = reader.ReadByte();
                this.layers = new List<Layer>();
                for (int i = 0; i < this.layersCount; i++)
                {
                    var la = new Layer(this);
                    la.FromRaw(reader, this.mapVersion);
                    this.layers.Add(la);
                }
                this.cells = new List<CellData>();
                for (int i = 0; i < this.cellsCount; i++)
                {
                    var cd = new CellData(this, i);
                    cd.FromRaw(reader);
                    if (_oldMvtSystem == 0)
                    {
                        _oldMvtSystem = cd.moveZone;
                    }
                    if (cd.moveZone != _oldMvtSystem)
                    {
                        this.isUsingNewMovementSystem = true;
                    }

                    this.cells.Add(cd);
                }

                this.m_parsed = true;
                return;
            }
            catch (Exception e)
            {
                this.m_failed = true;
                throw e;
            }
        }

        private void ComputeGfxList(bool param1 = false)
        {
            var graphicalElements = new Dictionary<int, GraphicalElementData>();

            this.m_gfxCount = new Dictionary<int, int>();
            for (int i = 0; i < this.layers.Count; i++)
            {
                var layer = this.layers[i];
                if (!(param1 && i == 0))
                {
                    var cells = layer.cells;
                    for (int j = 0; j < cells.Count; j++)
                    {
                        var cell = cells[j];
                        var elements = cell.elements;
                        for (int k = 0; k < elements.Count; k++)
                        {
                            var element = elements[k];
                            if (element.ElementType == (int)ElementTypesEnum.GRAPHICAL)
                            {
                                var loc15 = ((GraphicalElement)element).elementId;
                                var loc16 = Singleton<Protocol.Elements.Element>.Instance.GetElementData((int)loc15);
                                if (loc16 == null)
                                {
                                    // log
                                }
                                else if (loc16 is NormalGraphicalElementData)
                                {
                                    var loc17 = loc16 as NormalGraphicalElementData;
                                    graphicalElements[loc17.gfxId] = loc17;
                                    if (this.m_gfxCount.ContainsKey(loc17.gfxId))
                                    {
                                        this.m_gfxCount[loc17.gfxId]++;
                                    }
                                    else
                                    {
                                        this.m_gfxCount[loc17.gfxId] = 1;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            this.m_gfxList = new Dictionary<int, GraphicalElementData>();
            foreach (var item in graphicalElements)
            {
                this.m_gfxList.Add(item.Key, item.Value);
            }
        }
    }
}
