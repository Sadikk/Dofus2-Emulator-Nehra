using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Fights.Results;
using Stump.Server.WorldServer.Game.Fights.Teams;
using Stump.Server.WorldServer.Game.Maps;
using Stump.Server.WorldServer.Handlers.Context;
using System;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Fights
{
	public class FightDuel : Fight
	{
		public override FightTypeEnum FightType
		{
			get
			{
				return FightTypeEnum.FIGHT_TYPE_CHALLENGE;
			}
		}
		public FightDuel(int id, Map fightMap, FightTeam blueTeam, FightTeam redTeam) : base(id, fightMap, blueTeam, redTeam)
		{
		}
		public override void StartPlacement()
		{
			base.StartPlacement();
			this.m_placementTimer = base.Map.Area.CallDelayed(Fight.PlacementPhaseTime, new Action(this.StartFighting));
		}
		public override void StartFighting()
		{
			this.m_placementTimer.Dispose();
			base.StartFighting();
		}
		protected override System.Collections.Generic.IEnumerable<IFightResult> GenerateResults()
		{
			return 
				from entry in base.GetFightersAndLeavers()
				where !(entry is SummonedFighter)
				select entry into fighter
				select fighter.GetFightResult();
		}
		protected override void SendGameFightJoinMessage(CharacterFighter fighter)
		{
			ContextHandler.SendGameFightJoinMessage(fighter.Character.Client, this.CanCancelFight(), !base.IsStarted, false, base.IsStarted, (int)this.GetPlacementTimeLeft().TotalMilliseconds, this.FightType);
		}
		protected override void SendGameFightJoinMessage(FightSpectator spectator)
		{
			ContextHandler.SendGameFightJoinMessage(spectator.Character.Client, false, false, true, base.IsStarted, (int)this.GetPlacementTimeLeft().TotalMilliseconds, this.FightType);
		}
		public System.TimeSpan GetPlacementTimeLeft()
		{
			System.TimeSpan timeSpan = System.TimeSpan.FromMilliseconds((double)Fight.PlacementPhaseTime) - (System.DateTime.Now - base.CreationTime);
			if (timeSpan < System.TimeSpan.Zero)
			{
				timeSpan = System.TimeSpan.Zero;
			}
			return timeSpan;
		}
		protected override bool CanCancelFight()
		{
			return base.State == FightState.Placement;
		}
	}
}
