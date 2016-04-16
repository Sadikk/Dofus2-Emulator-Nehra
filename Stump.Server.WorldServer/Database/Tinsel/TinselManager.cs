using Stump.Server.BaseServer.Database;
using Stump.Server.BaseServer.Initialization;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Stump.Server.WorldServer.Database.Tinsel
{
    public class TinselManager : DataManager<TinselManager>
    {
        private System.Collections.Generic.Dictionary<ushort, TitleRecord> m_titles;
        private System.Collections.Generic.Dictionary<ushort, OrnamentRecord> m_ornaments;

        public IReadOnlyDictionary<ushort, TitleRecord> Titles
        {
            get
            {
                return new ReadOnlyDictionary<ushort, TitleRecord>(this.m_titles);
            }
        }
        public IReadOnlyDictionary<ushort, OrnamentRecord> Ornaments
        {
            get
            {
                return new ReadOnlyDictionary<ushort, OrnamentRecord>(this.m_ornaments);
            }
        }

        [Initialization(InitializationPass.Fourth)]
        public override void Initialize()
        {
            this.m_titles = base.Database.Fetch<TitleRecord>(TitleRelator.FetchQuery, new object[0]).ToDictionary((TitleRecord entry) => (ushort)entry.Id);
            this.m_ornaments = base.Database.Fetch<OrnamentRecord>(OrnamentRelator.FetchQuery, new object[0]).ToDictionary((OrnamentRecord entry) => (ushort)entry.Id);
        }
    }
}
