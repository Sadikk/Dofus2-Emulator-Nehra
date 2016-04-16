using Stump.Core.Pool;
using Stump.Core.Reflection;
using Stump.Server.WorldServer.Database.Items.Templates;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Npcs;
using Stump.Server.WorldServer.Game.Exchanges.Items;
using Stump.Server.WorldServer.Game.Items;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Exchanges
{
	public class NpcTrader : Trader
	{
		private readonly UniqueIdProvider m_idProvider = new UniqueIdProvider();
		public Npc Npc
		{
			get;
			private set;
		}
		public override int Id
		{
			get
			{
				return this.Npc.Id;
			}
		}
		public NpcTrader(Npc npc, ITrade trade) : base(trade)
		{
			this.Npc = npc;
		}
		public override bool MoveItem(int id, int quantity)
		{
			ItemTemplate itemTemplate = Singleton<ItemManager>.Instance.TryGetTemplate(id);
			bool result;
			if (itemTemplate == null || quantity <= 0)
			{
				result = false;
			}
			else
			{
				this.AddItem(itemTemplate, (uint)quantity);
				result = true;
			}
			return result;
		}
		public void AddItem(ItemTemplate template, uint amount)
		{
			TradeItem tradeItem = base.Items.FirstOrDefault((TradeItem x) => x.Template == template);
			if (tradeItem != null)
			{
				tradeItem.Stack += amount;
				this.OnItemMoved(tradeItem, true, (int)amount);
			}
			else
			{
				tradeItem = new NpcTradeItem(this.m_idProvider.Pop(), template, amount);
				base.AddItem(tradeItem);
				this.OnItemMoved(tradeItem, false, (int)amount);
			}
		}
		public bool RemoveItem(ItemTemplate template, uint amount)
		{
			TradeItem tradeItem = base.Items.FirstOrDefault((TradeItem x) => x.Template == template);
			bool result;
			if (tradeItem == null)
			{
				result = false;
			}
			else
			{
				uint num = amount;
				if (tradeItem.Stack - amount <= 0u)
				{
					base.RemoveItem(tradeItem);
					num = tradeItem.Stack;
				}
				else
				{
					tradeItem.Stack -= amount;
				}
				this.OnItemMoved(tradeItem, tradeItem.Stack > 0u, (int)(-(int)num));
				result = true;
			}
			return result;
		}
	}
}
