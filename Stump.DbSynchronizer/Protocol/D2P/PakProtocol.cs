using Stump.Core.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stump.DbSynchronizer.Protocol.D2P
{
    public class PakProtocol
    {
        // FIELDS
        private static Dictionary<string, Dictionary<string, Tuple<int, int, IDataReader>>> _indexes = new Dictionary<string, Dictionary<string, Tuple<int, int, IDataReader>>>();
        private static Dictionary<string, Dictionary<string, string>> _properties = new Dictionary<string, Dictionary<string, string>>();

        // PROPERTIES

        // CONSTRUCTORS
        public PakProtocol()
        { }

        // METHODS
        public Dictionary<string, Tuple<int, int, IDataReader>> GetFilesIndex(string param1)
        {
            if (!PakProtocol._indexes.ContainsKey(param1))
            {
                if (this.InitStream(param1) == null)
                {
                    return null;
                }
            }

            return PakProtocol._indexes[param1];
        }
        public IReadOnlyDictionary<string, Dictionary<string, Tuple<int, int, IDataReader>>> GetIndexes()
        {
            return new ReadOnlyDictionary<string, Dictionary<string, Tuple<int, int, IDataReader>>>(PakProtocol._indexes);
        }

        public void Load(string param1)
        {
            if (!PakProtocol._indexes.ContainsKey(param1))
            {
                if (this.InitStream(param1) == null)
                {
                    throw new ArgumentNullException("FileStream");
                }
            }

            var index = PakProtocol._indexes[param1];
        }

        private FileStream InitStream(string param1)
        {
            var stream = File.OpenRead(param1);
            var reader = new BigEndianReader(stream);
            var loc7 = reader.ReadByte();
            var loc8 = reader.ReadByte();
            if (loc7 != 2 || loc8 != 1)
                return null;

            var loc4 = new Dictionary<string, Tuple<int, int, IDataReader>>();
            var loc5 = new Dictionary<string, string>();

            PakProtocol._indexes.Add(param1, loc4);
            PakProtocol._properties.Add(param1, loc5);

            reader.Seek((int)reader.BaseStream.Length - 24, SeekOrigin.Begin);
            var loc9 = reader.ReadUInt();
            var loc10 = reader.ReadUInt();
            var loc11 = reader.ReadUInt();
            var loc12 = reader.ReadUInt();
            var loc13 = reader.ReadUInt();
            var loc14 = reader.ReadUInt();
            reader.Seek((int)loc13, SeekOrigin.Begin);

            for (int i = 0; i < loc14; i++)
            {
                var loc15 = reader.ReadUTF();
                var loc16 = reader.ReadUTF();
                loc5.Add(loc15, loc16);

                if (loc15 == "link")
                {
                    this.InitStream(param1.Replace(Path.GetFileName(param1), loc16));
                }
            }

            reader.Seek((int)loc11, SeekOrigin.Begin);
            for (int i = 0; i < loc12; i++)
            {
                var loc18 = reader.ReadUTF();
                var loc19 = reader.ReadInt();
                var loc20 = reader.ReadInt();

                loc4.Add(loc18, new Tuple<int, int, IDataReader>((int)(loc19 + loc9), loc20, reader));
            }

            return stream;
        }
    }
}
