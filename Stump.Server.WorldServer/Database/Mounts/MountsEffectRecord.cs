using Stump.ORM.SubSonic.SQLGeneration.Schema;
using System.Collections.Generic;
using Stump.DofusProtocol.Types;

namespace Database.Mounts
{
    [TableName("mounts_effect")]
    class MountsEffectRelation
    {
        public int Id { get; set; }
        public int MountId { get; set; }
        public string FormatedEffects { get; set; }

        public List<ObjectEffectInteger> Effects = new List<ObjectEffectInteger>();
    }
}