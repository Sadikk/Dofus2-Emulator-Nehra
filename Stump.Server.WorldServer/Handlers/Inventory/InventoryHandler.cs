using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Messages;
using Stump.DofusProtocol.Types;
using Stump.Server.BaseServer.Network;
using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Database.Items.Templates;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Merchants;
using Stump.Server.WorldServer.Game.Actors.RolePlay.TaxCollectors;
using Stump.Server.WorldServer.Game.Dialogs;
using Stump.Server.WorldServer.Game.Dialogs.Merchants;
using Stump.Server.WorldServer.Game.Dialogs.Npcs;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Exchanges;
using Stump.Server.WorldServer.Game.Exchanges.Items;
using Stump.Server.WorldServer.Game.Items.Player;
using Stump.Server.WorldServer.Game.Items.Player.Custom.LivingObjects;
using Stump.Server.WorldServer.Game.Spells;
using System.Linq;
namespace Stump.Server.WorldServer.Handlers.Inventory
{
	public class InventoryHandler : WorldHandlerContainer
	{
        private InventoryHandler() { }

        [WorldHandler(ExchangePlayerRequestMessage.Id)]
		public static void HandleExchangePlayerRequestMessage(WorldClient client, ExchangePlayerRequestMessage message)
		{
			ExchangeTypeEnum exchangeType = (ExchangeTypeEnum)message.exchangeType;
			if (exchangeType != ExchangeTypeEnum.PLAYER_TRADE)
			{
				InventoryHandler.SendExchangeErrorMessage(client, ExchangeErrorEnum.REQUEST_IMPOSSIBLE);
			}
			else
			{
				Character character = Singleton<World>.Instance.GetCharacter((int)message.target);
				if (character == null)
				{
					InventoryHandler.SendExchangeErrorMessage(client, ExchangeErrorEnum.BID_SEARCH_ERROR);
				}
				else
				{
					if (character.Map.Id != client.Character.Map.Id)
					{
						InventoryHandler.SendExchangeErrorMessage(client, ExchangeErrorEnum.REQUEST_CHARACTER_TOOL_TOO_FAR);
					}
					else
					{
						if (character.IsInRequest() || character.IsTrading())
						{
							InventoryHandler.SendExchangeErrorMessage(client, ExchangeErrorEnum.REQUEST_CHARACTER_OCCUPIED);
						}
						else
						{
							if (character.IsAway)
							{
								InventoryHandler.SendExchangeErrorMessage(client, ExchangeErrorEnum.REQUEST_CHARACTER_OCCUPIED);
							}
							else
							{
								if (!client.Character.Map.AllowExchangesBetweenPlayers)
								{
									InventoryHandler.SendExchangeErrorMessage(client, ExchangeErrorEnum.REQUEST_IMPOSSIBLE);
								}
								else
								{
									PlayerTradeRequest playerTradeRequest = new PlayerTradeRequest(client.Character, character);
									client.Character.OpenRequestBox(playerTradeRequest);
									character.OpenRequestBox(playerTradeRequest);
									playerTradeRequest.Open();
								}
							}
						}
					}
				}
			}
		}
        [WorldHandler(ExchangeAcceptMessage.Id)]
		public static void HandleExchangeAcceptMessage(WorldClient client, ExchangeAcceptMessage message)
		{
			if (client.Character.IsInRequest() && client.Character.RequestBox is PlayerTradeRequest)
			{
				client.Character.AcceptRequest();
			}
		}
        [WorldHandler(ExchangeObjectMoveKamaMessage.Id)]
		public static void HandleExchangeObjectMoveKamaMessage(WorldClient client, ExchangeObjectMoveKamaMessage message)
		{
			if (client.Character.IsTrading())
			{
				client.Character.Trader.SetKamas((uint)message.quantity);
			}
		}
        [WorldHandler(ExchangeObjectMoveMessage.Id)]
		public static void HandleExchangeObjectMoveMessage(WorldClient client, ExchangeObjectMoveMessage message)
		{
			if (client.Character.IsTrading())
			{
                client.Character.Trader.MoveItem((int)message.objectUID, message.quantity);
			}
			else
			{
				if (client.Character.IsInMerchantDialog() && message.quantity <= 0)
				{
                    MerchantItem merchantItem = client.Character.MerchantBag.TryGetItem((int)message.objectUID);
					if (merchantItem != null && client.Character.MerchantBag.MoveToInventory(merchantItem))
					{
						client.Send(new ExchangeShopStockMovementRemovedMessage(message.objectUID));
					}
				}
			}
		}
        [WorldHandler(ExchangeReadyMessage.Id)]
		public static void HandleExchangeReadyMessage(WorldClient client, ExchangeReadyMessage message)
		{
			if (message != null && client != null)
			{
				client.Character.Trader.ToggleReady(message.ready);
			}
		}
        [WorldHandler(ExchangeBuyMessage.Id)]
		public static void HandleExchangeBuyMessage(WorldClient client, ExchangeBuyMessage message)
		{
			IShopDialog shopDialog = client.Character.Dialog as IShopDialog;
			if (shopDialog != null)
			{
                shopDialog.BuyItem((int)message.objectToBuyId, (uint)message.quantity);
			}
		}
        [WorldHandler(ExchangeSellMessage.Id)]
		public static void HandleExchangeSellMessage(WorldClient client, ExchangeSellMessage message)
		{
			IShopDialog shopDialog = client.Character.Dialog as IShopDialog;
			if (shopDialog != null)
			{
                shopDialog.SellItem((int)message.objectToSellId, (uint)message.quantity);
			}
            client.Character.SendServerMessage("Mode marchant retiré ! Utilise HDV", System.Drawing.Color.Red);
            /*int num = client.Character.MerchantBag.GetMerchantTax();
			if (num <= 0)
			{
				num = 1;
			}
			client.Send(new ExchangeReplyTaxVendorMessage(0, num));*/
        }
        [WorldHandler(ExchangeShowVendorTaxMessage.Id)]
		public static void HandleExchangeShowVendorTaxMessage(WorldClient client, ExchangeShowVendorTaxMessage message)
		{
			int num = client.Character.MerchantBag.GetMerchantTax();
			if (num <= 0)
			{
				num = 1;
			}
            client.Send(new ExchangeReplyTaxVendorMessage(0, (uint)num));
		}
        [WorldHandler(ExchangeRequestOnShopStockMessage.Id)]
		public static void HandleExchangeRequestOnShopStockMessage(WorldClient client, ExchangeRequestOnShopStockMessage message)
		{
			InventoryHandler.SendExchangeShopStockStartedMessage(client, client.Character.MerchantBag);
		}
        [WorldHandler(ExchangeObjectMovePricedMessage.Id)]
		public static void HandleExchangeObjectMovePricedMessage(WorldClient client, ExchangeObjectMovePricedMessage message)
		{
			if (message.quantity > 0 && message.price > 0)
			{
                BasePlayerItem basePlayerItem = client.Character.Inventory.TryGetItem((int)message.objectUID);
				if (basePlayerItem != null && !client.Character.IsBusy())
				{
					client.Character.Inventory.MoveToMerchantBag(basePlayerItem, (uint)message.quantity, (uint)message.price);
				}
			}
		}
        [WorldHandler(ExchangeObjectModifyPricedMessage.Id)]
		public static void HandleExchangeObjectModifyPricedMessage(WorldClient client, ExchangeObjectModifyPricedMessage message)
		{
			if (!client.Character.IsBusy() && message.price > 0)
			{
                MerchantItem merchantItem = client.Character.MerchantBag.TryGetItem((int)message.objectUID);
				if (merchantItem != null)
				{
					merchantItem.Price = (uint)message.price;
					if ((long)message.quantity == (long)((ulong)merchantItem.Stack) || message.quantity == 0)
					{
						InventoryHandler.SendExchangeShopStockMovementUpdatedMessage(client, merchantItem);
					}
					else
					{
						if ((long)message.quantity < (long)((ulong)merchantItem.Stack))
						{
							client.Character.MerchantBag.MoveToInventory(merchantItem, (uint)((ulong)merchantItem.Stack - (ulong)((long)message.quantity)));
						}
						else
						{
							BasePlayerItem basePlayerItem = client.Character.Inventory.TryGetItem(merchantItem.Template);
							if (basePlayerItem != null)
							{
								client.Character.Inventory.MoveToMerchantBag(basePlayerItem, (uint)((long)message.quantity - (long)((ulong)merchantItem.Stack)), (uint)message.price);
							}
						}
					}
				}
			}
		}
        [WorldHandler(ExchangeStartAsVendorMessage.Id)]
		public static void HandleExchangeStartAsVendorMessage(WorldClient client, ExchangeStartAsVendorMessage message)
		{
			client.Character.EnableMerchantMode();
		}
        [WorldHandler(ExchangeOnHumanVendorRequestMessage.Id)]
		public static void HandleExchangeOnHumanVendorRequestMessage(WorldClient client, ExchangeOnHumanVendorRequestMessage message)
		{
            Merchant actor = client.Character.Map.GetActor<Merchant>((int)message.humanVendorId);
			if (actor != null && (int)actor.Cell.Id == message.humanVendorCell)
			{
				MerchantShopDialog merchantShopDialog = new MerchantShopDialog(actor, client.Character);
				merchantShopDialog.Open();
			}
		}
        [WorldHandler(ExchangeRequestOnTaxCollectorMessage.Id)]
		public static void HandleExchangeRequestOnTaxCollectorMessage(WorldClient client, ExchangeRequestOnTaxCollectorMessage message)
		{
			if (client.Character.Guild != null)
			{
				TaxCollectorNpc taxCollector = client.Character.Map.TaxCollector;
				if (taxCollector != null)
				{
					if (!taxCollector.IsTaxCollectorOwner(client.Character.GuildMember))
					{
						client.Send(new TaxCollectorErrorMessage(2));
					}
					else
					{
						TaxCollectorTrade taxCollectorTrade = new TaxCollectorTrade(taxCollector, client.Character);
						taxCollectorTrade.Open();
					}
				}
			}
		}
        [WorldHandler(ObjectSetPositionMessage.Id)]
        public static void HandleObjectSetPositionMessage(WorldClient client, ObjectSetPositionMessage message)
        {
            if (System.Enum.IsDefined(typeof(CharacterInventoryPositionEnum), (int)message.position))
            {
                BasePlayerItem basePlayerItem = client.Character.Inventory.TryGetItem((int)message.objectUID);
                if (basePlayerItem != null)
                {
                    client.Character.Inventory.MoveItem(basePlayerItem, (CharacterInventoryPositionEnum)message.position);
                }
            }
        }
        [WorldHandler(ObjectDeleteMessage.Id)]
        public static void HandleObjectDeleteMessage(WorldClient client, ObjectDeleteMessage message)
        {
            BasePlayerItem basePlayerItem = client.Character.Inventory.TryGetItem((int)message.objectUID);
            if (basePlayerItem != null)
            {
                client.Character.Inventory.RemoveItem(basePlayerItem, (uint)message.quantity, true);
            }
        }
        [WorldHandler(ObjectUseMessage.Id)]
        public static void HandleObjectUseMessage(WorldClient client, ObjectUseMessage message)
        {
            BasePlayerItem basePlayerItem = client.Character.Inventory.TryGetItem((int)message.objectUID);
            if (basePlayerItem != null)
            {
                client.Character.Inventory.UseItem(basePlayerItem, 1u);
            }
        }
        [WorldHandler(ObjectUseMultipleMessage.Id)]
        public static void HandleObjectUseMultipleMessage(WorldClient client, ObjectUseMultipleMessage message)
        {
            BasePlayerItem basePlayerItem = client.Character.Inventory.TryGetItem((int)message.objectUID);
            if (basePlayerItem != null)
            {
                client.Character.Inventory.UseItem(basePlayerItem, (uint)message.quantity);
            }
        }
        [WorldHandler(ObjectUseOnCellMessage.Id)]
        public static void HandleObjectUseOnCellMessage(WorldClient client, ObjectUseOnCellMessage message)
        {
            BasePlayerItem basePlayerItem = client.Character.Inventory.TryGetItem((int)message.objectUID);
            if (basePlayerItem != null)
            {
                Cell cell = client.Character.Map.GetCell((int)message.cells);
                if (cell != null)
                {
                    client.Character.Inventory.UseItem(basePlayerItem, cell, 1u);
                }
            }
        }
        [WorldHandler(ObjectUseOnCharacterMessage.Id)]
        public static void HandleObjectUseOnCharacterMessage(WorldClient client, ObjectUseOnCharacterMessage message)
        {
            BasePlayerItem basePlayerItem = client.Character.Inventory.TryGetItem((int)message.objectUID);
            if (basePlayerItem != null)
            {
                Character actor = client.Character.Map.GetActor<Character>((int)message.characterId);
                if (actor != null)
                {
                    client.Character.Inventory.UseItem(basePlayerItem, actor, 1u);
                }
            }
        }
        [WorldHandler(ObjectFeedMessage.Id)]
        public static void HandleObjectFeedMessage(WorldClient client, ObjectFeedMessage message)
        {
            BasePlayerItem basePlayerItem = client.Character.Inventory.TryGetItem((int)message.objectUID);
            BasePlayerItem basePlayerItem2 = client.Character.Inventory.TryGetItem((int)message.foodUID);
            if (basePlayerItem != null && basePlayerItem2 != null)
            {
                if ((ulong)basePlayerItem2.Stack < (ulong)((long)message.foodQuantity))
                {
                    message.foodQuantity = (uint)basePlayerItem2.Stack;
                }
                uint num = 0u;
                while ((ulong)num < (ulong)((long)message.foodQuantity) && basePlayerItem.Feed(basePlayerItem2))
                {
                    num += 1u;
                }
                client.Character.Inventory.RemoveItem(basePlayerItem2, num, true);
            }
        }
        [WorldHandler(LivingObjectChangeSkinRequestMessage.Id)]
        public static void HandleLivingObjectChangeSkinRequestMessage(WorldClient client, LivingObjectChangeSkinRequestMessage message)
        {
            BasePlayerItem basePlayerItem = client.Character.Inventory.TryGetItem((int)message.livingUID);
            if (basePlayerItem is CommonLivingObject)
            {
                ((CommonLivingObject)basePlayerItem).SelectedLevel = (short)message.skinId;
            }
        }
        [WorldHandler(LivingObjectDissociateMessage.Id)]
        public static void HandleLivingObjectDissociateMessage(WorldClient client, LivingObjectDissociateMessage message)
        {
            BasePlayerItem basePlayerItem = client.Character.Inventory.TryGetItem((int)message.livingUID);
            if (basePlayerItem is BoundLivingObjectItem)
            {
                ((BoundLivingObjectItem)basePlayerItem).Dissociate();
            }
        }
        [WorldHandler(SpellUpgradeRequestMessage.Id)]
        public static void HandleSpellUpgradeRequestMessage(WorldClient client, SpellUpgradeRequestMessage message)
        {
            client.Character.Spells.BoostSpell((int)message.spellId, message.spellLevel);
            client.Character.RefreshStats();
        }

		public static void SendExchangeRequestedTradeMessage(IPacketReceiver client, ExchangeTypeEnum type, Character source, Character target)
		{
            client.Send(new ExchangeRequestedTradeMessage((sbyte)type, (uint)source.Id, (uint)target.Id));
		}
		public static void SendExchangeStartedWithPodsMessage(IPacketReceiver client, PlayerTrade playerTrade)
		{
            client.Send(new ExchangeStartedWithPodsMessage(1, playerTrade.FirstTrader.Character.Id, (uint)playerTrade.FirstTrader.Character.Inventory.Weight, (uint)playerTrade.FirstTrader.Character.Inventory.WeightTotal, playerTrade.SecondTrader.Character.Id, (uint)playerTrade.SecondTrader.Character.Inventory.Weight, (uint)playerTrade.SecondTrader.Character.Inventory.WeightTotal));
		}
        //public static void SendExchangeStartOkTaxCollectorMessage(IPacketReceiver client, TaxCollectorNpc taxCollector)
        //{
        //    client.Send(new ExchangeStartOkTaxCollectorMessage(taxCollector.Id, 
        //        from x in taxCollector.Bag
        //        select x.GetObjectItem(), 0));
        //}
		public static void SendExchangeStartOkHumanVendorMessage(IPacketReceiver client, Merchant merchant)
		{
            client.Send(new ExchangeStartOkHumanVendorMessage((uint)merchant.Id, 
				from x in merchant.Bag
				select x.GetObjectItemToSellInHumanVendorShop()));
		}
		public static void SendExchangeStartOkNpcTradeMessage(IPacketReceiver client, NpcTrade trade)
		{
			client.Send(new ExchangeStartOkNpcTradeMessage(trade.SecondTrader.Npc.Id));
		}
		public static void SendExchangeStartOkNpcShopMessage(IPacketReceiver client, NpcShopDialog dialog)
		{
            client.Send(new ExchangeStartOkNpcShopMessage(dialog.Npc.Id, (dialog.Token != null) ? (ushort)dialog.Token.Id : (ushort)0, 
				from entry in dialog.Items
				select entry.GetNetworkItem() as ObjectItemToSellInNpcShop));
		}
		public static void SendExchangeLeaveMessage(IPacketReceiver client, DialogTypeEnum dialogType, bool success)
		{
			client.Send(new ExchangeLeaveMessage((sbyte)dialogType, success));
		}
		public static void SendExchangeObjectAddedMessage(IPacketReceiver client, bool remote, TradeItem item)
		{
			client.Send(new ExchangeObjectAddedMessage(remote, item.GetObjectItem()));
		}
		public static void SendExchangeObjectModifiedMessage(IPacketReceiver client, bool remote, TradeItem item)
		{
			client.Send(new ExchangeObjectModifiedMessage(remote, item.GetObjectItem()));
		}
		public static void SendExchangeObjectRemovedMessage(IPacketReceiver client, bool remote, int guid)
		{
            client.Send(new ExchangeObjectRemovedMessage(remote, (uint)guid));
		}
		public static void SendExchangeIsReadyMessage(IPacketReceiver client, Trader trader, bool ready)
		{
            client.Send(new ExchangeIsReadyMessage((uint)trader.Id, ready));
		}
		public static void SendExchangeErrorMessage(IPacketReceiver client, ExchangeErrorEnum errorEnum)
		{
			client.Send(new ExchangeErrorMessage((sbyte)errorEnum));
		}
		public static void SendExchangeShopStockStartedMessage(IPacketReceiver client, CharacterMerchantBag merchantBag)
		{
			client.Send(new ExchangeShopStockStartedMessage(
				from x in merchantBag
				select x.GetObjectItemToSell()));
		}
		public static void SendKamasUpdateMessage(IPacketReceiver client, int kamasAmount)
		{
			client.Send(new KamasUpdateMessage(kamasAmount));
		}
		public static void SendGameRolePlayPlayerLifeStatusMessage(IPacketReceiver client)
		{
			client.Send(new GameRolePlayPlayerLifeStatusMessage());
		}
		public static void SendInventoryContentMessage(WorldClient client)
		{
			client.Send(new InventoryContentMessage(
				from entry in client.Character.Inventory
                select entry.GetObjectItem(), (uint)client.Character.Inventory.Kamas));
		}
		public static void SendInventoryWeightMessage(WorldClient client)
		{
            client.Send(new InventoryWeightMessage((uint)client.Character.Inventory.Weight, (uint)client.Character.Inventory.WeightTotal));
		}
		public static void SendExchangeKamaModifiedMessage(IPacketReceiver client, bool remote, int kamasAmount)
		{
            client.Send(new ExchangeKamaModifiedMessage(remote, (uint)kamasAmount));
		}
		public static void SendObjectAddedMessage(IPacketReceiver client, BasePlayerItem addedItem)
		{
			client.Send(new ObjectAddedMessage(addedItem.GetObjectItem()));
		}
		public static void SendObjectsAddedMessage(IPacketReceiver client, System.Collections.Generic.IEnumerable<BasePlayerItem> addeditems)
		{
			client.Send(new ObjectsAddedMessage(
				from entry in addeditems
				select entry.GetObjectItem()));
		}
		public static void SendObjectDeletedMessage(IPacketReceiver client, int guid)
		{
            client.Send(new ObjectDeletedMessage((uint)guid));
		}
		public static void SendObjectsDeletedMessage(IPacketReceiver client, System.Collections.Generic.IEnumerable<int> guids)
		{
			client.Send(new ObjectsDeletedMessage((
				from entry in guids
				select entry).Cast<uint>().ToList()));
		}
		public static void SendObjectModifiedMessage(IPacketReceiver client, BasePlayerItem item)
		{
			client.Send(new ObjectModifiedMessage(item.GetObjectItem()));
		}
		public static void SendObjectMovementMessage(IPacketReceiver client, BasePlayerItem movedItem)
		{
			client.Send(new ObjectMovementMessage((uint)movedItem.Guid, (byte)movedItem.Position));
		}
		public static void SendObjectQuantityMessage(IPacketReceiver client, BasePlayerItem item)
		{
            client.Send(new ObjectQuantityMessage((uint)item.Guid, (uint)item.Stack));
		}
		public static void SendObjectErrorMessage(IPacketReceiver client, ObjectErrorEnum error)
		{
			client.Send(new ObjectErrorMessage((sbyte)error));
		}
		public static void SendSetUpdateMessage(WorldClient client, ItemSetTemplate itemSet)
		{
			client.Send(new SetUpdateMessage((ushort)itemSet.Id, 
				from entry in client.Character.Inventory.GetItemSetEquipped(itemSet)
				select (ushort)entry.Template.Id, client.Character.Inventory.GetItemSetEffects(itemSet).Select((EffectBase entry) => entry.GetObjectEffect())));
		}
		public static void SendExchangeShopStockMovementUpdatedMessage(IPacketReceiver client, MerchantItem item)
		{
			client.Send(new ExchangeShopStockMovementUpdatedMessage(item.GetObjectItemToSell()));
		}
		public static void SendExchangeShopStockMovementRemovedMessage(IPacketReceiver client, MerchantItem item)
		{
            client.Send(new ExchangeShopStockMovementRemovedMessage((uint)item.Guid));
		}
		public static void SendSpellUpgradeSuccessMessage(IPacketReceiver client, Spell spell)
		{
			client.Send(new SpellUpgradeSuccessMessage(spell.Id, (sbyte)spell.CurrentLevel));
		}
		public static void SendSpellUpgradeSuccessMessage(IPacketReceiver client, int spellId, sbyte level)
		{
			client.Send(new SpellUpgradeSuccessMessage(spellId, level));
		}
		public static void SendSpellUpgradeFailureMessage(IPacketReceiver client)
		{
			client.Send(new SpellUpgradeFailureMessage());
		}
		public static void SendSpellListMessage(WorldClient client, bool previsualization)
		{
			client.Send(new SpellListMessage(previsualization, 
				from entry in client.Character.Spells.GetSpells()
				select entry.GetSpellItem()));
		}
		public static void SendSpellForgottenMessage(IPacketReceiver client, System.Collections.Generic.IEnumerable<Spell> forgottenSpells, short spellPoints)
		{
			client.Send(new SpellForgottenMessage(
				from x in forgottenSpells
				select (ushort)x.Id, (ushort)spellPoints));
		}
		public static void SendSpellForgottenMessage(IPacketReceiver client, Spell forgottenSpell, short spellPoints)
		{
			client.Send(new SpellForgottenMessage(new ushort[]
			{
				(ushort)forgottenSpell.Id
			}, (ushort)spellPoints));
		}
		public static void SendStorageInventoryContentMessage(IPacketReceiver client, TaxCollectorNpc taxCollector)
		{
			client.Send(new ExchangeStartedMessage(8));
			client.Send(taxCollector.GetStorageInventoryContent());
		}
	}
}
