using Stump.Core.Reflection;
using Stump.Server.WorldServer.Database.Items;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Handlers.Inventory;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Items.Player
{
	public sealed class CharacterMerchantBag : ItemsCollection<MerchantItem>
	{
		public Character Owner
		{
			get;
			private set;
		}
		public bool IsLoaded
		{
			get;
			private set;
		}
		public CharacterMerchantBag(Character owner)
		{
			this.Owner = owner;
		}
		internal void LoadMerchantBag(MerchantBag bag)
		{
			if (!this.IsLoaded)
			{
				base.Items = bag.ToDictionary((MerchantItem x) => x.Guid);
				this.IsLoaded = true;
			}
		}
		internal void LoadMerchantBag()
		{
			if (!this.IsLoaded)
			{
				System.Collections.Generic.List<PlayerMerchantItemRecord> source = Singleton<ItemManager>.Instance.FindPlayerMerchantItems(this.Owner.Id);
				base.Items = (
					from entry in source
					select new MerchantItem(entry)).ToDictionary((MerchantItem entry) => entry.Guid);
				this.IsLoaded = true;
			}
		}
		private void UnLoadMerchantBag()
		{
			base.Items.Clear();
		}
		public int GetMerchantTax()
		{
			double num = base.Items.Aggregate(0.0, (double current, System.Collections.Generic.KeyValuePair<int, MerchantItem> item) => current + item.Value.Price * item.Value.Stack);
			num *= 0.1;
			return (int)num;
		}
		public bool MoveToInventory(MerchantItem item)
		{
			return this.MoveToInventory(item, item.Stack);
		}
		public bool MoveToInventory(MerchantItem item, uint quantity)
		{
			bool result;
			if (quantity == 0u)
			{
				result = false;
			}
			else
			{
				if (quantity > item.Stack)
				{
					quantity = item.Stack;
				}
				this.RemoveItem(item, quantity, true);
				BasePlayerItem item2 = Singleton<ItemManager>.Instance.CreatePlayerItem(this.Owner, item.Template, quantity, item.Effects);
				this.Owner.Inventory.AddItem(item2);
				result = true;
			}
			return result;
		}
		protected override void OnItemAdded(MerchantItem item)
		{
			InventoryHandler.SendExchangeShopStockMovementUpdatedMessage(this.Owner.Client, item);
			base.OnItemAdded(item);
		}
		protected override void OnItemRemoved(MerchantItem item)
		{
			InventoryHandler.SendExchangeShopStockMovementRemovedMessage(this.Owner.Client, item);
			base.OnItemRemoved(item);
		}
		protected override void OnItemStackChanged(MerchantItem item, int difference)
		{
			InventoryHandler.SendExchangeShopStockMovementUpdatedMessage(this.Owner.Client, item);
			base.OnItemStackChanged(item, difference);
		}
	}
}
