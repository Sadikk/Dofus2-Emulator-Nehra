using Stump.Core.IO;

namespace Stump.DbSynchronizer.Protocol.Elements
{
    public abstract class GraphicalElementData
    {
        // FIELDS
        public int id;
        public int type;

        // PROPERTIES

        // CONSTRUCTORS
        public GraphicalElementData(int id, int type)
        {
            this.id = id;
            this.type = type;
        }

        // METHODS
        public abstract void FromRaw(IDataReader reader, int param2);
    }
}
