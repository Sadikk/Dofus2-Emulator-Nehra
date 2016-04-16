using Stump.Server.WorldServer.Game.Items;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Fights.Results
{
	public class FightLoot
	{
		private readonly System.Collections.Generic.Dictionary<int, DroppedItem> m_items = new System.Collections.Generic.Dictionary<int, DroppedItem>();
		public IReadOnlyDictionary<int, DroppedItem> Items
		{
			get
			{
				return new ReadOnlyDictionary<int, DroppedItem>(this.m_items);
			}
		}
		public int Kamas
		{
			get;
			set;
		}
		public void AddItem(int itemId)
		{
			this.AddItem(itemId, 1u);
		}
		public void AddItem(int itemId, uint amount)
		{
			if (this.m_items.ContainsKey(itemId))
			{
				this.m_items[itemId].Amount += 1u;
			}
			else
			{
				this.m_items.Add(itemId, new DroppedItem(itemId, amount));
			}
		}
		public void AddItem(DroppedItem item)
		{
			if (this.m_items.ContainsKey(item.ItemId))
			{
				this.m_items[item.ItemId].Amount += item.Amount;
			}
			else
			{
				this.m_items.Add(item.ItemId, new DroppedItem(item.ItemId, item.Amount));
			}
		}
		public Stump.DofusProtocol.Types.FightLoot GetFightLoot()
		{
			return new Stump.DofusProtocol.Types.FightLoot(this.m_items.Values.SelectMany((DroppedItem entry) => new ushort[]
			{
				(ushort)entry.ItemId,
				(ushort)entry.Amount
			}),(uint) this.Kamas);
		}
		public string FightItemsString()
		{
			return string.Join("|", (
				from item in this.m_items
				select item.Value.ItemId + "_" + item.Value.Amount).ToList<string>());
		}
	}
}
