using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Actors.RolePlay.TaxCollectors;
using Stump.Server.WorldServer.Handlers.Inventory;
using Stump.Server.WorldServer.Handlers.TaxCollector;

namespace Stump.Server.WorldServer.Game.Exchanges
{
	public class TaxCollectorTrade : Trade<CollectorTrader, EmptyTrader>
	{
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
		public override ExchangeTypeEnum ExchangeType
		{
			get
			{
				return ExchangeTypeEnum.TAXCOLLECTOR;
			}
		}
		public TaxCollectorTrade(TaxCollectorNpc taxCollector, Character character)
		{
			this.TaxCollector = taxCollector;
			this.Character = character;
			base.FirstTrader = new CollectorTrader(taxCollector, character, this);
			base.SecondTrader = new EmptyTrader(taxCollector.Id, this);
		}
		public override void Open()
		{
			base.Open();
			this.Character.SetDialoger(base.FirstTrader);
			this.TaxCollector.OnDialogOpened(this);
			InventoryHandler.SendStorageInventoryContentMessage(this.Character.Client, this.TaxCollector);
			this.Character.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 139, new object[]
			{
				2
			});
		}
		public override void Close()
		{
			base.Close();
			InventoryHandler.SendExchangeLeaveMessage(this.Character.Client, base.DialogType, false);
			this.Character.CloseDialog(this);
			this.TaxCollector.OnDialogClosed(this);
			this.TaxCollector.Guild.AddXP((long)this.TaxCollector.GatheredExperience);
			TaxCollectorHandler.SendTaxCollectorMovementMessage(this.TaxCollector.Guild.Clients, false, this.TaxCollector, this.Character.Name);
			this.TaxCollector.Delete();
		}
		protected override void Apply()
		{
		}
	}
}
