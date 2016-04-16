using Stump.Core.Reflection;
using Stump.ORM;
using Stump.Server.WorldServer.Database.Items.Templates;
using Stump.Server.WorldServer.Game.Effects;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Items;
using System.Linq;
namespace Stump.Server.WorldServer.Database.Items
{
	public abstract class ItemRecord<T> : AutoAssignedRecord<T>, IItemRecord
	{
		private System.Collections.Generic.List<EffectBase> m_effects;
		private byte[] m_serializedEffects;
		private ItemTemplate m_template;
		private int m_itemId;
		private bool m_isDirty;
		private uint m_stack;
		public int ItemId
		{
			get
			{
				return this.m_itemId;
			}
			set
			{
				this.m_itemId = value;
				this.m_template = null;
				this.IsDirty = true;
			}
		}
		[Ignore]
		public ItemTemplate Template
		{
			get
			{
				ItemTemplate arg_23_0;
				if ((arg_23_0 = this.m_template) == null)
				{
					arg_23_0 = (this.m_template = Singleton<ItemManager>.Instance.TryGetTemplate(this.ItemId));
				}
				return arg_23_0;
			}
			set
			{
				this.m_template = value;
				this.ItemId = value.Id;
				this.IsDirty = true;
			}
		}
		public uint Stack
		{
			get
			{
				return this.m_stack;
			}
			set
			{
				this.m_stack = value;
				this.IsDirty = true;
			}
		}
		public byte[] SerializedEffects
		{
			get
			{
				return this.m_serializedEffects;
			}
			set
			{
				this.m_serializedEffects = value;
				this.m_effects = Singleton<EffectManager>.Instance.DeserializeEffects(this.m_serializedEffects);
				this.IsDirty = true;
			}
		}
		[Ignore]
		public System.Collections.Generic.List<EffectBase> Effects
		{
			get
			{
				return this.m_effects;
			}
			set
			{
				this.m_effects = value;
				this.IsDirty = true;
			}
		}
		[Ignore]
		public bool IsDirty
		{
			get
			{
				bool arg_33_0;
				if (!this.m_isDirty)
				{
					arg_33_0 = this.m_effects.Any((EffectBase x) => x.IsDirty);
				}
				else
				{
					arg_33_0 = true;
				}
				return arg_33_0;
			}
			set
			{
				this.m_isDirty = value;
			}
		}
		public ItemRecord()
		{
			this.m_serializedEffects = new byte[0];
		}
		public override void BeforeSave(bool insert)
		{
			base.BeforeSave(insert);
			this.m_serializedEffects = Singleton<EffectManager>.Instance.SerializeEffects(this.Effects);
			this.IsDirty = false;
			foreach (EffectBase current in this.Effects)
			{
				current.IsDirty = false;
			}
		}
	}
}
