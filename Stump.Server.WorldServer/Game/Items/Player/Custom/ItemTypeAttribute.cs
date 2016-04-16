using Stump.DofusProtocol.Enums;

namespace Stump.Server.WorldServer.Game.Items.Player.Custom
{
	public class ItemTypeAttribute : System.Attribute
	{
		public ItemTypeEnum ItemType
		{
			get;
			set;
		}
		public ItemTypeAttribute(ItemTypeEnum itemType)
		{
			this.ItemType = itemType;
		}
	}
}
