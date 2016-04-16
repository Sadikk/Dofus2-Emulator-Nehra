using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Messages;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Merchants;
using Stump.Server.WorldServer.Game.Items;
using Stump.Server.WorldServer.Game.Items.Player;
using Stump.Server.WorldServer.Handlers.Basic;
using Stump.Server.WorldServer.Handlers.Inventory;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Dialogs.Merchants
{
	public class MerchantShopDialog : IDialog, IShopDialog
	{
		public Merchant Merchant
		{
			get;
			private set;
		}
		public Character Character
		{
			get;
			private set;
		}
		public DialogTypeEnum DialogType
		{
			get
			{
				return DialogTypeEnum.DIALOG_EXCHANGE;
			}
		}
		public MerchantShopDialog(Merchant merchant, Character character)
		{
			this.Merchant = merchant;
			this.Character = character;
		}
		public void Open()
		{
			this.Character.SetDialog(this);
			this.Merchant.OnDialogOpened(this);
			InventoryHandler.SendExchangeStartOkHumanVendorMessage(this.Character.Client, this.Merchant);
		}
		public void Close()
		{
			InventoryHandler.SendExchangeLeaveMessage(this.Character.Client, this.DialogType, false);
			this.Character.CloseDialog(this);
			this.Merchant.OnDialogClosed(this);
		}
		public bool BuyItem(int itemGuid, uint quantity)
		{
			MerchantItem merchantItem = this.Merchant.Bag.FirstOrDefault((MerchantItem x) => x.Guid == itemGuid);
			bool result;
			if (merchantItem == null || quantity == 0u || !this.CanBuy(merchantItem, quantity))
			{
				this.Character.Client.Send(new ExchangeErrorMessage(8));
				result = false;
			}
			else
			{
				this.Merchant.Bag.RemoveItem(merchantItem, quantity, true);
				BasePlayerItem item = Singleton<ItemManager>.Instance.CreatePlayerItem(this.Character, merchantItem.Template, quantity, merchantItem.Effects);
				this.Character.Inventory.AddItem(item);
				BasicHandler.SendTextInformationMessage(this.Character.Client, TextInformationTypeEnum.TEXT_INFORMATION_MESSAGE, 21, new object[]
				{
					quantity,
					merchantItem.Template.Id
				});
				this.Character.Inventory.SubKamas((int)(merchantItem.Price * quantity));
				BasicHandler.SendTextInformationMessage(this.Character.Client, TextInformationTypeEnum.TEXT_INFORMATION_MESSAGE, 46, new object[]
				{
					(int)(merchantItem.Price * quantity)
				});
				this.Merchant.KamasEarned += merchantItem.Price * quantity;
				this.Character.Client.Send(new ExchangeBuyOkMessage());
				result = true;
			}
			return result;
		}
		public bool CanBuy(MerchantItem item, uint amount)
		{
			return (long)this.Character.Inventory.Kamas >= (long)((ulong)(item.Price * amount)) || !this.Merchant.CanBeSee(this.Character);
		}
		public bool SellItem(int id, uint quantity)
		{
			this.Character.Client.Send(new ExchangeErrorMessage(9));
			return false;
		}
	}
}
