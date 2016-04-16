using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Messages;
using Stump.Server.WorldServer.Database.Items.Shops;
using Stump.Server.WorldServer.Database.Items.Templates;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Npcs;
using Stump.Server.WorldServer.Game.Items;
using Stump.Server.WorldServer.Game.Items.Player;
using Stump.Server.WorldServer.Handlers.Basic;
using Stump.Server.WorldServer.Handlers.Dialogs;
using Stump.Server.WorldServer.Handlers.Inventory;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Dialogs.Npcs
{
	public class NpcShopDialog : IDialog, IShopDialog
	{
		public DialogTypeEnum DialogType
		{
			get
			{
				return DialogTypeEnum.DIALOG_PURCHASABLE;
			}
		}
		public System.Collections.Generic.IEnumerable<NpcItem> Items
		{
			get;
			protected set;
		}
		public ItemTemplate Token
		{
			get;
			protected set;
		}
		public Character Character
		{
			get;
			protected set;
		}
		public Npc Npc
		{
			get;
			protected set;
		}
		public bool CanSell
		{
			get;
			set;
		}
		public bool MaxStats
		{
			get;
			set;
		}
		public NpcShopDialog(Character character, Npc npc, System.Collections.Generic.IEnumerable<NpcItem> items)
		{
			this.Character = character;
			this.Npc = npc;
			this.Items = items;
			this.CanSell = true;
		}
		public NpcShopDialog(Character character, Npc npc, System.Collections.Generic.IEnumerable<NpcItem> items, ItemTemplate token)
		{
			this.Character = character;
			this.Npc = npc;
			this.Items = items;
			this.Token = token;
			this.CanSell = true;
		}
		public void Open()
		{
			this.Character.SetDialog(this);
			InventoryHandler.SendExchangeStartOkNpcShopMessage(this.Character.Client, this);
		}
		public void Close()
		{
			DialogHandler.SendLeaveDialogMessage(this.Character.Client, this.DialogType);
			this.Character.CloseDialog(this);
		}
		public virtual bool BuyItem(int itemId, uint amount)
		{
			NpcItem npcItem = this.Items.FirstOrDefault((NpcItem entry) => entry.Item.Id == itemId);
			bool result;
			if (npcItem == null)
			{
				this.Character.Client.Send(new ExchangeErrorMessage(8));
				result = false;
			}
			else
			{
				uint num = (uint)(npcItem.Price * amount);
				if (!this.CanBuy(npcItem, amount))
				{
					this.Character.Client.Send(new ExchangeErrorMessage(8));
					result = false;
				}
				else
				{
					BasicHandler.SendTextInformationMessage(this.Character.Client, TextInformationTypeEnum.TEXT_INFORMATION_MESSAGE, 21, new object[]
					{
						amount,
						itemId
					});
					BasePlayerItem item = Singleton<ItemManager>.Instance.CreatePlayerItem(this.Character, itemId, amount, this.MaxStats || npcItem.MaxStats);
					this.Character.Inventory.AddItem(item);
					if (this.Token != null)
					{
						this.Character.Inventory.UnStackItem(this.Character.Inventory.TryGetItem(this.Token), num);
					}
					else
					{
						this.Character.Inventory.SubKamas((int)num);
						BasicHandler.SendTextInformationMessage(this.Character.Client, TextInformationTypeEnum.TEXT_INFORMATION_MESSAGE, 46, new object[]
						{
							num
						});
					}
					this.Character.Client.Send(new ExchangeBuyOkMessage());
					result = true;
				}
			}
			return result;
		}
		public bool CanBuy(NpcItem item, uint amount)
		{
			bool result;
			if (this.Token != null)
			{
				BasePlayerItem basePlayerItem = this.Character.Inventory.TryGetItem(this.Token);
				if (basePlayerItem == null || basePlayerItem.Stack < item.Price * amount)
				{
					result = false;
					return result;
				}
			}
			else
			{
				if ((double)this.Character.Inventory.Kamas < item.Price * amount)
				{
					result = false;
					return result;
				}
			}
			result = true;
			return result;
		}
		public bool SellItem(int guid, uint amount)
		{
			bool result;
			if (!this.CanSell)
			{
				this.Character.Client.Send(new ExchangeErrorMessage(9));
				result = false;
			}
			else
			{
				BasePlayerItem item = this.Character.Inventory.TryGetItem(guid);
				if (item == null)
				{
					this.Character.Client.Send(new ExchangeErrorMessage(9));
					result = false;
				}
				else
				{
					if (item.Stack < amount)
					{
						this.Character.Client.Send(new ExchangeErrorMessage(9));
						result = false;
					}
					else
					{
						NpcItem npcItem = this.Items.FirstOrDefault((NpcItem entry) => entry.Item.Id == item.Template.Id);
						int num;
						if (npcItem != null)
						{
							num = (int)((long)((int)System.Math.Ceiling(npcItem.Price / 10.0)) * (long)((ulong)amount));
						}
						else
						{
							num = (int)((long)((int)System.Math.Ceiling(item.Template.Price / 10.0)) * (long)((ulong)amount));
						}
						BasicHandler.SendTextInformationMessage(this.Character.Client, TextInformationTypeEnum.TEXT_INFORMATION_MESSAGE, 22, new object[]
						{
							amount,
							item.Template.Id
						});
						this.Character.Inventory.RemoveItem(item, amount, true);
						if (this.Token != null)
						{
							this.Character.Inventory.AddItem(this.Token, (uint)num);
						}
						else
						{
							this.Character.Inventory.AddKamas(num);
							BasicHandler.SendTextInformationMessage(this.Character.Client, TextInformationTypeEnum.TEXT_INFORMATION_MESSAGE, 45, new object[]
							{
								num
							});
						}
						this.Character.Client.Send(new ExchangeSellOkMessage());
						result = true;
					}
				}
			}
			return result;
		}
	}
}
