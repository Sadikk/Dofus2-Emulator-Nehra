using Stump.Core.IO;
using Stump.Core.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stump.DbSynchronizer.Protocol.GameData
{
    public class GameDataField
    {
        // FIELDS
        private const int NULL_IDENTIFIER = -1431655766;

        private string m_name;
        private Func<string, IDataReader, uint, object> m_readData;
        private List<Func<string, IDataReader, uint, object>> m_innerReadMethods;
        private List<string> m_innerTypeNames;

        // PROPERTIES
        public string Name { get { return this.m_name; } set { this.m_name = value; } }
        public List<GameDataTypeEnum> GameDataTypes { get; private set; }
        public Func<string, IDataReader, uint, object> ReadData { get { return this.m_readData; } }

        // CONSTRUCTORS
        public GameDataField(string fieldName)
        {
            this.m_name = fieldName;

            this.GameDataTypes = new List<GameDataTypeEnum>();
        }

        // METHODS
        public void ReadType(IDataReader stream)
        {
            var type = stream.ReadInt();
            this.m_readData = this.GetReadMethod(type, stream);
        }

        private Func<string, IDataReader, uint, object> GetReadMethod(int type, IDataReader stream)
        {
            var dataType = (GameDataTypeEnum)type;

            if (dataType != 0)
            {
                this.GameDataTypes.Add(dataType);
            }

            switch (dataType)
            {
                case GameDataTypeEnum.INT:
                    return this.ReadInteger;
                case GameDataTypeEnum.BOOLEAN:
                    return this.Readbool;
                case GameDataTypeEnum.STRING:
                    return this.Readstring;
                case GameDataTypeEnum.NUMBER:
                    return this.ReadNumber;
                case GameDataTypeEnum.I18N:
                    return this.ReadI18n;
                case GameDataTypeEnum.UINT:
                    return this.ReadUnsignedInteger;
                case GameDataTypeEnum.VECTOR:
                    if (this.m_innerReadMethods == null)
                    {
                        this.m_innerReadMethods = new List<Func<string, IDataReader, uint, object>>();
                        this.m_innerTypeNames = new List<string>();
                    }
                    this.m_innerTypeNames.Add(stream.ReadUTF());
                    this.m_innerReadMethods.Insert(0, this.GetReadMethod(stream.ReadInt(), stream));
                    return this.ReadVector;

                default:
                    if (type > 0)
                    {
                        return this.ReadObject;
                    }

                    throw new Exception(string.Format("Cannot handle the type '{0}'", type));
            }
        }

        private object ReadVector(string moduleName, IDataReader stream, uint innerIndex = 0)
        {
            var length = stream.ReadInt();
            var vectorTypeName = this.m_innerTypeNames[Math.Abs((int)innerIndex)];

            var vectorInstance = Singleton<DataCenterManager>.Instance.ReturnVectorInstance(vectorTypeName);

            var content = new object[length];
            for (var i = 0; i < length; i++)
            {
                vectorInstance.Add(this.m_innerReadMethods[(int)innerIndex](moduleName, stream, innerIndex + 1));
            }

            return vectorInstance;
        }
        private object ReadObject(string moduleName, IDataReader stream, uint innerIndex = 0)
        {
            var classIdentifier = stream.ReadInt();
            if (classIdentifier == NULL_IDENTIFIER)
            {
                return null;
            }

            var classDefinition = Singleton<GameDataFileAccessor>.Instance.GetClassDefinition(moduleName, classIdentifier);
            return classDefinition.Read(moduleName, stream);
        }
        private object ReadInteger(string moduleName, IDataReader stream, uint innerIndex = 0)
        {
            return stream.ReadInt();
        }
        private object Readbool(string moduleName, IDataReader stream, uint innerIndex = 0)
        {
            return stream.ReadBoolean();
        }
        private object Readstring(string moduleName, IDataReader stream, uint innerIndex = 0)
        {
            var result = stream.ReadUTF();
            if (result == "null")
            {
                return null;
            }

            return result;
        }
        private object ReadNumber(string moduleName, IDataReader stream, uint innerIndex = 0)
        {
            return stream.ReadDouble();
        }
        private object ReadI18n(string moduleName, IDataReader stream, uint innerIndex = 0)
        {
            return stream.ReadInt();
        }
        private object ReadUnsignedInteger(string moduleName, IDataReader stream, uint innerIndex = 0)
        {
            return stream.ReadUInt();
        }
    }
}
