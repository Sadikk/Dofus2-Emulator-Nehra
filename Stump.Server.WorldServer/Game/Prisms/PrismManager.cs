using Stump.Core.Pool;
using Stump.Core.Reflection;
using Stump.Server.BaseServer;
using Stump.Server.BaseServer.Database;
using Stump.Server.BaseServer.Initialization;
using Stump.Server.WorldServer.Database;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using System.Collections.Generic;
using System.Linq;

namespace Stump.Server.WorldServer.Game.Prisms
{
    public class PrismManager : DataManager<PrismManager>, ISaveable
    {
        // FIELDS
        private UniqueIdProvider m_idProvider;
        private Dictionary<int, WorldMapPrismRecord> m_prismSpawns;
        private readonly List<PrismNpc> m_activePrisms = new List<PrismNpc>();

        // PROPERTIES

        // CONSTRUCTORS

        // METHODS
        [Initialization(InitializationPass.Eighth)]
        public override void Initialize()
        {
            this.m_prismSpawns = base.Database.Query<WorldMapPrismRecord>(WorldMapPrismRelator.FetchQuery, new object[0])
                .ToDictionary(entry => entry.Id);

            if (this.m_prismSpawns.Any())
            {
                this.m_idProvider = new UniqueIdProvider((from item in this.m_prismSpawns select item.Value.Id).Max());
            }
            else
            {
                this.m_idProvider = UniqueIdProvider.Default;
            }

            Singleton<World>.Instance.RegisterSaveableInstance(this);
            Singleton<World>.Instance.SpawnPrisms();
        }

        public WorldMapPrismRecord[] GetPrismSpawns()
        {
            return this.m_prismSpawns.Values.ToArray<WorldMapPrismRecord>();
        }
        public WorldMapPrismRecord[] GetPrismSpawns(int allianceId)
        {
            return (
                from entry in this.m_prismSpawns.Values
                where entry.AllianceId == allianceId
                select entry).ToArray<WorldMapPrismRecord>();
        }

        public bool TryAddPrism(Character character, bool lazySave = true)
        {
            bool result;
            if (true)
            {
                var position = character.Position.Clone();
                var npc = new PrismNpc(this.m_idProvider.Pop(), position.Map.GetNextContextualId(), position, character.Guild.Alliance);
                if (lazySave)
                {
                    ServerBase<WorldServer>.Instance.IOTaskPool.AddMessage(delegate
                    {
                        this.Database.Insert(npc.Record);
                    });
                }
                else
                {
                    base.Database.Insert(npc.Record);
                }
                this.m_prismSpawns.Add(npc.GlobalId, npc.Record);
                this.m_activePrisms.Add(npc);
                npc.Map.Enter(npc);
                character.Guild.Alliance.AddPrism(npc);

                result = true;
            }
            return result;
        }

        public void Save()
        {

        }
    }
}
