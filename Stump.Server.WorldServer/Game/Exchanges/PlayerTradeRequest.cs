using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Dialogs;
using Stump.Server.WorldServer.Handlers.Inventory;

namespace Stump.Server.WorldServer.Game.Exchanges
{
	public class PlayerTradeRequest : RequestBox
	{
		public PlayerTradeRequest(Character source, Character target) : base(source, target)
		{
			base.Source = source;
			base.Target = target;
		}
		protected override void OnOpen()
		{
			InventoryHandler.SendExchangeRequestedTradeMessage(base.Source.Client, ExchangeTypeEnum.PLAYER_TRADE, base.Source, base.Target);
			InventoryHandler.SendExchangeRequestedTradeMessage(base.Target.Client, ExchangeTypeEnum.PLAYER_TRADE, base.Source, base.Target);
		}
		protected override void OnAccept()
		{
			PlayerTrade playerTrade = new PlayerTrade(base.Source, base.Target);
			playerTrade.Open();
		}
		protected override void OnDeny()
		{
			InventoryHandler.SendExchangeLeaveMessage(base.Source.Client, DialogTypeEnum.DIALOG_EXCHANGE, false);
			InventoryHandler.SendExchangeLeaveMessage(base.Target.Client, DialogTypeEnum.DIALOG_EXCHANGE, false);
		}
		protected override void OnCancel()
		{
			base.Deny();
		}
	}
}
