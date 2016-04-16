using Stump.Core.Reflection;
using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Database.Items;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Merchants;
using Stump.Server.WorldServer.Handlers.Inventory;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Items.Player
{
	public class MerchantBag : ItemsCollection<MerchantItem>
	{
		public Merchant Owner
		{
			get;
			set;
		}
		public bool IsDirty
		{
			get;
			set;
		}
		public MerchantBag(Merchant owner)
		{
			this.Owner = owner;
		}
		public MerchantBag(Merchant owner, System.Collections.Generic.IEnumerable<MerchantItem> merchantBag)
		{
			this.Owner = owner;
			base.Items = merchantBag.ToDictionary((MerchantItem entry) => entry.Guid);
		}
		protected override void OnItemStackChanged(MerchantItem item, int difference)
		{
			this.IsDirty = true;
			InventoryHandler.SendExchangeShopStockMovementUpdatedMessage((
				from x in this.Owner.OpenDialogs
				select x.Character).ToClients(), item);
			base.OnItemStackChanged(item, difference);
		}
		protected override void OnItemAdded(MerchantItem item)
		{
			this.IsDirty = true;
			InventoryHandler.SendExchangeShopStockMovementUpdatedMessage((
				from x in this.Owner.OpenDialogs
				select x.Character).ToClients(), item);
			base.OnItemAdded(item);
		}
		protected override void OnItemRemoved(MerchantItem item)
		{
			this.IsDirty = true;
			InventoryHandler.SendExchangeShopStockMovementRemovedMessage((
				from x in this.Owner.OpenDialogs
				select x.Character).ToClients(), item);
			if (base.Count == 0)
			{
				this.Owner.Delete();
			}
			base.OnItemRemoved(item);
		}
		public void LoadRecord()
		{
			System.Collections.Generic.List<PlayerMerchantItemRecord> source = Singleton<ItemManager>.Instance.FindPlayerMerchantItems(this.Owner.Id);
			base.Items = (
				from entry in source
				select new MerchantItem(entry)).ToDictionary((MerchantItem entry) => entry.Guid);
		}
		public override void Save()
		{
			base.Save();
			this.IsDirty = false;
		}
	}
}
