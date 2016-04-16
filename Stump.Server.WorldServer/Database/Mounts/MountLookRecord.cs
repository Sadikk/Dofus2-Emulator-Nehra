using Stump.ORM.SubSonic.SQLGeneration.Schema;

namespace Database.Mounts
{
    [TableName("mounts_look")]
    class MountLookRecord
    {
        public int Id { get; set; }
        public int NameId { get; set; }
        public string Look { get; set; }
        public int MountId { get; set; }
    }
}
