using Stump.Core.Reflection;
using Stump.Server.BaseServer;
using Stump.Server.WorldServer.Database.Items;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Actors.RolePlay.TaxCollectors;
using Stump.Server.WorldServer.Game.Items.Player;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Items.TaxCollector
{
	public class TaxCollectorBag : ItemsCollection<TaxCollectorItem>
	{
		public TaxCollectorNpc Owner
		{
			get;
			private set;
		}
		public int BagWeight
		{
			get
			{
				return (int)this.Sum((TaxCollectorItem x) => (long)((ulong)(x.Template.RealWeight * x.Stack)));
			}
		}
		public int BagValue
		{
			get
			{
				return (int)this.Sum((TaxCollectorItem x) => x.Template.Price * x.Stack);
			}
		}
		public bool IsDirty
		{
			get;
			private set;
		}
		public TaxCollectorBag(TaxCollectorNpc owner)
		{
			this.Owner = owner;
		}
		protected override void OnItemStackChanged(TaxCollectorItem item, int difference)
		{
			this.IsDirty = true;
			base.OnItemStackChanged(item, difference);
		}
		protected override void OnItemAdded(TaxCollectorItem item)
		{
			this.IsDirty = true;
			base.OnItemAdded(item);
		}
		protected override void OnItemRemoved(TaxCollectorItem item)
		{
			this.IsDirty = true;
			if (base.Count == 0)
			{
				this.Owner.Delete();
			}
			base.OnItemRemoved(item);
		}
		public bool MoveToInventory(TaxCollectorItem item, Character character)
		{
			return this.MoveToInventory(item, character, item.Stack);
		}
		public bool MoveToInventory(TaxCollectorItem item, Character character, uint quantity)
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
				BasePlayerItem item2 = Singleton<ItemManager>.Instance.CreatePlayerItem(character, item.Template, quantity, item.Effects);
				character.Inventory.AddItem(item2);
				result = true;
			}
			return result;
		}
		public void LoadRecord()
		{
			if (ServerBase<WorldServer>.Instance.IsInitialized)
			{
				ServerBase<WorldServer>.Instance.IOTaskPool.EnsureContext();
			}
			System.Collections.Generic.List<TaxCollectorItemRecord> source = Singleton<ItemManager>.Instance.FindTaxCollectorItems(this.Owner.Id);
			base.Items = (
				from entry in source
				select new TaxCollectorItem(entry)).ToDictionary((TaxCollectorItem entry) => entry.Guid);
		}
		public void DeleteBag(bool lazySave = true)
		{
			base.DeleteAll(false);
			if (lazySave)
			{
				ServerBase<WorldServer>.Instance.IOTaskPool.AddMessage(delegate
				{
					this.Save();
				});
			}
			else
			{
				this.Save();
			}
		}
		public override void Save()
		{
			if (ServerBase<WorldServer>.Instance.IsInitialized)
			{
				ServerBase<WorldServer>.Instance.IOTaskPool.EnsureContext();
			}
			base.Save();
			this.IsDirty = false;
		}
	}
}
