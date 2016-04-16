using Stump.Core.Extensions;
using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Database.Items;
using Stump.Server.WorldServer.Database.Items.Templates;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Effects.Instances;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Items.Player
{
	public class MerchantItem : Item<PlayerMerchantItemRecord>
	{
		public uint Price
		{
			get
			{
				return base.Record.Price;
			}
			set
			{
				base.Record.Price = value;
			}
		}
		public MerchantItem(PlayerMerchantItemRecord record) : base(record)
		{
		}
		public MerchantItem(Character owner, int guid, ItemTemplate template, System.Collections.Generic.List<EffectBase> effects, uint stack, uint price)
		{
			base.Record = new PlayerMerchantItemRecord
			{
				Id = guid,
				OwnerId = owner.Id,
				Template = template,
				Stack = stack,
				Price = price,
				Effects = effects
			};
		}
		public bool MustStackWith(MerchantItem compared)
		{
			return compared.Template.Id == this.Template.Id && compared.Effects.CompareEnumerable(this.Effects);
		}
		public bool MustStackWith(BasePlayerItem compared)
		{
			return compared.Template.Id == this.Template.Id && compared.Effects.CompareEnumerable(this.Effects);
		}
		public ObjectItemToSell GetObjectItemToSell()
		{
			return new ObjectItemToSell((ushort)this.Template.Id,
				from x in this.Effects
                select x.GetObjectEffect(), (uint)this.Guid, (uint)this.Stack, (uint)this.Price);
		}
		public ObjectItemToSellInHumanVendorShop GetObjectItemToSellInHumanVendorShop()
		{
			return new ObjectItemToSellInHumanVendorShop((ushort)this.Template.Id, 
				from x in this.Effects
				select x.GetObjectEffect(), (uint) this.Guid, (uint)this.Stack, (uint)this.Price, 0);
		}
	}
}
