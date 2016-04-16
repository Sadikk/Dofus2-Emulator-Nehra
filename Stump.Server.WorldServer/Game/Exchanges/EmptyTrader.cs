namespace Stump.Server.WorldServer.Game.Exchanges
{
	public class EmptyTrader : Trader
	{
		private int m_id;
		public override int Id
		{
			get
			{
				return this.m_id;
			}
		}
		public EmptyTrader(int id, ITrade trade) : base(trade)
		{
			this.m_id = id;
		}
	}
}
