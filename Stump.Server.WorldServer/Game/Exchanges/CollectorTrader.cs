using Stump.DofusProtocol.Messages;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Actors.RolePlay.TaxCollectors;
using Stump.Server.WorldServer.Game.Items.TaxCollector;

namespace Stump.Server.WorldServer.Game.Exchanges
{
	public class CollectorTrader : Trader
	{
		public override int Id
		{
			get
			{
				return this.Character.Id;
			}
		}
		public TaxCollectorNpc TaxCollector
		{
			get;
			private set;
		}
		public Character Character
		{
			get;
			private set;
		}
		public CollectorTrader(TaxCollectorNpc taxCollector, Character character, TaxCollectorTrade taxCollectorTrade) : base(taxCollectorTrade)
		{
			this.TaxCollector = taxCollector;
			this.Character = character;
		}
		public override bool MoveItem(int id, int quantity)
		{
			bool result;
			if (quantity >= 0)
			{
				this.Character.SendSystemMessage(7, false, new object[0]);
				result = false;
			}
			else
			{
				quantity = -quantity;
				TaxCollectorItem taxCollectorItem = this.TaxCollector.Bag.TryGetItem(id);
				if (taxCollectorItem == null)
				{
					result = false;
				}
				else
				{
					if (this.TaxCollector.Bag.MoveToInventory(taxCollectorItem, this.Character, (uint)quantity))
					{
                        this.Character.Client.Send(new StorageObjectRemoveMessage((uint)id));
					}
					result = true;
				}
			}
			return result;
		}
		public override bool SetKamas(uint amount)
		{
			if (this.TaxCollector.GatheredKamas <= 0)
			{
				amount = 0u;
			}
			if ((long)this.TaxCollector.GatheredKamas < (long)((ulong)amount))
			{
				amount = (uint)this.TaxCollector.GatheredKamas;
			}
			this.TaxCollector.GatheredKamas -= (int)amount;
			this.Character.Inventory.AddKamas((int)amount);
			this.Character.Client.Send(new StorageKamasUpdateMessage(this.TaxCollector.GatheredKamas));
			return true;
		}
	}
}
