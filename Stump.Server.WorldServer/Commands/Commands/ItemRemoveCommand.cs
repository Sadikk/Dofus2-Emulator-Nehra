using Stump.Core.Attributes;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Messages;
using Stump.Server.BaseServer.Commands;
using Stump.Server.WorldServer.Commands.Commands.Patterns;
using Stump.Server.WorldServer.Commands.Trigger;
using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Database.Items.Templates;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Items;
using Stump.Server.WorldServer.Game.Items.Player;
using System.Linq;
namespace Stump.Server.WorldServer.Commands.Commands
{
	public class ItemRemoveCommand : TargetSubCommand
	{
		public class ItemListCommand : SubCommand
		{
			[Variable]
			public static readonly int LimitItemList = 50;
			public ItemListCommand()
			{
				base.Aliases = new string[]
				{
					"list",
					"ls"
				};
				base.RequiredRole = RoleEnum.GameMaster;
				base.Description = "Lists loaded items or items from an inventory with a search pattern";
				base.ParentCommand = typeof(ItemCommand);
				base.AddParameter<string>("pattern", "p", "Search pattern (see docs)", "*", false, null);
				string arg_7B_1 = "target";
				string arg_7B_2 = "t";
				string arg_7B_3 = "Where items will be search";
				ConverterHandler<Character> characterConverter = ParametersConverter.CharacterConverter;
				base.AddParameter<Character>(arg_7B_1, arg_7B_2, arg_7B_3, null, true, characterConverter);
				base.AddParameter<int>("page", "page", "Page number of the list (starts at 0)", 0, true, null);
			}
			public override void Execute(TriggerBase trigger)
			{
				if (trigger.IsArgumentDefined("target"))
				{
					Character character = trigger.Get<Character>("target");
					System.Collections.Generic.IEnumerable<BasePlayerItem> itemsByPattern = Singleton<ItemManager>.Instance.GetItemsByPattern(trigger.Get<string>("pattern"), character.Inventory);
					using (System.Collections.Generic.IEnumerator<BasePlayerItem> enumerator = itemsByPattern.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							BasePlayerItem current = enumerator.Current;
							trigger.Reply("'{0}'({1}) Amount:{2} Guid:{3}", new object[]
							{
								current.Template.Name,
								current.Template.Id,
								current.Stack,
								current.Guid
							});
						}
						return;
					}
				}
				System.Collections.Generic.IEnumerable<ItemTemplate> itemsByPattern2 = Singleton<ItemManager>.Instance.GetItemsByPattern(trigger.Get<string>("pattern"));
				int num = trigger.Get<int>("page") * ItemRemoveCommand.ItemListCommand.LimitItemList;
				int num2 = 0;
				System.Collections.Generic.IEnumerator<ItemTemplate> enumerator2 = itemsByPattern2.GetEnumerator();
				int num3 = 0;
				while (enumerator2.MoveNext())
				{
					if (num3 >= num)
					{
						ItemTemplate current2 = enumerator2.Current;
						if (num2 < ItemRemoveCommand.ItemListCommand.LimitItemList)
						{
							trigger.Reply("'{0}'({1})", new object[]
							{
								current2.Name,
								current2.Id
							});
							num2++;
						}
						else
						{
							trigger.Reply("... (limit reached : {0})", new object[]
							{
								ItemRemoveCommand.ItemListCommand.LimitItemList
							});
							if (num2 == 0)
							{
								trigger.Reply("No results");
								return;
							}
							return;
						}
					}
					num3++;
				}
			}
		}
		public class ItemShowInvCommand : SubCommand
		{
			public ItemShowInvCommand()
			{
				base.Aliases = new string[]
				{
					"showinv"
				};
				base.RequiredRole = RoleEnum.Moderator;
				base.Description = "Show items of the target into your inventory";
				base.ParentCommand = typeof(ItemCommand);
				string arg_57_1 = "target";
				string arg_57_2 = "t";
				string arg_57_3 = "Where items will be search";
				ConverterHandler<Character> characterConverter = ParametersConverter.CharacterConverter;
				base.AddParameter<Character>(arg_57_1, arg_57_2, arg_57_3, null, true, characterConverter);
			}
			public override void Execute(TriggerBase trigger)
			{
				if (trigger.IsArgumentDefined("target"))
				{
					Character character = trigger.Get<Character>("target");
					WorldClient client = ((GameTrigger)trigger).Character.Client;
					client.Send(new InventoryContentMessage(
						from entry in character.Inventory
						select entry.GetObjectItem(), (uint)character.Inventory.Kamas));
				}
				else
				{
					trigger.ReplyError("Please define a target");
				}
			}
		}
		public class ItemAddSetCommand : TargetSubCommand
		{
			public ItemAddSetCommand()
			{
				base.Aliases = new string[]
				{
					"addset"
				};
				base.RequiredRole = RoleEnum.GameMaster;
				base.Description = "Add the entire itemset to the targeted character";
				base.ParentCommand = typeof(ItemCommand);
				base.AddParameter<ItemSetTemplate>("template", "itemset", "Itemset to add", null, false, ParametersConverter.ItemSetTemplateConverter);
				base.AddTargetParameter(true, "Character who will receive the itemset");
				base.AddParameter<bool>("max", "max", "Set item's effect to maximal values", false, true, null);
			}
			public override void Execute(TriggerBase trigger)
			{
				ItemSetTemplate itemSetTemplate = trigger.Get<ItemSetTemplate>("template");
				Character target = base.GetTarget(trigger);
				ItemTemplate[] items = itemSetTemplate.Items;
				for (int i = 0; i < items.Length; i++)
				{
					ItemTemplate itemTemplate = items[i];
					BasePlayerItem basePlayerItem = Singleton<ItemManager>.Instance.CreatePlayerItem(target, itemTemplate, 1u, trigger.IsArgumentDefined("max"));
					target.Inventory.AddItem(basePlayerItem);
					if (basePlayerItem == null)
					{
						trigger.Reply("Item '{0}'({1}) can't be add for an unknown reason", new object[]
						{
							itemTemplate.Name,
							itemTemplate.Id
						});
					}
					else
					{
						if (trigger is GameTrigger && (trigger as GameTrigger).Character.Id == target.Id)
						{
							trigger.Reply("Added '{0}'({1}) to your inventory.", new object[]
							{
								itemTemplate.Name,
								itemTemplate.Id
							});
						}
						else
						{
							trigger.Reply("Added '{0}'({1}) to '{2}' inventory.", new object[]
							{
								itemTemplate.Name,
								itemTemplate.Id,
								target.Name
							});
						}
					}
				}
			}
		}
		public class ItemAddTypeCommand : TargetSubCommand
		{
			public ItemAddTypeCommand()
			{
				base.Aliases = new string[]
				{
					"addtype"
				};
				base.RequiredRole = RoleEnum.GameMaster;
				base.Description = "Add all the items match with typeId.";
				base.ParentCommand = typeof(ItemCommand);
				base.AddParameter<int>("typeid", "type", "TypeId to add", 0, false, null);
				base.AddParameter<bool>("etheral", "eth", "Etheral", false, false, null);
				base.AddTargetParameter(true, "Character who will receive the items");
			}
			public override void Execute(TriggerBase trigger)
			{
				int num = trigger.Get<int>("typeid");
				bool flag = trigger.Get<bool>("etheral");
				Character target = base.GetTarget(trigger);
				System.Collections.Generic.IEnumerable<ItemTemplate> templates = Singleton<ItemManager>.Instance.GetTemplates();
				foreach (ItemTemplate current in templates)
				{
					if ((ulong)current.TypeId == (ulong)((long)num) && (!current.IsWeapon() || current.Etheral == flag))
					{
						BasePlayerItem basePlayerItem = Singleton<ItemManager>.Instance.CreatePlayerItem(target, current, 1u, false);
						target.Inventory.AddItem(basePlayerItem);
						if (basePlayerItem == null)
						{
							trigger.Reply("Item '{0}'({1}) can't be add for an unknown reason", new object[]
							{
								current.Name,
								current.Id
							});
						}
						else
						{
							if (trigger is GameTrigger && (trigger as GameTrigger).Character.Id == target.Id)
							{
								trigger.Reply("Added '{0}'({1}) to your inventory.", new object[]
								{
									current.Name,
									current.Id
								});
							}
							else
							{
								trigger.Reply("Added '{0}'({1}) to '{2}' inventory.", new object[]
								{
									current.Name,
									current.Id,
									target.Name
								});
							}
						}
					}
				}
			}
		}
		public class ItemDelTypeCommand : TargetSubCommand
		{
			public ItemDelTypeCommand()
			{
				base.Aliases = new string[]
				{
					"deltype"
				};
				base.RequiredRole = RoleEnum.GameMaster;
				base.Description = "Remove all the items match with typeId.";
				base.ParentCommand = typeof(ItemCommand);
				base.AddParameter<int>("typeid", "type", "TypeId to remove", 0, false, null);
				base.AddTargetParameter(true, "Character who will remove the items");
			}
			public override void Execute(TriggerBase trigger)
			{
				int typeId = trigger.Get<int>("typeid");
				Character target = base.GetTarget(trigger);
				BasePlayerItem[] source = (
					from x in target.Inventory
					where (ulong)x.Template.TypeId == (ulong)((long)typeId)
					select x).ToArray<BasePlayerItem>();
				foreach (BasePlayerItem current in 
					from item in source
					where (ulong)item.Template.TypeId == (ulong)((long)typeId)
					select item)
				{
					target.Inventory.RemoveItem(current, true);
					trigger.ReplyBold("Item {0} removed from {1}'s inventory", new object[]
					{
						current.Template.Name,
						target
					});
				}
			}
		}
		public ItemRemoveCommand()
		{
			base.Aliases = new string[]
			{
				"remove",
				"delete"
			};
			base.RequiredRole = RoleEnum.GameMaster;
			base.Description = "Delete an item from the target";
			base.ParentCommand = typeof(ItemCommand);
			base.AddParameter<ItemTemplate>("template", "item", "Item to remove", null, false, ParametersConverter.ItemTemplateConverter);
			base.AddTargetParameter(true, "Character who will lose the item");
			base.AddParameter<uint>("amount", "amount", "Amount of items to remove", 0u, true, null);
		}
		public override void Execute(TriggerBase trigger)
		{
			ItemTemplate itemTemplate = trigger.Get<ItemTemplate>("template");
			Character[] targets = base.GetTargets(trigger);
			for (int i = 0; i < targets.Length; i++)
			{
				Character character = targets[i];
				BasePlayerItem basePlayerItem = character.Inventory.TryGetItem(itemTemplate);
				if (basePlayerItem != null)
				{
					if (trigger.IsArgumentDefined("amount"))
					{
						character.Inventory.RemoveItem(basePlayerItem, trigger.Get<uint>("amount"), true);
						trigger.ReplyBold("'{0}'x{1} removed from {1}'s inventory", new object[]
						{
							itemTemplate.Name,
							trigger.Get<uint>("amount"),
							character
						});
					}
					else
					{
						character.Inventory.RemoveItem(basePlayerItem, true);
						trigger.ReplyBold("Item {0} removed from {1}'s inventory", new object[]
						{
							itemTemplate.Name,
							character
						});
					}
				}
				else
				{
					trigger.ReplyError("{0} hasn't item {1}");
				}
			}
		}
	}
}
