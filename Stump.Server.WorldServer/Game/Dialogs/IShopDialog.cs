namespace Stump.Server.WorldServer.Game.Dialogs
{
	public interface IShopDialog : IDialog
	{
		bool BuyItem(int id, uint quantity);
		bool SellItem(int id, uint quantity);
	}
}
