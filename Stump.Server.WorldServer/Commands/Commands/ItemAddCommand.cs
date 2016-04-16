using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.WorldServer.Commands.Commands.Patterns;
using Stump.Server.WorldServer.Commands.Trigger;
using Stump.Server.WorldServer.Database.Items.Templates;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Items;
using Stump.Server.WorldServer.Game.Items.Player;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class ItemAddCommand : TargetSubCommand
	{
		public ItemAddCommand()
		{
			base.Aliases = new string[]
			{
				"add",
				"new"
			};
			base.RequiredRole = RoleEnum.GameMaster;
			base.Description = "Add an item to the targeted character";
			base.ParentCommand = typeof(ItemCommand);
			base.AddParameter<ItemTemplate>("template", "item", "Item to add", null, false, ParametersConverter.ItemTemplateConverter);
			base.AddTargetParameter(true, "Character who will receive the item");
			base.AddParameter<uint>("amount", "amount", "Amount of items to add", 1u, false, null);
			base.AddParameter<bool>("max", "max", "Set item's effect to maximal values", false, true, null);
		}
		public override void Execute(TriggerBase trigger)
		{
			ItemTemplate itemTemplate = trigger.Get<ItemTemplate>("template");
			Character[] targets = base.GetTargets(trigger);
			for (int i = 0; i < targets.Length; i++)
			{
				Character character = targets[i];
				BasePlayerItem basePlayerItem = Singleton<ItemManager>.Instance.CreatePlayerItem(character, itemTemplate, trigger.Get<uint>("amount"), trigger.IsArgumentDefined("max"));
				character.Inventory.AddItem(basePlayerItem);
				if (basePlayerItem == null)
				{
					trigger.ReplyError("Item '{0}'({1}) can't be add for an unknown reason", new object[]
					{
						itemTemplate.Name,
						itemTemplate.Id
					});
				}
				else
				{
					if (trigger is GameTrigger && (trigger as GameTrigger).Character.Id == character.Id)
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
							character.Name
						});
					}
				}
			}
		}
	}
}
