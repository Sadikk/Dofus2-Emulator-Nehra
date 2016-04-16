using Stump.Core.IO;
using Stump.Core.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Stump.DbSynchronizer.Protocol.GameData
{
    public class MissingClassException : Exception
    {
        public MissingClassException(string message)
            : base(message)
        {

        }
    }

    public class GameDataClassDefinition
    {
        // FIELDS
        private Type m_class;
        private Dictionary<GameDataField, FieldInfo> m_fieldsType;
        private List<GameDataField> m_fields;

        // PROPERTIES
        public IReadOnlyList<GameDataField> Fields { get { return this.m_fields.AsReadOnly(); } }

        // CONSTRUCTORS
        public GameDataClassDefinition(string packageName, string className)
        {
            this.m_class = Singleton<DataCenterManager>.Instance.GetClassByPath(string.Format("{0}.{1}", packageName, className));

            if (this.m_class == null)
            {
                throw new MissingClassException(string.Format("Missing class '{0}' ['{1}']", className, string.Format("{0}.{1}", packageName, className)));
            }

            this.m_fieldsType = new Dictionary<GameDataField, FieldInfo>();
            this.m_fields = new List<GameDataField>();
        }

        // METHODS
        public object Read(string module, IDataReader stream)
        {
            var instance = Activator.CreateInstance(this.m_class);

            foreach (var field in this.m_fields)
            {
                if (!this.m_fieldsType.ContainsKey(field))
                {
                    this.m_fieldsType.Add(field, this.m_class.GetField(field.Name));
                }

                var fieldInfo = this.m_fieldsType[field];
                var value = field.ReadData(module, stream, 0);

                if (value != null)
                {
                    if (fieldInfo == null)
                    {
                        throw new Exception(string.Format("Field name '{0}' type '{1}' missed in the Type '{2}'",
                            field.Name,
                            value.GetType().Name,
                            this.m_class.Name));
                    }

                    if (value is int && (int)value < 0 && fieldInfo.FieldType == typeof(uint))
                    {
                        fieldInfo.SetValue(instance, unchecked((uint)((int)value)));
                    }
                    else if (value.GetType() != fieldInfo.FieldType)
                    {
                        fieldInfo.SetValue(instance, Convert.ChangeType(value, fieldInfo.FieldType));
                    }
                    else
                    {
                        fieldInfo.SetValue(instance, value);
                    }
                }
            }
            // TODO 

            return instance;
        }

        public void AddField(string fieldName, IDataReader stream)
        {
            var field = new GameDataField(fieldName);
            field.ReadType(stream);

            this.m_fields.Add(field);
        }
    }
}
