using Stump.Core.IO;

namespace Stump.DbSynchronizer.Protocol.Elements.Subtypes
{
    public class AnimatedGraphicalElementData : NormalGraphicalElementData
    {
        // FIELDS
        public uint minDelay;
        public uint maxDelay;

        // PROPERTIES

        // CONSTRUCTORS
        public AnimatedGraphicalElementData(int id, int type)
            : base(id, type)
        { }

        // METHODS
        public override void FromRaw(IDataReader reader, int param2)
        {
            base.FromRaw(reader, param2);
            if (param2 == 4)
            {
                this.minDelay = reader.ReadUInt();
                this.maxDelay = reader.ReadUInt();
            }
        }
    }
}
