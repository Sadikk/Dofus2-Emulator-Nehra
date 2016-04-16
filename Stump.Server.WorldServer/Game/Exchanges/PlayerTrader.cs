using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Exchanges.Items;
using Stump.Server.WorldServer.Game.Items.Player;
using Stump.Server.WorldServer.Handlers.Basic;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Exchanges
{
	public class PlayerTrader : Trader
	{
		public Character Character
		{
			get;
			private set;
		}
		public override int Id
		{
			get
			{
				return this.Character.Id;
			}
		}
		public PlayerTrader(Character character, ITrade trade) : base(trade)
		{
			this.Character = character;
		}
		public override bool MoveItem(int guid, int amount)
		{
			return amount != 0 && ((amount > 0) ? this.MoveItemToPanel(guid, (uint)amount) : this.MoveItemToInventory(guid, (uint)(-(uint)amount)));
		}
		public bool MoveItemToPanel(int guid, uint amount)
		{
			BasePlayerItem basePlayerItem = this.Character.Inventory[guid];
			TradeItem tradeItem = base.Items.SingleOrDefault((TradeItem entry) => entry.Guid == guid);
			this.ToggleReady(false);
			bool result;
			if (basePlayerItem == null)
			{
				result = false;
			}
			else
			{
				if (amount > basePlayerItem.Stack || amount <= 0u)
				{
					result = false;
				}
				else
				{
					if (basePlayerItem.IsLinked() && base.Trade is PlayerTrade)
					{
						BasicHandler.SendTextInformationMessage(this.Character.Client, TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 345, new object[]
						{
							basePlayerItem.Template.Id,
							basePlayerItem.Guid
						});
						result = false;
					}
					else
					{
						if (tradeItem != null)
						{
							if (basePlayerItem.Stack < tradeItem.Stack + amount)
							{
								result = false;
							}
							else
							{
								uint stack = tradeItem.Stack;
								tradeItem.Stack += amount;
								if (tradeItem.Stack <= 0u)
								{
									base.RemoveItem(tradeItem);
								}
								this.OnItemMoved(tradeItem, true, (int)(tradeItem.Stack - stack));
								result = true;
							}
						}
						else
						{
							tradeItem = new PlayerTradeItem(basePlayerItem, amount);
							base.AddItem(tradeItem);
							this.OnItemMoved(tradeItem, false, (int)amount);
							result = true;
						}
					}
				}
			}
			return result;
		}
		public bool MoveItemToInventory(int guid, uint amount)
		{
			TradeItem tradeItem = base.Items.SingleOrDefault((TradeItem entry) => entry.Guid == guid);
			bool result;
			if (amount == 0u)
			{
				result = false;
			}
			else
			{
				this.ToggleReady(false);
				if (tradeItem == null)
				{
					result = false;
				}
				else
				{
					if (tradeItem.Stack <= amount)
					{
						base.RemoveItem(tradeItem);
						tradeItem.Stack = 0u;
					}
					else
					{
						tradeItem.Stack -= amount;
					}
					this.OnItemMoved(tradeItem, tradeItem.Stack != 0u, (int)(-(int)((ulong)amount)));
					result = true;
				}
			}
			return result;
		}
		public override bool SetKamas(uint amount)
		{
			return (ulong)amount <= (ulong)((long)this.Character.Inventory.Kamas) && base.SetKamas(amount);
		}
	}
}
