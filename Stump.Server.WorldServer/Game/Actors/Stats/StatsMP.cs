using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Game.Actors.Interfaces;

namespace Stump.Server.WorldServer.Game.Actors.Stats
{
	public class StatsMP : StatsData
	{
		public short Used
		{
			get;
			set;
		}
		public int TotalMax
		{
			get
			{
				return base.Total;
			}
		}
		public override int Total
		{
			get
			{
				return this.TotalMax - (int)this.Used;
			}
		}
		public StatsMP(IStatsOwner owner, int valueBase) : base(owner, PlayerFields.MP, valueBase, null)
		{
		}
		public StatsMP(IStatsOwner owner, int valueBase, int limit) : base(owner, PlayerFields.AP, valueBase, limit)
		{
		}
	}
}
