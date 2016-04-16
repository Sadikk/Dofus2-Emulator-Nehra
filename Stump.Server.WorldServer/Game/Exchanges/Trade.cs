using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Game.Dialogs;
using Stump.Server.WorldServer.Game.Exchanges.Items;

namespace Stump.Server.WorldServer.Game.Exchanges
{
	public abstract class Trade<TFirst, TSecond> : IDialog, ITrade where TFirst : Trader where TSecond : Trader
	{
		public TFirst FirstTrader
		{
			get;
			protected set;
		}
		public TSecond SecondTrader
		{
			get;
			protected set;
		}
		public DialogTypeEnum DialogType
		{
			get
			{
				return DialogTypeEnum.DIALOG_EXCHANGE;
			}
		}
		Trader ITrade.FirstTrader
		{
			get
			{
				return this.FirstTrader;
			}
		}
		Trader ITrade.SecondTrader
		{
			get
			{
				return this.SecondTrader;
			}
		}
		public abstract ExchangeTypeEnum ExchangeType
		{
			get;
		}
		public virtual void Open()
		{
			this.FirstTrader.ItemMoved += new Trader.ItemMovedHandler(this.OnTraderItemMoved);
			this.FirstTrader.KamasChanged += new Trader.KamasChangedHandler(this.OnTraderKamasChanged);
			this.FirstTrader.ReadyStatusChanged += new Trader.ReadyStatusChangedHandler(this.OnTraderReadyStatusChanged);
			this.SecondTrader.ItemMoved += new Trader.ItemMovedHandler(this.OnTraderItemMoved);
			this.SecondTrader.KamasChanged += new Trader.KamasChangedHandler(this.OnTraderKamasChanged);
			this.SecondTrader.ReadyStatusChanged += new Trader.ReadyStatusChangedHandler(this.OnTraderReadyStatusChanged);
		}
		public virtual void Close()
		{
			TFirst firstTrader = this.FirstTrader;
			bool arg_30_0;
			if (firstTrader.ReadyToApply)
			{
				TSecond secondTrader = this.SecondTrader;
				arg_30_0 = !secondTrader.ReadyToApply;
			}
			else
			{
				arg_30_0 = true;
			}
			if (!arg_30_0)
			{
				this.Apply();
			}
			this.FirstTrader.ItemMoved -= new Trader.ItemMovedHandler(this.OnTraderItemMoved);
			this.FirstTrader.KamasChanged -= new Trader.KamasChangedHandler(this.OnTraderKamasChanged);
			this.FirstTrader.ReadyStatusChanged -= new Trader.ReadyStatusChangedHandler(this.OnTraderReadyStatusChanged);
			this.SecondTrader.ItemMoved -= new Trader.ItemMovedHandler(this.OnTraderItemMoved);
			this.SecondTrader.KamasChanged -= new Trader.KamasChangedHandler(this.OnTraderKamasChanged);
			this.SecondTrader.ReadyStatusChanged -= new Trader.ReadyStatusChangedHandler(this.OnTraderReadyStatusChanged);
		}
		protected abstract void Apply();
		protected virtual void OnTraderItemMoved(Trader trader, TradeItem item, bool modified, int difference)
		{
			TFirst firstTrader = this.FirstTrader;
			firstTrader.ToggleReady(false);
			TSecond secondTrader = this.SecondTrader;
			secondTrader.ToggleReady(false);
		}
		protected virtual void OnTraderKamasChanged(Trader trader, uint amount)
		{
			TFirst firstTrader = this.FirstTrader;
			firstTrader.ToggleReady(false);
			TSecond secondTrader = this.SecondTrader;
			secondTrader.ToggleReady(false);
		}
		protected virtual void OnTraderReadyStatusChanged(Trader trader, bool status)
		{
			TFirst firstTrader = this.FirstTrader;
			bool arg_30_0;
			if (firstTrader.ReadyToApply)
			{
				TSecond secondTrader = this.SecondTrader;
				arg_30_0 = !secondTrader.ReadyToApply;
			}
			else
			{
				arg_30_0 = true;
			}
			if (!arg_30_0)
			{
				this.Close();
			}
		}
	}
}
