using Stump.Core.IO;

namespace Stump.DbSynchronizer.Protocol.Elements.Subtypes
{
    public class ParticlesGraphicalElementData : GraphicalElementData
    {
        // FIELDS
        public int scriptId;

        // PROPERTIES

        // CONSTRUCTORS
        public ParticlesGraphicalElementData(int id, int type)
            : base(id, type)
        { }

        // METHODS
        public override void FromRaw(IDataReader reader, int param2)
        {
            this.scriptId = reader.ReadShort();
        }
    }
}
