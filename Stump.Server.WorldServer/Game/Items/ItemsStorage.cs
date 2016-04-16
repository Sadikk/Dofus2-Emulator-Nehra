using System;
namespace Stump.Server.WorldServer.Game.Items
{
	public class ItemsStorage<T> : ItemsCollection<T> where T : IItem
	{
		public event Action<ItemsStorage<T>, int> KamasAmountChanged;
		public virtual int Kamas
		{
			get;
			protected set;
		}
		private void NotifyKamasAmountChanged(int kamas)
		{
			this.OnKamasAmountChanged(kamas);
			Action<ItemsStorage<T>, int> kamasAmountChanged = this.KamasAmountChanged;
			if (kamasAmountChanged != null)
			{
				kamasAmountChanged(this, kamas);
			}
		}
		protected virtual void OnKamasAmountChanged(int amount)
		{
		}
		public void AddKamas(int amount)
		{
			this.SetKamas(this.Kamas + amount);
		}
		public void SubKamas(int amount)
		{
			this.SetKamas(this.Kamas - amount);
		}
		public virtual void SetKamas(int amount)
		{
			this.Kamas = amount;
			this.NotifyKamasAmountChanged(amount);
		}
	}
}
