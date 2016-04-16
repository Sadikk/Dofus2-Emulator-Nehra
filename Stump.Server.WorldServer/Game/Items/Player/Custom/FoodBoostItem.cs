using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.Items;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Items.Player.Custom
{
	[ItemType(ItemTypeEnum.BOOST_FOOD)]
	public class FoodBoostItem : BasePlayerItem
	{
		public FoodBoostItem(Character owner, PlayerItemRecord record) : base(owner, record)
		{
		}
		public override bool OnEquipItem(bool unequip)
		{
			bool result;
			if (unequip)
			{
				base.Owner.Inventory.RemoveItem(this, true);
				result = false;
			}
			else
			{
				result = base.OnEquipItem(false);
			}
			return result;
		}
	}
}
