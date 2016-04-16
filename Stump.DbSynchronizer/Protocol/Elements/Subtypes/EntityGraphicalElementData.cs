using Stump.Core.IO;

namespace Stump.DbSynchronizer.Protocol.Elements.Subtypes
{
    public class EntityGraphicalElementData : GraphicalElementData
    {
        // FIELDS
        public string entityLook;
        public bool horizontalSymmetry;
        public bool playAnimation;
        public bool playAnimStatic;
        public uint minDelay;
        public uint maxDelay;

        // PROPERTIES

        // CONSTRUCTORS
        public EntityGraphicalElementData(int id, int type)
            : base(id, type)
        { }

        // METHODS
        public override void FromRaw(IDataReader reader, int param2)
        {
            var loc3 = reader.ReadUInt();
            this.entityLook = reader.ReadUTFBytes((ushort)loc3);
            this.horizontalSymmetry = reader.ReadBoolean();
            if (param2 >= 7)
            {
                this.playAnimation = reader.ReadBoolean();
            }
            if (param2 >= 6)
            {
                this.playAnimStatic = reader.ReadBoolean();
            }
            if (param2 >= 5)
            {
                this.minDelay = reader.ReadUInt();
                this.maxDelay = reader.ReadUInt();
            }
        }
    }
}
