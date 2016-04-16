using Stump.Core.Extensions;
using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Database.Items;
using Stump.Server.WorldServer.Database.Items.Templates;
using Stump.Server.WorldServer.Game.Actors.RolePlay.TaxCollectors;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Items.Player;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Items.TaxCollector
{
	public class TaxCollectorItem : Item<TaxCollectorItemRecord>
	{
		public TaxCollectorItem(TaxCollectorItemRecord record) : base(record)
		{
		}
		public TaxCollectorItem(TaxCollectorNpc owner, int guid, ItemTemplate template, System.Collections.Generic.List<EffectBase> effects, uint stack)
		{
			base.Record = new TaxCollectorItemRecord
			{
				Id = guid,
				OwnerId = owner.GlobalId,
				Template = template,
				Stack = stack,
				Effects = effects
			};
		}
		public bool MustStackWith(TaxCollectorItem compared)
		{
			return compared.Template.Id == this.Template.Id && compared.Effects.CompareEnumerable(this.Effects);
		}
		public bool MustStackWith(BasePlayerItem compared)
		{
			return compared.Template.Id == this.Template.Id && compared.Effects.CompareEnumerable(this.Effects);
		}
		public ObjectItem GetObjectItem()
		{
			return new ObjectItem(63, (ushort)this.Template.Id, 
				from x in this.Effects
                select x.GetObjectEffect(), (uint)this.Guid, (uint)this.Stack);
		}
        public ObjectItemGenericQuantity GetObjectItemQuantity()
		{
            return new ObjectItemGenericQuantity((ushort)this.Guid, (uint)this.Stack);
		}
	}
}
