using Stump.Core.IO;
using Stump.Core.Reflection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stump.DbSynchronizer.Protocol.GameData
{
    public class GameDataFileAccessor : Singleton<GameDataFileAccessor>
    {
        // FIELDS
        private Dictionary<string, Stream> m_streams;
        private Dictionary<string, int> m_streamStartIndex;
        private Dictionary<string, Dictionary<int, int>> m_indexes;
        private Dictionary<string, Dictionary<int, GameDataClassDefinition>> m_classes;
        private Dictionary<string, uint> m_counter;
        private Dictionary<string, string> m_gameDataProcessor;

        // PROPERTIES

        // CONSTRUCTORS
        public GameDataFileAccessor()
            : base()
        { }

        // METHODS
        public void Init(string path)
        {
            Stream stream;

            if (!File.Exists(path))
            {
                throw new Exception(string.Format("Game data file '{0}' not readable.", path));
            }

            var moduleName = Path.GetFileNameWithoutExtension(path);

            if (this.m_streams == null) this.m_streams = new Dictionary<string, Stream>();
            if (this.m_streamStartIndex == null) this.m_streamStartIndex = new Dictionary<string, int>();

            if (!this.m_streams.ContainsKey(moduleName))
            {
                this.m_streams.Add(moduleName, File.OpenRead(path));
                this.m_streamStartIndex.Add(moduleName, 7);
            }
            stream = this.m_streams[moduleName];
            stream.Position = 0;

            this.InitFromStream(stream, moduleName);
        }

        public void InitFromStream(Stream stream, string moduleName)
        {
            var reader = new BigEndianReader(stream);

            var count = 0u;
            var classIdentifier = 0;

            if (this.m_streams == null) this.m_streams = new Dictionary<string, Stream>();
            if (this.m_indexes == null) this.m_indexes = new Dictionary<string, Dictionary<int, int>>();
            if (this.m_classes == null) this.m_classes = new Dictionary<string, Dictionary<int, GameDataClassDefinition>>();
            if (this.m_counter == null) this.m_counter = new Dictionary<string, uint>();
            if (this.m_streamStartIndex == null) this.m_streamStartIndex = new Dictionary<string, int>();
            if (this.m_gameDataProcessor == null) this.m_gameDataProcessor = new Dictionary<string, string>();

            if (!this.m_streams.ContainsKey(moduleName))
            {
                this.m_streams.Add(moduleName, null);
            }
            this.m_streams[moduleName] = stream;
            if (!this.m_streamStartIndex.ContainsKey(moduleName))
            {
                this.m_streamStartIndex.Add(moduleName, 7);
            }
            var indexes = new Dictionary<int, int>();
            if (!this.m_indexes.ContainsKey(moduleName)) this.m_indexes.Add(moduleName, null);
            this.m_indexes[moduleName] = indexes;

            var contentOffset = 0;
            var headers = Encoding.ASCII.GetString(reader.ReadBytes(3));
            if (headers != "D2O")
            {
                reader.Seek(0, SeekOrigin.Begin);
                try
                {
                    headers = reader.ReadUTF();
                }
                catch { }

                if (headers != "AKSF")
                {
                    throw new Exception("Malformated game data file.");
                }
                else
                {
                    var formatVersion = reader.ReadShort();
                    var len = reader.ReadInt();
                    reader.Seek(len, SeekOrigin.Current);
                    contentOffset = (int)reader.Position;
                    this.m_streamStartIndex[moduleName] = contentOffset + 7;
                    headers = Encoding.ASCII.GetString(reader.ReadBytes(3));
                    if (headers != "D2O")
                    {
                        throw new Exception("Malformated game data file.");
                    }
                }
            }

            var indexesPointer = reader.ReadInt();
            stream.Position = contentOffset + indexesPointer;
            var indexesLength = reader.ReadInt();
            for (var i = 0; i < indexesLength; i += 8)
            {
                var key = reader.ReadInt();
                var pointer = reader.ReadInt();
                count++;

                indexes.Add(key, contentOffset + pointer);
            }

            if (this.m_counter.ContainsKey(moduleName)) this.m_counter.Add(moduleName, 0);
            this.m_counter[moduleName] = count;

            var classes = new Dictionary<int, GameDataClassDefinition>();
            if (this.m_classes.ContainsKey(moduleName)) this.m_classes.Add(moduleName, null);
            this.m_classes[moduleName] = classes;

            var classesCount = reader.ReadInt();
            for (var i = 0; i < classesCount; i++)
            {
                classIdentifier = reader.ReadInt();
                this.ReadClassDefinition(classIdentifier, reader, classes);
            }

            if (reader.BytesAvailable > 0)
            {
                // TODO
            }
        }

        //public GameDataProcess GetDataProcessor(string moduleName)
        //{
        //    if (this.m_gameDataProcessor.ContainsKey(moduleName))
        //    {
        //        return this.m_gameDataProcessor[moduleName];
        //    }

        //    return null;
        //}

        public GameDataClassDefinition GetClassDefinition(string moduleName, int classIdentifier)
        {
            return this.m_classes[moduleName][classIdentifier];
        }

        public uint GetCount(string moduleName)
        {
            if (this.m_counter.ContainsKey(moduleName))
            {
                return this.m_counter[moduleName];
            }

            return 0;
        }

        public object GetObject(string moduleName, int objectId)
        {
            if (this.m_indexes == null || !this.m_indexes.ContainsKey(moduleName))
            {
                return null;
            }

            if (!this.m_indexes.ContainsKey(moduleName) || !this.m_indexes[moduleName].ContainsKey(objectId))
            {
                return null;
            }

            var pointer = this.m_indexes[moduleName][objectId];

            var stream = new BigEndianReader(this.m_streams[moduleName]);
            stream.Seek(pointer, SeekOrigin.Begin);

            var classId = stream.ReadInt();

            return this.m_classes[moduleName][classId].Read(moduleName, stream);
        }
        public T GetObject<T>(string moduleName, int objectId)
        {
            return (T)this.GetObject(moduleName, objectId);
        }

        public object[] GetObjects(string moduleName)
        {
            if (this.m_counter == null || !this.m_counter.ContainsKey(moduleName))
            {
                return null;
            }

            var len = this.m_counter[moduleName];
            var classes = this.m_classes[moduleName];
            var stream = new BigEndianReader(this.m_streams[moduleName]);

            stream.Seek(this.m_streamStartIndex[moduleName], SeekOrigin.Begin);

            var objects = new object[len];
            for (var i = 0; i < len; i++)
            {
                objects[i] = classes[stream.ReadInt()].Read(moduleName, stream);
            }

            return objects;
        }
        public T[] GetObjects<T>(string moduleName)
        {
            return this.GetObjects(moduleName)
                .Select(entry => (T)entry)
                .ToArray();
        }

        public void Close()
        {
            foreach (var stream in this.m_streams)
            {
                try
                {
                    if (stream.Value is FileStream)
                    {
                        (stream.Value as FileStream).Close();
                    }
                }
                catch { continue; }
            }

            this.m_streams = null;
            this.m_indexes = null;
            this.m_classes = null;
        }

        private void ReadClassDefinition(int classIdentifier, IDataReader reader, Dictionary<int, GameDataClassDefinition> classes)
        {
            var className = reader.ReadUTF();
            var packageName = reader.ReadUTF();

            var classDef = new GameDataClassDefinition(packageName, className);
            var fieldsCount = reader.ReadInt();

            for (var i = 0; i < fieldsCount; i++)
            {
                var fieldName = reader.ReadUTF();
                classDef.AddField(fieldName, reader);
            }

            classes.Add(classIdentifier, classDef);
        }

        public void Foreach(Action<string> function)
        {
            foreach (var item in this.m_classes)
            {
                function(item.Key);
            }
        }
    }
}
