using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;

namespace Stump.Server.WorldServer.Game.Fights.Teams
{
	public abstract class FightTeamWithLeader<T> : FightTeam where T : FightActor
	{
		public new T Leader
		{
			get
			{
				return (T)((object)base.Leader);
			}
		}
		public FightTeamWithLeader(sbyte id, Cell[] placementCells) : base(id, placementCells)
		{
		}
		public FightTeamWithLeader(sbyte id, Cell[] placementCells, AlignmentSideEnum alignmentSide) : base(id, placementCells, alignmentSide)
		{
		}
		public override bool ChangeLeader(FightActor leader)
		{
			if (!(leader is T))
			{
				throw new System.Exception(string.Format("Leader of a FightPlayerTeam must be a {0} not {1}", typeof(T), leader.GetType()));
			}
			return base.ChangeLeader(leader);
		}
		protected override void OnFighterAdded(FightActor fighter)
		{
			if (base.Fighters.Count == 1 && !(base.Fighters[0] is T))
			{
				throw new System.Exception(string.Format("Leader of a FightPlayerTeam must be a {0} not {1}", typeof(T), base.Fighters[0].GetType()));
			}
			base.OnFighterAdded(fighter);
		}
	}
}
