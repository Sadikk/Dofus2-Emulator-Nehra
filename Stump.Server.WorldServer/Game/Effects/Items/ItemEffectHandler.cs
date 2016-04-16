using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.Items.Templates;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Items.Player;

namespace Stump.Server.WorldServer.Game.Effects.Handlers.Items
{
	public abstract class ItemEffectHandler : EffectHandler
	{
		public enum HandlerOperation
		{
			APPLY,
			UNAPPLY
		}
		private ItemEffectHandler.HandlerOperation? m_operation;
		public Character Target
		{
			get;
			protected set;
		}
		public BasePlayerItem Item
		{
			get;
			protected set;
		}
		public ItemSetTemplate ItemSet
		{
			get;
			protected set;
		}
		public bool ItemSetApply
		{
			get;
			set;
		}
		public bool Equiped
		{
			get
			{
				return this.Item != null && this.Item.IsEquiped();
			}
		}
		public bool Boost
		{
			get
			{
				return this.Item != null && this.Item.Template.Type.SuperType == ItemSuperTypeEnum.SUPERTYPE_BOOST;
			}
		}
		public ItemEffectHandler.HandlerOperation Operation
		{
			get
			{
				return this.m_operation.HasValue ? this.m_operation.Value : ((this.Equiped || this.ItemSetApply) ? ItemEffectHandler.HandlerOperation.APPLY : ItemEffectHandler.HandlerOperation.UNAPPLY);
			}
			set
			{
				this.m_operation = new ItemEffectHandler.HandlerOperation?(value);
			}
		}
		protected ItemEffectHandler(EffectBase effect, Character target, BasePlayerItem item) : base(effect)
		{
			this.Target = target;
			this.Item = item;
		}
		protected ItemEffectHandler(EffectBase effect, Character target, ItemSetTemplate itemSet, bool apply) : base(effect)
		{
			this.Target = target;
			this.ItemSet = itemSet;
			this.ItemSetApply = apply;
		}
	}
}
