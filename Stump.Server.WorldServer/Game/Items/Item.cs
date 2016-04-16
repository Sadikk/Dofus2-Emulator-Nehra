using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Database.Items;
using Stump.Server.WorldServer.Database.Items.Templates;
using Stump.Server.WorldServer.Game.Effects.Instances;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Items
{
	public abstract class Item<T> : IItem where T : ItemRecord<T>
	{
		IItemRecord IItem.Record
		{
			get
			{
				return this.Record;
			}
		}
		public T Record
		{
			get;
			protected set;
		}
		public virtual int Guid
		{
			get
			{
				T record = this.Record;
				return record.Id;
			}
			protected set
			{
				T record = this.Record;
				record.Id = value;
			}
		}
		public virtual uint Stack
		{
			get
			{
				T record = this.Record;
				return record.Stack;
			}
			set
			{
				T record = this.Record;
				record.Stack = value;
			}
		}
		public virtual ItemTemplate Template
		{
			get
			{
				T record = this.Record;
				return record.Template;
			}
			protected set
			{
				T record = this.Record;
				record.Template = value;
			}
		}
		public virtual System.Collections.Generic.List<EffectBase> Effects
		{
			get
			{
				T record = this.Record;
				return record.Effects;
			}
			protected set
			{
				T record = this.Record;
				record.Effects = value;
			}
		}
		protected Item()
		{
		}
		protected Item(T record)
		{
			this.Record = record;
		}
		public ObjectItemInformationWithQuantity GetObjectItemInformationWithQuantity()
		{
			return new ObjectItemInformationWithQuantity((ushort)this.Template.Id, (
				from entry in this.Effects
				select entry.GetObjectEffect()).ToArray<ObjectEffect>(), (uint)this.Stack);
		}
	}
}
