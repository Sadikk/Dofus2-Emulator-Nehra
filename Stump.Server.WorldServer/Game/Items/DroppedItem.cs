namespace Stump.Server.WorldServer.Game.Items
{
	public class DroppedItem
	{
		public int ItemId
		{
			get;
			set;
		}
		public uint Amount
		{
			get;
			set;
		}

		public DroppedItem(int itemId, uint amount)
		{
			this.ItemId = itemId;
			this.Amount = amount;
		}
	}
}
