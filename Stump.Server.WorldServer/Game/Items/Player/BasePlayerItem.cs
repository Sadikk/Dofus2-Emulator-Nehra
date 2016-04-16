using Stump.Core.Cache;
using Stump.Core.Extensions;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Database.Items;
using Stump.Server.WorldServer.Database.Items.Templates;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Effects;
using Stump.Server.WorldServer.Game.Effects.Handlers.Usables;
using Stump.Server.WorldServer.Game.Effects.Instances;
using System;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Items.Player
{
	public abstract class BasePlayerItem : Item<PlayerItemRecord>
	{
		private readonly ObjectValidator<ObjectItem> m_objectItemValidator;
		public Character Owner
		{
			get;
			private set;
		}
		public virtual bool AllowFeeding
		{
			get
			{
				return false;
			}
		}
		public virtual bool AllowDropping
		{
			get
			{
				return false;
			}
		}
		public override int Guid
		{
			get
			{
				return base.Guid;
			}
			protected set
			{
				base.Guid = value;
				this.Invalidate();
			}
		}
		public override ItemTemplate Template
		{
			get
			{
				return base.Template;
			}
			protected set
			{
				base.Template = value;
				this.Invalidate();
			}
		}
		public override uint Stack
		{
			get
			{
				return base.Stack;
			}
			set
			{
				base.Stack = value;
				this.Invalidate();
			}
		}
		public override System.Collections.Generic.List<EffectBase> Effects
		{
			get
			{
				return base.Effects;
			}
			protected set
			{
				base.Effects = value;
				this.Invalidate();
			}
		}
		public virtual CharacterInventoryPositionEnum Position
		{
			get
			{
				return base.Record.Position;
			}
			set
			{
				base.Record.Position = value;
				this.Invalidate();
			}
		}
		public virtual uint AppearanceId
		{
			get
			{
				return this.Template.AppearanceId;
			}
		}
		public virtual int Weight
		{
			get
			{
				return (int)(this.Template.RealWeight * this.Stack);
			}
		}
		public BasePlayerItem(Character owner, PlayerItemRecord record) : base(record)
		{
			this.m_objectItemValidator = new ObjectValidator<ObjectItem>(new Func<ObjectItem>(this.BuildObjectItem));
			this.Owner = owner;
		}
		public virtual bool AreConditionFilled(Character character)
		{
			bool result;
			try
			{
				result = (this.Template.CriteriaExpression == null || this.Template.CriteriaExpression.Eval(character));
			}
			catch
			{
				result = false;
			}
			return result;
		}
		public virtual bool IsStackableWith(BasePlayerItem compared)
		{
			return compared.Template.Id == this.Template.Id && compared.Position == CharacterInventoryPositionEnum.INVENTORY_POSITION_NOT_EQUIPED && compared.Effects.CompareEnumerable(this.Effects);
		}
		public virtual bool MustStackWith(BasePlayerItem compared)
		{
			return compared.Template.Id == this.Template.Id && compared.Position == CharacterInventoryPositionEnum.INVENTORY_POSITION_NOT_EQUIPED && compared.Position == this.Position && compared.Effects.CompareEnumerable(this.Effects);
		}
		public virtual bool IsLinked()
		{
			bool result;
			if (this.Template.IsLinkedToOwner)
			{
				result = true;
			}
			else
			{
				if (this.Template.Type.SuperType == ItemSuperTypeEnum.SUPERTYPE_QUEST)
				{
					result = true;
				}
				else
				{
					if (this.IsTokenItem())
					{
						result = true;
					}
					else
					{
						result = this.Effects.Any((EffectBase x) => x.EffectId == EffectsEnum.Effect_NonExchangeable_981 || x.EffectId == EffectsEnum.Effect_NonExchangeable_982);
					}
				}
			}
			return result;
		}
		public bool IsTokenItem()
		{
			return Inventory.ActiveTokens && this.Template.Id == Inventory.TokenTemplateId;
		}
		public virtual bool IsUsable()
		{
			return this.Template.Usable;
		}
		public virtual bool IsEquiped()
		{
			return this.Position != CharacterInventoryPositionEnum.INVENTORY_POSITION_NOT_EQUIPED;
		}
		public virtual bool OnAddItem()
		{
			return true;
		}
		public virtual bool OnRemoveItem()
		{
			return true;
		}
		public virtual uint UseItem(uint amount = 1u, Cell targetCell = null, Character target = null)
		{
			uint num = 0u;
			foreach (UsableEffectHandler current in 
				from effect in this.Effects
				select Singleton<EffectManager>.Instance.GetUsableEffectHandler(effect, target ?? this.Owner, this))
			{
				current.NumberOfUses = amount;
				current.TargetCell = targetCell;
				if (current.Apply())
				{
					num = System.Math.Max(current.UsedItems, num);
				}
			}
			return num;
		}
		public virtual bool OnEquipItem(bool unequip)
		{
			return true;
		}
		public virtual bool Feed(BasePlayerItem food)
		{
			return false;
		}
		public virtual bool Drop(BasePlayerItem dropOnItem)
		{
			return false;
		}
		public void OnObjectModified()
		{
			base.Record.IsDirty = true;
		}
		protected virtual ObjectItem BuildObjectItem()
		{
			return new ObjectItem((byte)this.Position, (ushort)this.Template.Id,
				from entry in this.Effects
				where !entry.Hidden
                select entry.GetObjectEffect(), (uint)this.Guid, (uint)this.Stack);
		}
		public ObjectItem GetObjectItem()
		{
			return this.m_objectItemValidator;
		}
		public virtual void Invalidate()
		{
			this.m_objectItemValidator.Invalidate();
		}
	}
}
