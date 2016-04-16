using Stump.Core.Pool;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer;
using Stump.Server.BaseServer.Database;
using Stump.Server.BaseServer.Initialization;
using Stump.Server.WorldServer.Database;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Maps.Cells;
using Stump.Server.WorldServer.Handlers.TaxCollector;
using System.Linq;

namespace Stump.Server.WorldServer.Game.Actors.RolePlay.TaxCollectors
{
	public class TaxCollectorManager : DataManager<TaxCollectorManager>, ISaveable
	{
        // FIELDS
		private UniqueIdProvider m_idProvider;
		private System.Collections.Generic.Dictionary<int, WorldMapTaxCollectorRecord> m_taxCollectorSpawns;
		private readonly System.Collections.Generic.List<TaxCollectorNpc> m_activeTaxCollectors = new System.Collections.Generic.List<TaxCollectorNpc>();

        // PROPERTIES

        // CONSTRUCTORS

        // METHODS
		[Initialization(InitializationPass.Eighth)]
		public override void Initialize()
		{
			this.m_taxCollectorSpawns = base.Database.Query<WorldMapTaxCollectorRecord>(WorldMapTaxCollectorRelator.FetchQuery, new object[0])
                .ToDictionary(entry => entry.Id);

            if (this.m_taxCollectorSpawns.Any())
            {
                this.m_idProvider = new UniqueIdProvider((from item in this.m_taxCollectorSpawns select item.Value.Id).Max());
            }
            else
            {
                this.m_idProvider = UniqueIdProvider.Default;
            }

			Singleton<World>.Instance.RegisterSaveableInstance(this);
			Singleton<World>.Instance.SpawnTaxCollectors();
		}

		public WorldMapTaxCollectorRecord[] GetTaxCollectorSpawns()
		{
			return this.m_taxCollectorSpawns.Values.ToArray<WorldMapTaxCollectorRecord>();
		}
		public WorldMapTaxCollectorRecord[] GetTaxCollectorSpawns(int guildId)
		{
			return (
				from x in this.m_taxCollectorSpawns.Values
				where x.GuildId == guildId
				select x).ToArray<WorldMapTaxCollectorRecord>();
		}

        public bool TryAddTaxCollectorSpawn(Character character, bool lazySave = true)
        {
            bool result;
            if (!character.GuildMember.HasRight(GuildRightsBitEnum.GUILD_RIGHT_HIRE_TAX_COLLECTOR))
            {
                TaxCollectorHandler.SendTaxCollectorErrorMessage(character.Client, TaxCollectorErrorReasonEnum.TAX_COLLECTOR_NO_RIGHTS);
                result = false;
            }
            else
            {
                if (character.Guild.TaxCollectors.Count<TaxCollectorNpc>() >= character.Guild.MaxTaxCollectors)
                {
                    TaxCollectorHandler.SendTaxCollectorErrorMessage(character.Client, TaxCollectorErrorReasonEnum.TAX_COLLECTOR_MAX_REACHED);
                    result = false;
                }
                else
                {
                    if (character.Position.Map.TaxCollector != null)
                    {
                        TaxCollectorHandler.SendTaxCollectorErrorMessage(character.Client, TaxCollectorErrorReasonEnum.TAX_COLLECTOR_ALREADY_ONE);
                        result = false;
                    }
                    else
                    {
                        if (!character.Position.Map.AllowCollector)
                        {
                            TaxCollectorHandler.SendTaxCollectorErrorMessage(character.Client, TaxCollectorErrorReasonEnum.TAX_COLLECTOR_CANT_HIRE_HERE);
                            result = false;
                        }
                        else
                        {
                            if (character.IsInFight())
                            {
                                TaxCollectorHandler.SendTaxCollectorErrorMessage(character.Client, TaxCollectorErrorReasonEnum.TAX_COLLECTOR_ERROR_UNKNOWN);
                                result = false;
                            }
                            else
                            {
                                //var basePlayerItem = character.Inventory.TryGetItem(Singleton<ItemManager>.Instance.TryGetTemplate(AllianceManager.ALLIOGEMME_ID));
                                //character.Inventory.RemoveItem(basePlayerItem, 1, true);

                                ObjectPosition objectPosition = character.Position.Clone();
                                TaxCollectorNpc taxCollectorNpc = new TaxCollectorNpc(this.m_idProvider.Pop(), objectPosition.Map.GetNextContextualId(), objectPosition, character.Guild, character.Name);
                                if (lazySave)
                                {
                                    ServerBase<WorldServer>.Instance.IOTaskPool.AddMessage(delegate
                                    {
                                        this.Database.Insert(taxCollectorNpc.Record);
                                    });
                                }
                                else
                                {
                                    base.Database.Insert(taxCollectorNpc.Record);
                                }
                                this.m_taxCollectorSpawns.Add(taxCollectorNpc.GlobalId, taxCollectorNpc.Record);
                                this.m_activeTaxCollectors.Add(taxCollectorNpc);
                                taxCollectorNpc.Map.Enter(taxCollectorNpc);
                                character.Guild.AddTaxCollector(taxCollectorNpc);
                                
                                result = true;
                            }
                        }
                    }
                }
            }
            return result;
        }

		public void RemoveTaxCollectorSpawn(TaxCollectorNpc taxCollector, bool lazySave = true)
		{
			if (lazySave)
			{
				ServerBase<WorldServer>.Instance.IOTaskPool.AddMessage(delegate
				{
					this.Database.Delete(taxCollector.Record);
				});
			}
			else
			{
				base.Database.Delete(taxCollector.Record);
			}
			taxCollector.Bag.DeleteBag(lazySave);
			this.m_taxCollectorSpawns.Remove(taxCollector.GlobalId);
			this.m_activeTaxCollectors.Remove(taxCollector);
		}

		public void Save()
		{
			foreach (TaxCollectorNpc current in 
				from taxCollector in this.m_activeTaxCollectors
				where taxCollector.IsRecordDirty
				select taxCollector)
			{
				current.Save();
			}
		}
    }
}
