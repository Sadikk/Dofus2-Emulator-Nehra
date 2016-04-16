using System;

namespace Stump.DofusProtocol.Tools.D2o
{
    public class D2OClassAttribute : Attribute
    {
        public string Name
        {
            get;
            set;
        }
        public string PackageName
        {
            get;
            set;
        }
        public bool AutoBuild
        {
            get;
            set;
        }
        public D2OClassAttribute(string name, bool autoBuild = true)
        {
            this.Name = name;
            this.PackageName = null;
            this.AutoBuild = autoBuild;
        }
        public D2OClassAttribute(string name, string packageName, bool autoBuild = true)
        {
            this.Name = name;
            this.PackageName = packageName;
            this.AutoBuild = autoBuild;
        }
    }
}