using Stump.Core.Reflection;
using Stump.Server.BaseServer;
using Stump.Server.BaseServer.Database;
using Stump.Server.BaseServer.Initialization;
using Stump.Server.WorldServer.Database;
using Stump.Server.WorldServer.Database.Accounts;
using Stump.Server.WorldServer.Database.World;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Actors.RolePlay.Merchants
{
	public class MerchantManager : DataManager<MerchantManager>, ISaveable
	{
		private System.Collections.Generic.Dictionary<int, WorldMapMerchantRecord> m_merchantSpawns;
		private readonly System.Collections.Generic.List<Merchant> m_activeMerchants = new System.Collections.Generic.List<Merchant>();
		public System.Collections.ObjectModel.ReadOnlyCollection<Merchant> Merchants
		{
			get
			{
				return this.m_activeMerchants.AsReadOnly();
			}
		}
		[Initialization(InitializationPass.Sixth)]
		public override void Initialize()
		{
			this.m_merchantSpawns = base.Database.Query<WorldMapMerchantRecord>(WorldMapMerchantRelator.FetchQuery, new object[0]).ToDictionary((WorldMapMerchantRecord entry) => entry.CharacterId);
			Singleton<World>.Instance.RegisterSaveableInstance(this);
		}
		public WorldMapMerchantRecord[] GetMerchantSpawns()
		{
			return this.m_merchantSpawns.Values.ToArray<WorldMapMerchantRecord>();
		}
		public WorldMapMerchantRecord GetMerchantSpawn(int characterId)
		{
			WorldMapMerchantRecord worldMapMerchantRecord;
			return this.m_merchantSpawns.TryGetValue(characterId, out worldMapMerchantRecord) ? worldMapMerchantRecord : null;
		}
		public void AddMerchantSpawn(WorldMapMerchantRecord spawn, bool lazySave = true)
		{
			if (lazySave)
			{
				ServerBase<WorldServer>.Instance.IOTaskPool.AddMessage(delegate
				{
					this.Database.Insert(spawn);
				});
			}
			else
			{
				base.Database.Insert(spawn);
			}
			this.m_merchantSpawns.Add(spawn.CharacterId, spawn);
		}
		public void RemoveMerchantSpawn(WorldMapMerchantRecord spawn, bool lazySave = true)
		{
			if (lazySave)
			{
				ServerBase<WorldServer>.Instance.IOTaskPool.AddMessage(delegate
				{
					this.Database.Delete(spawn);
				});
			}
			else
			{
				base.Database.Delete(spawn);
			}
			this.m_merchantSpawns.Remove(spawn.CharacterId);
		}
		public void ActiveMerchant(Merchant merchant)
		{
			this.m_activeMerchants.Add(merchant);
		}
		public void UnActiveMerchant(Merchant merchant)
		{
			merchant.Delete();
			this.m_activeMerchants.Remove(merchant);
		}
		public System.Collections.Generic.IEnumerable<Merchant> UnActiveMerchantFromAccount(WorldAccount account)
		{
            var merchants = m_activeMerchants.Where(entry => entry.IsMerchantOwner(account)).ToArray();
            foreach (Merchant merchant in merchants)
            {
                this.UnActiveMerchant(merchant);
                yield return merchant;
            }
		}
		public void Save()
		{
			foreach (Merchant current in 
				from merchant in this.m_activeMerchants
				where merchant.IsRecordDirty
				select merchant)
			{
				current.Save();
			}
		}
	}
}
