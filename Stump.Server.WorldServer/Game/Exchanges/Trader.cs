using Stump.Server.WorldServer.Game.Dialogs;
using Stump.Server.WorldServer.Game.Exchanges.Items;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Exchanges
{
	public abstract class Trader : IDialoger
	{
		public delegate void ItemMovedHandler(Trader trader, TradeItem item, bool modified, int difference);
		public delegate void KamasChangedHandler(Trader trader, uint kamasAmount);
		public delegate void ReadyStatusChangedHandler(Trader trader, bool isReady);
		private readonly System.Collections.Generic.List<TradeItem> m_items = new System.Collections.Generic.List<TradeItem>();
		public event Trader.ItemMovedHandler ItemMoved;
		public event Trader.KamasChangedHandler KamasChanged;
		public event Trader.ReadyStatusChangedHandler ReadyStatusChanged;
		public ITrade Trade
		{
			get;
			private set;
		}
		public IDialog Dialog
		{
			get
			{
				return this.Trade;
			}
		}
		public System.Collections.ObjectModel.ReadOnlyCollection<TradeItem> Items
		{
			get
			{
				return this.m_items.AsReadOnly();
			}
		}
		public string ItemsString
		{
			get
			{
				return string.Join("|", 
					from item in this.m_items
					select item.Template.Id + "_" + item.Stack);
			}
		}
		public abstract int Id
		{
			get;
		}
		public uint Kamas
		{
			get;
			private set;
		}
		public bool ReadyToApply
		{
			get;
			private set;
		}
		protected virtual void OnItemMoved(TradeItem item, bool modified, int difference)
		{
			Trader.ItemMovedHandler itemMoved = this.ItemMoved;
			if (itemMoved != null)
			{
				itemMoved(this, item, modified, difference);
			}
		}
		protected virtual void OnKamasChanged(uint kamasAmount)
		{
			Trader.KamasChangedHandler kamasChanged = this.KamasChanged;
			if (kamasChanged != null)
			{
				kamasChanged(this, kamasAmount);
			}
		}
		protected virtual void OnReadyStatusChanged(bool isready)
		{
			Trader.ReadyStatusChangedHandler readyStatusChanged = this.ReadyStatusChanged;
			if (readyStatusChanged != null)
			{
				readyStatusChanged(this, isready);
			}
		}
		protected Trader(ITrade trade)
		{
			this.Trade = trade;
		}
		protected void AddItem(TradeItem item)
		{
			this.m_items.Add(item);
		}
		protected bool RemoveItem(TradeItem item)
		{
			return this.m_items.Remove(item);
		}
		public void ToggleReady()
		{
			this.ToggleReady(!this.ReadyToApply);
		}
		public virtual bool MoveItem(int id, int quantity)
		{
			return false;
		}
		public virtual void ToggleReady(bool status)
		{
			if (status != this.ReadyToApply)
			{
				this.ReadyToApply = status;
				this.OnReadyStatusChanged(this.ReadyToApply);
			}
		}
		public virtual bool SetKamas(uint amount)
		{
			this.ToggleReady(false);
			this.Kamas = amount;
			this.OnKamasChanged(this.Kamas);
			return true;
		}
	}
}
