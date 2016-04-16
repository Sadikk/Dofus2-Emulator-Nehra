using Stump.Core.Attributes;
using Stump.Core.Extensions;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer;
using Stump.Server.BaseServer.Initialization;
using Stump.Server.BaseServer.IPC.Messages;
using Stump.Server.WorldServer.Core.IPC;
using Stump.Server.WorldServer.Database.Items;
using Stump.Server.WorldServer.Database.Items.Templates;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Effects;
using Stump.Server.WorldServer.Game.Effects.Handlers.Items;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Fights;
using Stump.Server.WorldServer.Handlers.Basic;
using Stump.Server.WorldServer.Handlers.Inventory;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Items.Player
{
	public sealed class Inventory : ItemsStorage<BasePlayerItem>, System.IDisposable
	{
		public delegate void ItemMovedEventHandler(Inventory sender, BasePlayerItem item, CharacterInventoryPositionEnum lastPosition);
		[Variable(true)]
		private const int MaxInventoryKamas = 150000000;
		[Variable]
		public static readonly bool ActiveTokens = true;
		[Variable]
		public static readonly int TokenTemplateId = 7919;
		private static ItemTemplate TokenTemplate;
		private readonly System.Collections.Generic.Dictionary<CharacterInventoryPositionEnum, System.Collections.Generic.List<BasePlayerItem>> m_itemsByPosition = new System.Collections.Generic.Dictionary<CharacterInventoryPositionEnum, System.Collections.Generic.List<BasePlayerItem>>
		{

			{
				CharacterInventoryPositionEnum.ACCESSORY_POSITION_HAT,
				new System.Collections.Generic.List<BasePlayerItem>()
			},

			{
				CharacterInventoryPositionEnum.ACCESSORY_POSITION_CAPE,
				new System.Collections.Generic.List<BasePlayerItem>()
			},

			{
				CharacterInventoryPositionEnum.ACCESSORY_POSITION_BELT,
				new System.Collections.Generic.List<BasePlayerItem>()
			},

			{
				CharacterInventoryPositionEnum.ACCESSORY_POSITION_BOOTS,
				new System.Collections.Generic.List<BasePlayerItem>()
			},

			{
				CharacterInventoryPositionEnum.ACCESSORY_POSITION_AMULET,
				new System.Collections.Generic.List<BasePlayerItem>()
			},

			{
				CharacterInventoryPositionEnum.ACCESSORY_POSITION_SHIELD,
				new System.Collections.Generic.List<BasePlayerItem>()
			},

			{
				CharacterInventoryPositionEnum.ACCESSORY_POSITION_WEAPON,
				new System.Collections.Generic.List<BasePlayerItem>()
			},

			{
				CharacterInventoryPositionEnum.ACCESSORY_POSITION_PETS,
				new System.Collections.Generic.List<BasePlayerItem>()
			},

			{
				CharacterInventoryPositionEnum.INVENTORY_POSITION_RING_LEFT,
				new System.Collections.Generic.List<BasePlayerItem>()
			},

			{
				CharacterInventoryPositionEnum.INVENTORY_POSITION_RING_RIGHT,
				new System.Collections.Generic.List<BasePlayerItem>()
			},

			{
				CharacterInventoryPositionEnum.INVENTORY_POSITION_DOFUS_1,
				new System.Collections.Generic.List<BasePlayerItem>()
			},

			{
				CharacterInventoryPositionEnum.INVENTORY_POSITION_DOFUS_2,
				new System.Collections.Generic.List<BasePlayerItem>()
			},

			{
				CharacterInventoryPositionEnum.INVENTORY_POSITION_DOFUS_3,
				new System.Collections.Generic.List<BasePlayerItem>()
			},

			{
				CharacterInventoryPositionEnum.INVENTORY_POSITION_DOFUS_4,
				new System.Collections.Generic.List<BasePlayerItem>()
			},

			{
				CharacterInventoryPositionEnum.INVENTORY_POSITION_DOFUS_5,
				new System.Collections.Generic.List<BasePlayerItem>()
			},

			{
				CharacterInventoryPositionEnum.INVENTORY_POSITION_DOFUS_6,
				new System.Collections.Generic.List<BasePlayerItem>()
			},

			{
				CharacterInventoryPositionEnum.INVENTORY_POSITION_MOUNT,
				new System.Collections.Generic.List<BasePlayerItem>()
			},

			{
				CharacterInventoryPositionEnum.INVENTORY_POSITION_MUTATION,
				new System.Collections.Generic.List<BasePlayerItem>()
			},

			{
				CharacterInventoryPositionEnum.INVENTORY_POSITION_BOOST_FOOD,
				new System.Collections.Generic.List<BasePlayerItem>()
			},

			{
				CharacterInventoryPositionEnum.INVENTORY_POSITION_FIRST_BONUS,
				new System.Collections.Generic.List<BasePlayerItem>()
			},

			{
				CharacterInventoryPositionEnum.INVENTORY_POSITION_SECOND_BONUS,
				new System.Collections.Generic.List<BasePlayerItem>()
			},

			{
				CharacterInventoryPositionEnum.INVENTORY_POSITION_FIRST_MALUS,
				new System.Collections.Generic.List<BasePlayerItem>()
			},

			{
				CharacterInventoryPositionEnum.INVENTORY_POSITION_SECOND_MALUS,
				new System.Collections.Generic.List<BasePlayerItem>()
			},

			{
				CharacterInventoryPositionEnum.INVENTORY_POSITION_ROLEPLAY_BUFFER,
				new System.Collections.Generic.List<BasePlayerItem>()
			},

			{
				CharacterInventoryPositionEnum.INVENTORY_POSITION_FOLLOWER,
				new System.Collections.Generic.List<BasePlayerItem>()
			},

			{
				CharacterInventoryPositionEnum.INVENTORY_POSITION_NOT_EQUIPED,
				new System.Collections.Generic.List<BasePlayerItem>()
			}
		};
		private readonly System.Collections.Generic.Dictionary<ItemSuperTypeEnum, CharacterInventoryPositionEnum[]> m_itemsPositioningRules;
		public event Inventory.ItemMovedEventHandler ItemMoved;
		public Character Owner
		{
			get;
			private set;
		}
		public override int Kamas
		{
			get
			{
				return this.Owner.Kamas;
			}
			protected set
			{
				this.Owner.Kamas = value;
			}
		}
		public BasePlayerItem this[int guid]
		{
			get
			{
				return base.TryGetItem(guid);
			}
		}
		public int Weight
		{
			get
			{
				int num = base.Items.Values.Sum((BasePlayerItem entry) => entry.Weight);
				if (this.Tokens != null)
				{
					num -= this.Tokens.Weight;
				}
				return (num > 0) ? num : 0;
			}
		}
		public uint WeightTotal
		{
			get
			{
				return 1000u;
			}
		}
		public uint WeaponCriticalHit
		{
			get
			{
				BasePlayerItem basePlayerItem;
				uint result;
				if ((basePlayerItem = this.TryGetItem(CharacterInventoryPositionEnum.ACCESSORY_POSITION_WEAPON)) != null)
				{
					result = (uint)((basePlayerItem.Template is WeaponTemplate) ? (basePlayerItem.Template as WeaponTemplate).CriticalHitBonus : 0);
				}
				else
				{
					result = 0u;
				}
				return result;
			}
		}
		public BasePlayerItem Tokens
		{
			get;
			private set;
		}
		[Initialization(typeof(ItemManager), Silent = true)]
		private static void InitializeTokenTemplate()
		{
			if (Inventory.ActiveTokens)
			{
				Inventory.TokenTemplate = Singleton<ItemManager>.Instance.TryGetTemplate(Inventory.TokenTemplateId);
			}
		}
		public void NotifyItemMoved(BasePlayerItem item, CharacterInventoryPositionEnum lastPosition)
		{
			this.OnItemMoved(item, lastPosition);
			Inventory.ItemMovedEventHandler itemMoved = this.ItemMoved;
			if (itemMoved != null)
			{
				itemMoved(this, item, lastPosition);
			}
		}
		public Inventory(Character owner)
		{
			System.Collections.Generic.Dictionary<ItemSuperTypeEnum, CharacterInventoryPositionEnum[]> dictionary = new System.Collections.Generic.Dictionary<ItemSuperTypeEnum, CharacterInventoryPositionEnum[]>();
			System.Collections.Generic.Dictionary<ItemSuperTypeEnum, CharacterInventoryPositionEnum[]> arg_167_0 = dictionary;
			ItemSuperTypeEnum arg_167_1 = ItemSuperTypeEnum.SUPERTYPE_AMULET;
			CharacterInventoryPositionEnum[] value = new CharacterInventoryPositionEnum[1];
			arg_167_0.Add(arg_167_1, value);
			dictionary.Add(ItemSuperTypeEnum.SUPERTYPE_WEAPON, new CharacterInventoryPositionEnum[]
			{
				CharacterInventoryPositionEnum.ACCESSORY_POSITION_WEAPON
			});
			dictionary.Add(ItemSuperTypeEnum.SUPERTYPE_WEAPON_8, new CharacterInventoryPositionEnum[]
			{
				CharacterInventoryPositionEnum.ACCESSORY_POSITION_WEAPON
			});
			dictionary.Add(ItemSuperTypeEnum.SUPERTYPE_CAPE, new CharacterInventoryPositionEnum[]
			{
				CharacterInventoryPositionEnum.ACCESSORY_POSITION_CAPE
			});
			dictionary.Add(ItemSuperTypeEnum.SUPERTYPE_HAT, new CharacterInventoryPositionEnum[]
			{
				CharacterInventoryPositionEnum.ACCESSORY_POSITION_HAT
			});
			dictionary.Add(ItemSuperTypeEnum.SUPERTYPE_RING, new CharacterInventoryPositionEnum[]
			{
				CharacterInventoryPositionEnum.INVENTORY_POSITION_RING_LEFT,
				CharacterInventoryPositionEnum.INVENTORY_POSITION_RING_RIGHT
			});
			dictionary.Add(ItemSuperTypeEnum.SUPERTYPE_BOOTS, new CharacterInventoryPositionEnum[]
			{
				CharacterInventoryPositionEnum.ACCESSORY_POSITION_BOOTS
			});
			dictionary.Add(ItemSuperTypeEnum.SUPERTYPE_BELT, new CharacterInventoryPositionEnum[]
			{
				CharacterInventoryPositionEnum.ACCESSORY_POSITION_BELT
			});
			dictionary.Add(ItemSuperTypeEnum.SUPERTYPE_PET, new CharacterInventoryPositionEnum[]
			{
				CharacterInventoryPositionEnum.ACCESSORY_POSITION_PETS
			});
			dictionary.Add(ItemSuperTypeEnum.SUPERTYPE_DOFUS, new CharacterInventoryPositionEnum[]
			{
				CharacterInventoryPositionEnum.INVENTORY_POSITION_DOFUS_1,
				CharacterInventoryPositionEnum.INVENTORY_POSITION_DOFUS_2,
				CharacterInventoryPositionEnum.INVENTORY_POSITION_DOFUS_3,
				CharacterInventoryPositionEnum.INVENTORY_POSITION_DOFUS_4,
				CharacterInventoryPositionEnum.INVENTORY_POSITION_DOFUS_5,
				CharacterInventoryPositionEnum.INVENTORY_POSITION_DOFUS_6
			});
			dictionary.Add(ItemSuperTypeEnum.SUPERTYPE_SHIELD, new CharacterInventoryPositionEnum[]
			{
				CharacterInventoryPositionEnum.ACCESSORY_POSITION_SHIELD
			});
			dictionary.Add(ItemSuperTypeEnum.SUPERTYPE_BOOST, new CharacterInventoryPositionEnum[]
			{
				CharacterInventoryPositionEnum.INVENTORY_POSITION_BOOST_FOOD
			});
			this.m_itemsPositioningRules = dictionary;
			this.Owner = owner;
			this.InitializeEvents();
		}
		internal void LoadInventory()
		{
			System.Collections.Generic.List<PlayerItemRecord> source = Singleton<ItemManager>.Instance.FindPlayerItems(this.Owner.Id);
			base.Items = (
				from entry in source
				select Singleton<ItemManager>.Instance.LoadPlayerItem(this.Owner, entry)).ToDictionary((BasePlayerItem entry) => entry.Guid);
			foreach (BasePlayerItem current in this)
			{
				this.m_itemsByPosition[current.Position].Add(current);
				if (current.IsEquiped())
				{
					this.ApplyItemEffects(current, false);
				}
			}
			foreach (ItemSetTemplate current2 in (
				from entry in this.GetEquipedItems()
				where entry.Template.ItemSet != null
				select entry.Template.ItemSet).Distinct<ItemSetTemplate>())
			{
				this.ApplyItemSetEffects(current2, this.CountItemSetEquiped(current2), true, false);
			}
			if (Inventory.TokenTemplate != null && Inventory.ActiveTokens && this.Owner.Account.Tokens > 0u)
			{
				this.Tokens = Singleton<ItemManager>.Instance.CreatePlayerItem(this.Owner, Inventory.TokenTemplate, this.Owner.Account.Tokens, false);
				base.Items.Add(this.Tokens.Guid, this.Tokens);
			}
		}
		private void UnLoadInventory()
		{
			base.Items.Clear();
			foreach (System.Collections.Generic.KeyValuePair<CharacterInventoryPositionEnum, System.Collections.Generic.List<BasePlayerItem>> current in this.m_itemsByPosition)
			{
				this.m_itemsByPosition[current.Key].Clear();
			}
		}
		public override void Save()
		{
			lock (base.Locker)
			{
                Stump.ORM.Database database = ServerBase<WorldServer>.Instance.DBAccessor.Database;
				using (System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int, BasePlayerItem>> enumerator = (
					from item in base.Items
					where this.Tokens == null || item.Value != this.Tokens
					select item).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						System.Collections.Generic.KeyValuePair<int, BasePlayerItem> current = enumerator.Current;
						if (current.Value.Record.IsNew)
						{
							database.Insert(current.Value.Record);
							current.Value.Record.IsNew = false;
						}
						else
						{
							if (current.Value.Record.IsDirty)
							{
								database.Update(current.Value.Record);
							}
						}
					}
					goto IL_EF;
				}
				IL_D6:
				BasePlayerItem basePlayerItem = base.ItemsToDelete.Dequeue();
				database.Delete(basePlayerItem.Record);
				IL_EF:
				if (base.ItemsToDelete.Count > 0)
				{
					goto IL_D6;
				}
				if ((this.Tokens == null && this.Owner.Account.Tokens > 0u) || (this.Tokens != null && this.Owner.Account.Tokens != this.Tokens.Stack))
				{
					this.Owner.Account.Tokens = ((this.Tokens == null) ? 0u : this.Tokens.Stack);
					IPCAccessor.Instance.Send(new UpdateAccountMessage(this.Owner.Account));
				}
			}
		}
		public void Dispose()
		{
			this.UnLoadInventory();
			this.TeardownEvents();
		}
		public override void SetKamas(int amount)
		{
			if (amount >= 150000000)
			{
				this.Kamas = 150000000;
				this.Owner.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_MESSAGE, 344, new object[0]);
			}
			base.SetKamas(amount);
		}
		public BasePlayerItem AddItem(ItemTemplate template, uint amount = 1u)
		{
			BasePlayerItem basePlayerItem = base.TryGetItem(template);
			BasePlayerItem result;
			if (basePlayerItem != null && !basePlayerItem.IsEquiped())
			{
				if (!basePlayerItem.OnAddItem())
				{
					result = null;
				}
				else
				{
					this.StackItem(basePlayerItem, amount);
					result = basePlayerItem;
				}
			}
			else
			{
				basePlayerItem = Singleton<ItemManager>.Instance.CreatePlayerItem(this.Owner, template, amount, false);
				result = ((!basePlayerItem.OnAddItem()) ? null : this.AddItem(basePlayerItem));
			}
			return result;
		}
		public override bool RemoveItem(BasePlayerItem item, bool delete = true)
		{
			return item.OnRemoveItem() && base.RemoveItem(item, delete);
		}
		public void RefreshItemInstance(BasePlayerItem item)
		{
			if (base.Items.ContainsKey(item.Guid))
			{
				base.Items.Remove(item.Guid);
				BasePlayerItem basePlayerItem = Singleton<ItemManager>.Instance.RecreateItemInstance(item);
				base.Items.Add(basePlayerItem.Guid, basePlayerItem);
				this.RefreshItem(item);
			}
		}
		public bool CanEquip(BasePlayerItem item, CharacterInventoryPositionEnum position, bool send = true)
		{
			bool result;
			if (this.Owner.IsInFight() && this.Owner.Fight.State != FightState.Placement)
			{
				result = false;
			}
			else
			{
				if (position == CharacterInventoryPositionEnum.INVENTORY_POSITION_NOT_EQUIPED)
				{
					result = true;
				}
				else
				{
					if (!this.GetItemPossiblePositions(item).Contains(position))
					{
						result = false;
					}
					else
					{
						if (item.Template.Level > (uint)this.Owner.Level)
						{
							if (send)
							{
								BasicHandler.SendTextInformationMessage(this.Owner.Client, TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 3);
							}
							result = false;
						}
						else
						{
							BasePlayerItem basePlayerItem = this.TryGetItem(CharacterInventoryPositionEnum.ACCESSORY_POSITION_WEAPON);
							if (item.Template.Type.ItemType == ItemTypeEnum.SHIELD && basePlayerItem != null && basePlayerItem.Template.TwoHanded)
							{
								if (send)
								{
									BasicHandler.SendTextInformationMessage(this.Owner.Client, TextInformationTypeEnum.TEXT_INFORMATION_MESSAGE, 78);
								}
								result = false;
							}
							else
							{
								BasePlayerItem basePlayerItem2 = this.TryGetItem(CharacterInventoryPositionEnum.ACCESSORY_POSITION_SHIELD);
								if (item.Template is WeaponTemplate && item.Template.TwoHanded && basePlayerItem2 != null)
								{
									if (send)
									{
										BasicHandler.SendTextInformationMessage(this.Owner.Client, TextInformationTypeEnum.TEXT_INFORMATION_MESSAGE, 79);
									}
									result = false;
								}
								else
								{
									result = true;
								}
							}
						}
					}
				}
			}
			return result;
		}
		public CharacterInventoryPositionEnum[] GetItemPossiblePositions(BasePlayerItem item)
		{
			return (!this.m_itemsPositioningRules.ContainsKey(item.Template.Type.SuperType)) ? new CharacterInventoryPositionEnum[]
			{
				CharacterInventoryPositionEnum.INVENTORY_POSITION_NOT_EQUIPED
			} : this.m_itemsPositioningRules[item.Template.Type.SuperType];
		}
		public void MoveItem(BasePlayerItem item, CharacterInventoryPositionEnum position)
		{
			if (base.HasItem(item) && position != item.Position)
			{
				CharacterInventoryPositionEnum position2 = item.Position;
				BasePlayerItem basePlayerItem;
				if (position != CharacterInventoryPositionEnum.INVENTORY_POSITION_NOT_EQUIPED && (basePlayerItem = this.TryGetItem(position)) != null)
				{
					if (basePlayerItem.AllowFeeding)
					{
						if (basePlayerItem.Feed(item))
						{
							this.RemoveItem(item, true);
							return;
						}
						return;
					}
					else
					{
						if (item.AllowDropping)
						{
							if (item.Drop(basePlayerItem))
							{
								this.RemoveItem(item, true);
								return;
							}
							return;
						}
						else
						{
							if (this.CanEquip(item, position, false))
							{
								this.MoveItem(basePlayerItem, CharacterInventoryPositionEnum.INVENTORY_POSITION_NOT_EQUIPED);
							}
						}
					}
				}
				if (this.CanEquip(item, position, true) && base.HasItem(item))
				{
					if (position != CharacterInventoryPositionEnum.INVENTORY_POSITION_NOT_EQUIPED)
					{
						this.UnEquipedDouble(item);
					}
					if (item.Stack > 1u)
					{
						this.CutItem(item, item.Stack - 1u);
					}
					item.Position = position;
					BasePlayerItem basePlayerItem2;
					if (position == CharacterInventoryPositionEnum.INVENTORY_POSITION_NOT_EQUIPED && this.IsStackable(item, out basePlayerItem2) && basePlayerItem2 != null)
					{
						this.NotifyItemMoved(item, position2);
						this.StackItem(basePlayerItem2, item.Stack);
						this.RemoveItem(item, true);
					}
					else
					{
						this.NotifyItemMoved(item, position2);
					}
				}
			}
		}
		public MerchantItem MoveToMerchantBag(BasePlayerItem item, uint quantity, uint price)
		{
			MerchantItem result;
			if (!base.HasItem(item))
			{
				result = null;
			}
			else
			{
				if (quantity > item.Stack || quantity == 0u)
				{
					result = null;
				}
				else
				{
					if (item.IsLinked())
					{
						result = null;
					}
					else
					{
						this.RemoveItem(item, quantity, true);
						MerchantItem merchantItem = this.Owner.MerchantBag.FirstOrDefault((MerchantItem x) => x.MustStackWith(item));
						if (merchantItem != null)
						{
							merchantItem.Price = price;
							this.Owner.MerchantBag.StackItem(merchantItem, quantity);
							result = merchantItem;
						}
						else
						{
							MerchantItem merchantItem2 = Singleton<ItemManager>.Instance.CreateMerchantItem(item, quantity, price);
							this.Owner.MerchantBag.AddItem(merchantItem2);
							result = merchantItem2;
						}
					}
				}
			}
			return result;
		}
		private bool UnEquipedDouble(BasePlayerItem itemToEquip)
		{
			bool result;
			if (itemToEquip.Template.Type.ItemType == ItemTypeEnum.DOFUS)
			{
				BasePlayerItem basePlayerItem = this.GetEquipedItems().FirstOrDefault((BasePlayerItem entry) => entry.Guid != itemToEquip.Guid && entry.Template.Id == itemToEquip.Template.Id);
				if (basePlayerItem != null)
				{
					this.MoveItem(basePlayerItem, CharacterInventoryPositionEnum.INVENTORY_POSITION_NOT_EQUIPED);
					result = true;
					return result;
				}
			}
			if (itemToEquip.Template.Type.ItemType == ItemTypeEnum.RING)
			{
				BasePlayerItem basePlayerItem2 = this.GetEquipedItems().FirstOrDefault((BasePlayerItem entry) => entry.Guid != itemToEquip.Guid && entry.Template.Id == itemToEquip.Template.Id && entry.Template.ItemSetId > 0);
				if (basePlayerItem2 != null)
				{
					this.MoveItem(basePlayerItem2, CharacterInventoryPositionEnum.INVENTORY_POSITION_NOT_EQUIPED);
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}
		public void ChangeItemOwner(Character newOwner, BasePlayerItem item, uint amount)
		{
			if (base.HasItem(item.Guid))
			{
				if (amount > item.Stack)
				{
					amount = item.Stack;
				}
				if (amount >= item.Stack)
				{
					this.RemoveItem(item, true);
				}
				else
				{
					this.UnStackItem(item, amount);
				}
				BasePlayerItem item2 = Singleton<ItemManager>.Instance.CreatePlayerItem(newOwner, item, amount);
				newOwner.Inventory.AddItem(item2);
			}
		}
		public void CheckItemsCriterias()
		{
			foreach (BasePlayerItem current in 
				from equipedItem in this.GetEquipedItems().ToArray<BasePlayerItem>()
				where !equipedItem.AreConditionFilled(this.Owner)
				select equipedItem)
			{
				this.MoveItem(current, CharacterInventoryPositionEnum.INVENTORY_POSITION_NOT_EQUIPED);
			}
		}
		public bool CanUseItem(BasePlayerItem item, bool send = true)
		{
			bool result;
			if (!base.HasItem(item.Guid) || !item.IsUsable())
			{
				result = false;
			}
			else
			{
				if (this.Owner.IsInFight() && this.Owner.Fight.State != FightState.Placement)
				{
					result = false;
				}
				else
				{
					if (!item.AreConditionFilled(this.Owner))
					{
						if (send)
						{
							BasicHandler.SendTextInformationMessage(this.Owner.Client, TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 19);
						}
						result = false;
					}
					else
					{
						if (item.Template.Level > (uint)this.Owner.Level)
						{
							if (send)
							{
								BasicHandler.SendTextInformationMessage(this.Owner.Client, TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 3);
							}
							result = false;
						}
						else
						{
							result = true;
						}
					}
				}
			}
			return result;
		}
		public void UseItem(BasePlayerItem item, uint amount = 1u)
		{
			this.UseItem(item, amount, null, null);
		}
		public void UseItem(BasePlayerItem item, Cell targetCell, uint amount = 1u)
		{
			this.UseItem(item, amount, targetCell, null);
		}
		public void UseItem(BasePlayerItem item, Character target, uint amount = 1u)
		{
			this.UseItem(item, amount, null, target);
		}
		public void UseItem(BasePlayerItem item, uint amount, Cell targetCell, Character target)
		{
			if (this.CanUseItem(item, true))
			{
				if (amount > item.Stack)
				{
					amount = item.Stack;
				}
				uint num = item.UseItem(amount, targetCell, target);
				if (num > 0u)
				{
					this.RemoveItem(item, num, true);
				}
			}
		}
		public BasePlayerItem CutItem(BasePlayerItem item, uint amount)
		{
			BasePlayerItem result;
			if (amount >= item.Stack)
			{
				result = item;
			}
			else
			{
				this.UnStackItem(item, amount);
				BasePlayerItem basePlayerItem = Singleton<ItemManager>.Instance.CreatePlayerItem(this.Owner, item, amount);
				base.Items.Add(basePlayerItem.Guid, basePlayerItem);
				base.NotifyItemAdded(basePlayerItem);
				result = basePlayerItem;
			}
			return result;
		}
		private void ApplyItemEffects(BasePlayerItem item, bool send = true)
		{
			foreach (ItemEffectHandler current in 
				from effect in item.Effects
				select Singleton<EffectManager>.Instance.GetItemEffectHandler(effect, this.Owner, item))
			{
				current.Apply();
			}
			if (send)
			{
				this.Owner.RefreshStats();
			}
		}
		private void ApplyItemSetEffects(ItemSetTemplate itemSet, int count, bool apply, bool send = true)
		{
			EffectBase[] effects = itemSet.GetEffects(count);
			foreach (ItemEffectHandler current in 
				from effect in effects
				select Singleton<EffectManager>.Instance.GetItemEffectHandler(effect, this.Owner, itemSet, apply))
			{
				current.Apply();
			}
			if (send)
			{
				this.Owner.RefreshStats();
			}
		}
		protected override void DeleteItem(BasePlayerItem item)
		{
			if (item != this.Tokens)
			{
				base.DeleteItem(item);
			}
		}
		protected override void OnItemAdded(BasePlayerItem item)
		{
			this.m_itemsByPosition[item.Position].Add(item);
			if (item.IsEquiped())
			{
				this.ApplyItemEffects(item, true);
			}
			InventoryHandler.SendObjectAddedMessage(this.Owner.Client, item);
			InventoryHandler.SendInventoryWeightMessage(this.Owner.Client);
			base.OnItemAdded(item);
		}
		protected override void OnItemRemoved(BasePlayerItem item)
		{
			this.m_itemsByPosition[item.Position].Remove(item);
			if (item == this.Tokens)
			{
				this.Tokens = null;
			}
			bool flag = item.IsEquiped();
			item.Position = CharacterInventoryPositionEnum.INVENTORY_POSITION_NOT_EQUIPED;
			if (flag)
			{
				this.ApplyItemEffects(item, item.Template.ItemSet == null);
			}
			if (flag && item.Template.ItemSet != null)
			{
				int num = this.CountItemSetEquiped(item.Template.ItemSet);
				if (num >= 0)
				{
					this.ApplyItemSetEffects(item.Template.ItemSet, num + 1, false, true);
				}
				if (num > 0)
				{
					this.ApplyItemSetEffects(item.Template.ItemSet, num, true, true);
				}
				InventoryHandler.SendSetUpdateMessage(this.Owner.Client, item.Template.ItemSet);
			}
			InventoryHandler.SendObjectDeletedMessage(this.Owner.Client, item.Guid);
			InventoryHandler.SendInventoryWeightMessage(this.Owner.Client);
			if (flag)
			{
				this.CheckItemsCriterias();
			}
			if (flag && item.AppearanceId != 0u)
			{
				this.Owner.UpdateLook(true);
			}
			base.OnItemRemoved(item);
		}
		private void OnItemMoved(BasePlayerItem item, CharacterInventoryPositionEnum lastPosition)
		{
			this.m_itemsByPosition[lastPosition].Remove(item);
			this.m_itemsByPosition[item.Position].Add(item);
			bool flag = lastPosition != CharacterInventoryPositionEnum.INVENTORY_POSITION_NOT_EQUIPED;
			bool flag2 = item.IsEquiped();
			if ((flag && !flag2) || (!flag && flag2))
			{
				this.ApplyItemEffects(item, false);
			}
			if (item.OnEquipItem(flag))
			{
				if (item.Template.ItemSet != null && (!flag || !flag2))
				{
					int num = this.CountItemSetEquiped(item.Template.ItemSet);
					if (num >= 0)
					{
						this.ApplyItemSetEffects(item.Template.ItemSet, num + (flag ? 1 : -1), false, true);
					}
					if (num > 0)
					{
						this.ApplyItemSetEffects(item.Template.ItemSet, num, true, false);
					}
					InventoryHandler.SendSetUpdateMessage(this.Owner.Client, item.Template.ItemSet);
				}
				if (lastPosition == CharacterInventoryPositionEnum.INVENTORY_POSITION_NOT_EQUIPED && !item.AreConditionFilled(this.Owner))
				{
					BasicHandler.SendTextInformationMessage(this.Owner.Client, TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 19);
					this.MoveItem(item, lastPosition);
				}
				InventoryHandler.SendObjectMovementMessage(this.Owner.Client, item);
				InventoryHandler.SendInventoryWeightMessage(this.Owner.Client);
				if (lastPosition != CharacterInventoryPositionEnum.INVENTORY_POSITION_NOT_EQUIPED)
				{
					this.CheckItemsCriterias();
				}
				if ((flag2 || flag) && item.AppearanceId != 0u)
				{
					this.Owner.UpdateLook(true);
				}
				this.Owner.RefreshActor();
				this.Owner.RefreshStats();
			}
		}
		protected override void OnItemStackChanged(BasePlayerItem item, int difference)
		{
			InventoryHandler.SendObjectQuantityMessage(this.Owner.Client, item);
			InventoryHandler.SendInventoryWeightMessage(this.Owner.Client);
			base.OnItemStackChanged(item, difference);
		}
		protected override void OnKamasAmountChanged(int amount)
		{
			InventoryHandler.SendKamasUpdateMessage(this.Owner.Client, amount);
			base.OnKamasAmountChanged(amount);
		}
		public void RefreshItem(BasePlayerItem item)
		{
			InventoryHandler.SendObjectModifiedMessage(this.Owner.Client, item);
		}
		public override bool IsStackable(BasePlayerItem item, out BasePlayerItem stackableWith)
		{
			BasePlayerItem basePlayerItem;
			bool result;
			if ((basePlayerItem = this.TryGetItem(item.Template, item.Effects, item.Position, item)) != null)
			{
				stackableWith = basePlayerItem;
				result = true;
			}
			else
			{
				stackableWith = null;
				result = false;
			}
			return result;
		}
		public BasePlayerItem TryGetItem(CharacterInventoryPositionEnum position)
		{
			return base.Items.Values.FirstOrDefault((BasePlayerItem entry) => entry.Position == position);
		}
		public BasePlayerItem TryGetItem(ItemTemplate template, System.Collections.Generic.IEnumerable<EffectBase> effects, CharacterInventoryPositionEnum position, BasePlayerItem except)
		{
			System.Collections.Generic.IEnumerable<BasePlayerItem> source = 
				from entry in base.Items.Values
				where entry != except && entry.Template.Id == template.Id && entry.Position == position && effects.CompareEnumerable(entry.Effects)
				select entry;
			return source.FirstOrDefault<BasePlayerItem>();
		}
		public BasePlayerItem[] GetItems(CharacterInventoryPositionEnum position)
		{
			return (
				from entry in base.Items.Values
				where entry.Position == position
				select entry).ToArray<BasePlayerItem>();
		}
		public BasePlayerItem[] GetEquipedItems()
		{
			return (
				from entry in base.Items
				where entry.Value.IsEquiped()
				select entry.Value).ToArray<BasePlayerItem>();
		}
		public int CountItemSetEquiped(ItemSetTemplate itemSet)
		{
			return this.GetEquipedItems().Count((BasePlayerItem entry) => itemSet.Items.Contains(entry.Template));
		}
		public BasePlayerItem[] GetItemSetEquipped(ItemSetTemplate itemSet)
		{
			return (
				from entry in this.GetEquipedItems()
				where itemSet.Items.Contains(entry.Template)
				select entry).ToArray<BasePlayerItem>();
		}
		public EffectBase[] GetItemSetEffects(ItemSetTemplate itemSet)
		{
			return itemSet.GetEffects(this.CountItemSetEquiped(itemSet));
		}
		public short[] GetItemsSkins()
		{
			return (
				from entry in this.GetEquipedItems()
				where entry.Position != CharacterInventoryPositionEnum.ACCESSORY_POSITION_PETS && entry.AppearanceId != 0u
				select (short)entry.AppearanceId).ToArray<short>();
		}
		public short? GetPetSkin()
		{
			BasePlayerItem basePlayerItem = this.TryGetItem(CharacterInventoryPositionEnum.ACCESSORY_POSITION_PETS);
			short? result;
			if (basePlayerItem == null || basePlayerItem.AppearanceId == 0u)
			{
				result = null;
			}
			else
			{
				result = new short?((short)basePlayerItem.AppearanceId);
			}
			return result;
		}
		private void InitializeEvents()
		{
			this.Owner.FightEnded += new Character.CharacterFightEndedHandler(this.OnFightEnded);
		}
		private void TeardownEvents()
		{
			this.Owner.FightEnded -= new Character.CharacterFightEndedHandler(this.OnFightEnded);
		}
		private void OnFightEnded(Character character, CharacterFighter fighter)
		{
			BasePlayerItem[] items = this.GetItems(CharacterInventoryPositionEnum.INVENTORY_POSITION_BOOST_FOOD);
			for (int i = 0; i < items.Length; i++)
			{
				BasePlayerItem basePlayerItem = items[i];
				EffectMinMax effectMinMax = basePlayerItem.Effects.OfType<EffectMinMax>().FirstOrDefault((EffectMinMax x) => x.EffectId == EffectsEnum.Effect_RemainingFights);
				if (!(effectMinMax == null))
				{
					EffectMinMax expr_4C = effectMinMax;
					expr_4C.ValueMax -= 1;
					if (effectMinMax.ValueMax <= 0)
					{
						this.RemoveItem(basePlayerItem, true);
					}
					else
					{
						this.RefreshItem(basePlayerItem);
					}
				}
			}
		}
	}
}
