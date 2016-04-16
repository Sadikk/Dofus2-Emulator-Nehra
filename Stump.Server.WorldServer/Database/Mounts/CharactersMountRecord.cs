using Stump.ORM;
using Stump.ORM.SubSonic.SQLGeneration.Schema;

namespace Database.Mounts
{
    [TableName("characters_mount")]
    public class CharactersMountRecord
    {

        [PrimaryKey("Guid", false)]
        public int Guid { get; set; }
        public string Name { get; set; }
        public int MountId { get; set; }
        public bool IsCame { get; set; }
        public int OwnerId { get; set; }
        public bool IsRiding { get; set; }
        public bool IsEquiped { get; set; }
        [Ignore]
        public bool IsNew { get; set; }
        [Ignore]
        public bool IsUpdated { get; set; } 
    }
}
