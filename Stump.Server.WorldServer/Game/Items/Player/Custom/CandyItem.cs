using NLog;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.Items;
using Stump.Server.WorldServer.Database.Items.Templates;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Conditions.Criterions;

namespace Stump.Server.WorldServer.Game.Items.Player.Custom
{
	[ItemType(ItemTypeEnum.CANDY)]
	public class CandyItem : BasePlayerItem
	{
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();
		public CandyItem(Character owner, PlayerItemRecord record) : base(owner, record)
		{
		}
		public override uint UseItem(uint amount = 1u, Cell targetCell = null, Character target = null)
		{
			HasItemCriterion hasItemCriterion = this.Template.CriteriaExpression as HasItemCriterion;
			uint result;
			if (hasItemCriterion == null)
			{
				result = base.UseItem(amount, targetCell, target);
			}
			else
			{
				ItemTemplate itemTemplate = Singleton<ItemManager>.Instance.TryGetTemplate(hasItemCriterion.Item);
				if (itemTemplate == null)
				{
					CandyItem.logger.Error(string.Format("Candy {0} has boostItem {1} but it doesn't exist", this.Template.Id, hasItemCriterion.Item));
					result = 0u;
				}
				else
				{
					base.Owner.Inventory.MoveItem(base.Owner.Inventory.AddItem(itemTemplate, 1u), CharacterInventoryPositionEnum.INVENTORY_POSITION_BOOST_FOOD);
					result = 1u;
				}
			}
			return result;
		}
	}
}
