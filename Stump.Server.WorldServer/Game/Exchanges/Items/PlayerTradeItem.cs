using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.Items.Templates;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Items.Player;

namespace Stump.Server.WorldServer.Game.Exchanges.Items
{
	public class PlayerTradeItem : TradeItem
	{
		private readonly BasePlayerItem m_item;
		private uint m_stack;
		public override int Guid
		{
			get
			{
				return this.m_item.Guid;
			}
		}
		public override ItemTemplate Template
		{
			get
			{
				return this.m_item.Template;
			}
		}
		public override uint Stack
		{
			get
			{
				return this.m_stack;
			}
			set
			{
				this.m_stack = value;
			}
		}
		public override System.Collections.Generic.List<EffectBase> Effects
		{
			get
			{
				return this.m_item.Effects;
			}
		}
		public override CharacterInventoryPositionEnum Position
		{
			get
			{
				return CharacterInventoryPositionEnum.INVENTORY_POSITION_NOT_EQUIPED;
			}
		}
		public PlayerTradeItem(BasePlayerItem item, uint stack)
		{
			this.m_item = item;
			this.m_stack = stack;
		}
	}
}
