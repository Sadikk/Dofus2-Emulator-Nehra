using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Stump.Core.Collections
{
    [Serializable]
    public class SerializableDictionary<TKey, TVal> : Dictionary<TKey, TVal>, IXmlSerializable, ISerializable
    {
        private const string ItemNodeName = "Item";
        private const string KeyNodeName = "Key";
        private const string ValueNodeName = "Value";
        private XmlSerializer keySerializer;
        private XmlSerializer valueSerializer;
        protected XmlSerializer ValueSerializer
        {
            get
            {
                if (this.valueSerializer == null)
                {
                    this.valueSerializer = new XmlSerializer(typeof(TVal));
                }
                return this.valueSerializer;
            }
        }
        private XmlSerializer KeySerializer
        {
            get
            {
                if (this.keySerializer == null)
                {
                    this.keySerializer = new XmlSerializer(typeof(TKey));
                }
                return this.keySerializer;
            }
        }

        public SerializableDictionary()
        {
        }

        public SerializableDictionary(IDictionary<TKey, TVal> dictionary)
            : base(dictionary)
        {
        }

        public SerializableDictionary(IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
        }

        public SerializableDictionary(int capacity)
            : base(capacity)
        {
        }

        public SerializableDictionary(IDictionary<TKey, TVal> dictionary, IEqualityComparer<TKey> comparer)
            : base(dictionary, comparer)
        {
        }

        public SerializableDictionary(int capacity, IEqualityComparer<TKey> comparer)
            : base(capacity, comparer)
        {
        }

        protected SerializableDictionary(SerializationInfo info, StreamingContext context)
        {
            int @int = info.GetInt32("ItemCount");
            for (int i = 0; i < @int; i++)
            {
                KeyValuePair<TKey, TVal> keyValuePair = (KeyValuePair<TKey, TVal>)info.GetValue(string.Format("Item{0}", i), typeof(KeyValuePair<TKey, TVal>));
                base.Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ItemCount", base.Count);
            int num = 0;
            foreach (KeyValuePair<TKey, TVal> current in this)
            {
                info.AddValue(string.Format("Item{0}", num), current, typeof(KeyValuePair<TKey, TVal>));
                num++;
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            foreach (KeyValuePair<TKey, TVal> current in this)
            {
                writer.WriteStartElement("Item");
                writer.WriteStartElement("Key");
                this.KeySerializer.Serialize(writer, current.Key);
                writer.WriteEndElement();
                writer.WriteStartElement("Value");
                this.ValueSerializer.Serialize(writer, current.Value);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (!reader.IsEmptyElement)
            {
                if (!reader.Read())
                {
                    throw new XmlException("Error in Deserialization of Dictionary");
                }
                while (reader.NodeType != XmlNodeType.EndElement)
                {
                    reader.ReadStartElement("Item");
                    reader.ReadStartElement("Key");
                    TKey key = (TKey)((object)this.KeySerializer.Deserialize(reader));
                    reader.ReadEndElement();
                    reader.ReadStartElement("Value");
                    TVal value = (TVal)((object)this.ValueSerializer.Deserialize(reader));
                    reader.ReadEndElement();
                    reader.ReadEndElement();
                    base.Add(key, value);
                    reader.MoveToContent();
                }
                reader.ReadEndElement();
            }
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }
    }
}