using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.Items.Templates;
using Stump.Server.WorldServer.Game.Effects.Instances;

namespace Stump.Server.WorldServer.Game.Exchanges.Items
{
	public class NpcTradeItem : TradeItem
	{
		private readonly int m_guid;
		private readonly ItemTemplate m_template;
		private uint m_stack;
		public override int Guid
		{
			get
			{
				return this.m_guid;
			}
		}
		public override ItemTemplate Template
		{
			get
			{
				return this.m_template;
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
				return this.m_template.Effects;
			}
		}
		public override CharacterInventoryPositionEnum Position
		{
			get
			{
				return CharacterInventoryPositionEnum.INVENTORY_POSITION_NOT_EQUIPED;
			}
		}
		public NpcTradeItem(int guid, ItemTemplate template, uint stack)
		{
			this.m_guid = guid;
			this.m_template = template;
			this.m_stack = stack;
		}
	}
}
