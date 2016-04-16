using Stump.Core.IO;
using Stump.DofusProtocol.Classes;

namespace Stump.DbSynchronizer.Protocol.Elements.Subtypes
{
    public class NormalGraphicalElementData : GraphicalElementData
    {
        // FIELDS
        public int gfxId;
        public uint height;
        public bool horizontalSymmetry;
        public Point origin;
        public Point size;

        // PROPERTIES

        // CONSTRUCTORS
        public NormalGraphicalElementData(int id, int type)
            : base(id, type)
        { }

        // METHODS
        public override void FromRaw(IDataReader reader, int param2)
        {
            this.gfxId = reader.ReadInt();
            this.height = reader.ReadByte();
            this.horizontalSymmetry = reader.ReadBoolean();
            this.origin = new Point { x = reader.ReadShort(), y = reader.ReadShort() };
            this.size = new Point { x = reader.ReadShort(), y = reader.ReadShort() };
        }
    }
}
