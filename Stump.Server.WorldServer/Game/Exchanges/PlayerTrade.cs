using MongoDB.Bson;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Logging;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Exchanges.Items;
using Stump.Server.WorldServer.Game.Items.Player;
using Stump.Server.WorldServer.Handlers.Inventory;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Exchanges
{
	public class PlayerTrade : Trade<PlayerTrader, PlayerTrader>
	{
		public override ExchangeTypeEnum ExchangeType
		{
			get
			{
				return ExchangeTypeEnum.PLAYER_TRADE;
			}
		}
		public PlayerTrade(Character first, Character second)
		{
			base.FirstTrader = new PlayerTrader(first, this);
			base.SecondTrader = new PlayerTrader(second, this);
		}
		public override void Open()
		{
			base.Open();
			base.FirstTrader.Character.SetDialoger(base.FirstTrader);
			base.SecondTrader.Character.SetDialoger(base.SecondTrader);
			InventoryHandler.SendExchangeStartedWithPodsMessage(base.FirstTrader.Character.Client, this);
			InventoryHandler.SendExchangeStartedWithPodsMessage(base.SecondTrader.Character.Client, this);
		}
		public override void Close()
		{
			base.Close();
			InventoryHandler.SendExchangeLeaveMessage(base.FirstTrader.Character.Client, DialogTypeEnum.DIALOG_EXCHANGE, base.FirstTrader.ReadyToApply && base.SecondTrader.ReadyToApply);
			InventoryHandler.SendExchangeLeaveMessage(base.SecondTrader.Character.Client, DialogTypeEnum.DIALOG_EXCHANGE, base.FirstTrader.ReadyToApply && base.SecondTrader.ReadyToApply);
			base.FirstTrader.Character.CloseDialog(this);
			base.SecondTrader.Character.CloseDialog(this);
		}
		protected override void Apply()
		{
			if (base.FirstTrader.Items.All(delegate(TradeItem x)
			{
				BasePlayerItem basePlayerItem = base.FirstTrader.Character.Inventory.TryGetItem(x.Guid);
				return basePlayerItem != null && basePlayerItem.Stack >= x.Stack;
			}) && base.SecondTrader.Items.All(delegate(TradeItem x)
			{
				BasePlayerItem basePlayerItem = base.SecondTrader.Character.Inventory.TryGetItem(x.Guid);
				return basePlayerItem != null && basePlayerItem.Stack >= x.Stack;
			}))
			{
				base.FirstTrader.Character.Inventory.SetKamas((int)((long)base.FirstTrader.Character.Inventory.Kamas + (long)((ulong)(base.SecondTrader.Kamas - base.FirstTrader.Kamas))));
				base.SecondTrader.Character.Inventory.SetKamas((int)((long)base.SecondTrader.Character.Inventory.Kamas + (long)((ulong)(base.FirstTrader.Kamas - base.SecondTrader.Kamas))));
				foreach (TradeItem current in base.FirstTrader.Items)
				{
					BasePlayerItem item = base.FirstTrader.Character.Inventory.TryGetItem(current.Guid);
					base.FirstTrader.Character.Inventory.ChangeItemOwner(base.SecondTrader.Character, item, current.Stack);
				}
				foreach (TradeItem current in base.SecondTrader.Items)
				{
					BasePlayerItem item = base.SecondTrader.Character.Inventory.TryGetItem(current.Guid);
					base.SecondTrader.Character.Inventory.ChangeItemOwner(base.FirstTrader.Character, item, current.Stack);
				}
				InventoryHandler.SendInventoryWeightMessage(base.FirstTrader.Character.Client);
				InventoryHandler.SendInventoryWeightMessage(base.SecondTrader.Character.Client);
				BsonDocument document = new BsonDocument
				{

					{
						"FirstTraderId",
						base.FirstTrader.Id
					},

					{
						"SecondTraderId",
						base.SecondTrader.Id
					},

					{
						"FirstTraderKamas",
						(long)((ulong)base.FirstTrader.Kamas)
					},

					{
						"SecondTraderKamas",
						(long)((ulong)base.SecondTrader.Kamas)
					},

					{
						"FirstTraderItems",
						base.FirstTrader.ItemsString
					},

					{
						"SecondTraderItems",
						base.SecondTrader.ItemsString
					},

					{
						"Date",
						System.DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture)
					}
				};
				Singleton<MongoLogger>.Instance.Insert("PlayerTrade", document);
			}
		}
		protected override void OnTraderItemMoved(Trader trader, TradeItem item, bool modified, int difference)
		{
			base.OnTraderItemMoved(trader, item, modified, difference);
			if (item.Stack == 0u)
			{
				InventoryHandler.SendExchangeObjectRemovedMessage(base.FirstTrader.Character.Client, trader != base.FirstTrader, item.Guid);
				InventoryHandler.SendExchangeObjectRemovedMessage(base.SecondTrader.Character.Client, trader != base.SecondTrader, item.Guid);
			}
			else
			{
				if (modified)
				{
					InventoryHandler.SendExchangeObjectModifiedMessage(base.FirstTrader.Character.Client, trader != base.FirstTrader, item);
					InventoryHandler.SendExchangeObjectModifiedMessage(base.SecondTrader.Character.Client, trader != base.SecondTrader, item);
				}
				else
				{
					InventoryHandler.SendExchangeObjectAddedMessage(base.FirstTrader.Character.Client, trader != base.FirstTrader, item);
					InventoryHandler.SendExchangeObjectAddedMessage(base.SecondTrader.Character.Client, trader != base.SecondTrader, item);
				}
			}
		}
		protected override void OnTraderKamasChanged(Trader trader, uint amount)
		{
			base.OnTraderKamasChanged(trader, amount);
			InventoryHandler.SendExchangeKamaModifiedMessage(base.FirstTrader.Character.Client, trader != base.FirstTrader, (int)amount);
			InventoryHandler.SendExchangeKamaModifiedMessage(base.SecondTrader.Character.Client, trader != base.SecondTrader, (int)amount);
		}
		protected override void OnTraderReadyStatusChanged(Trader trader, bool status)
		{
			base.OnTraderReadyStatusChanged(trader, status);
			InventoryHandler.SendExchangeIsReadyMessage(base.FirstTrader.Character.Client, trader, status);
			InventoryHandler.SendExchangeIsReadyMessage(base.SecondTrader.Character.Client, trader, status);
		}
	}
}
