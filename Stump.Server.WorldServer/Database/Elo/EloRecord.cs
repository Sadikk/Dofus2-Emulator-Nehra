using Stump.ORM.SubSonic.SQLGeneration.Schema;

namespace Stump.Server.WorldServer.Database.Elo
{
    [TableName("elo_array")]
    public class EloRecord
    {
        public double Probability
        {
            get;
            set;
        }
        public short Difference
        {
            get;
            set;
        }
    }
}
