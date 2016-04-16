using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Database.Items.Templates;
using Stump.Server.WorldServer.Game.Effects.Instances;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Exchanges.Items
{
	public abstract class TradeItem
	{
		public abstract int Guid
		{
			get;
		}
		public abstract ItemTemplate Template
		{
			get;
		}
		public abstract uint Stack
		{
			get;
			set;
		}
		public abstract System.Collections.Generic.List<EffectBase> Effects
		{
			get;
		}
		public abstract CharacterInventoryPositionEnum Position
		{
			get;
		}
		public ObjectItem GetObjectItem()
		{
			return new ObjectItem((byte)this.Position, (ushort)this.Template.Id,
				from entry in this.Effects
				where !entry.Hidden
                select entry.GetObjectEffect(), (uint)this.Guid, (uint)this.Stack);
		}
	}
}
