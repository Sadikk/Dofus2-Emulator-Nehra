using Stump.Core.IO;

namespace Stump.DbSynchronizer.Protocol.Elements.Subtypes
{
    public class BlendedGraphicalElementData : NormalGraphicalElementData
    {
        // FIELDS
        public string blenMode;

        // PROPERTIES

        // CONSTRUCTORS
        public BlendedGraphicalElementData(int id, int type)
            : base(id, type)
        { }

        // METHODS
        public override void FromRaw(IDataReader reader, int param2)
        {
            base.FromRaw(reader, param2);
            var loc3 = reader.ReadUInt();
            this.blenMode = reader.ReadUTFBytes((ushort)loc3);
        }
    }
}
