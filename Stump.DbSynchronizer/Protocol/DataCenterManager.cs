using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Tools.D2o;
using System.Collections;
using System.Text.RegularExpressions;
using Stump.Server.WorldServer.Database;
using Stump.DofusProtocol.Classes;

namespace Stump.DbSynchronizer.Protocol
{
    public class DataCenterManager : Singleton<DataCenterManager>
    {
        // FIELDS
        private Dictionary<string, Type> m_dataCenterTypes;

        // PROPERTIES

        // CONSTRUCTORS
        private DataCenterManager()
            : base()
        {
            this.m_dataCenterTypes = new Dictionary<string, Type>();

            var types = from type in typeof(IDataObject).Assembly.GetTypes()
                        where type.IsClass && type.Namespace == @"Stump.DofusProtocol.Classes"
                        select type;

            foreach (var type in types)
            {
                //var attribute = (D2OClassAttribute)type.GetCustomAttribute(typeof(D2OClassAttribute), false);

                //var classPath = string.Format("{0}.{1}", attribute.PackageName, attribute.Name);

                if (this.m_dataCenterTypes.ContainsKey(type.Name))
                {
                    throw new Exception(string.Format("The classPath '{0}' is already added.", type.Name));
                }

                this.m_dataCenterTypes.Add(type.Name, type);
            }
        }

        // METHODS
        public Type GetClassByPath(string classPath)
        {
            Type type;

            classPath = classPath.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries).Last();

            this.m_dataCenterTypes.TryGetValue(classPath, out type);

            return type;
        }

        public string GetPathByClass(Type type)
        {
            var attribute = (D2OClassAttribute)type.GetCustomAttribute(typeof(D2OClassAttribute), false);
            if (attribute != null)
            {
                return string.Format("{0}.{1}", attribute.PackageName, attribute.Name);
            }

            return null;
        }

        public IList ReturnVectorInstance(string vectorTypeName)
        {
            var newType = "{0}";

            Match lastMatch = null;
            var match = Regex.Match(vectorTypeName, @"Vector\.<(?<genericParameter>.+)>");

            while (match.Success)
            {
                newType = string.Format(newType, "System.Collections.Generic.List`1[{0}]");

                lastMatch = match;
                match = Regex.Match(match.Groups["genericParameter"].Value, @"Vector\.<(?<genericParameter>.+)>");
            }

            var genericType = lastMatch.Groups["genericParameter"].Value;
            match = Regex.Match(genericType, @"(?<package>\w+[.\w]+[\w])::(?<class>\w+)");
            if (!match.Success)
            {
                switch (genericType)
                {
                    case "int":
                        genericType = "System.Int32";
                        break;

                    case "uint":
                        genericType = "System.UInt32";
                        break;

                    case "String":
                        genericType = "System.String";
                        break;

                    case "Number":
                        genericType = "System.Double";
                        break;

                    default:
                        break;
                }

                newType = string.Format(newType, genericType);
            }
            else
            {
                var type = this.GetClassByPath(match.Groups["class"].Value); //string.Format("{0}.{1}", match.Groups["package"].Value, match.Groups["class"].Value));
                if (type == null)
                {
                    throw new Exception("Malformated Vector type name.");
                }

                if (type.Assembly == typeof(IDataObject).Assembly)
                {
                    newType = string.Format(newType, string.Format("[{0}, Stump.DofusProtocol, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]", type.FullName));
                }
                else
                {
                    newType = string.Format(newType, type.FullName);
                }
            }

            var finalType = this.GetType(newType);
            if (finalType == null)
            {
                throw new Exception(string.Format("Can not create an instance of a type '{0}'", newType));
            }

            return (IList)Activator.CreateInstance(finalType);
        }

        private Type GetType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type == null)
            {
                foreach (var item in AppDomain.CurrentDomain.GetAssemblies())
                {
                    type = item.GetType(typeName);
                    if (type != null)
                    {
                        return type;
                    }
                }
            }

            return type;
        }
    }
}
