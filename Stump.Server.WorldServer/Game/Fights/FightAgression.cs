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
	public class FightAgression : Fight
	{
		public override FightTypeEnum FightType
		{
			get
			{
				return FightTypeEnum.FIGHT_TYPE_AGRESSION;
			}
		}
		public FightAgression(int id, Map fightMap, FightTeam blueTeam, FightTeam redTeam) : base(id, fightMap, blueTeam, redTeam)
		{
			this.m_placementTimer = base.Map.Area.CallDelayed(Fight.PlacementPhaseTime, new Action(this.StartFighting));
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
			IFightResult[] array = (
				from entry in base.GetFightersAndLeavers()
				where !(entry is SummonedFighter)
				select entry into fighter
				select fighter.GetFightResult()).ToArray<IFightResult>();
			IFightResult[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				IFightResult fightResult = array2[i];
				FightPlayerResult fightPlayerResult = fightResult as FightPlayerResult;
				if (fightPlayerResult != null)
				{
					fightPlayerResult.SetEarnedHonor(this.CalculateEarnedHonor(fightPlayerResult.Fighter), this.CalculateEarnedDishonor(fightPlayerResult.Fighter));
				}
			}
			return array;
		}
		protected override void SendGameFightJoinMessage(CharacterFighter fighter)
		{
			ContextHandler.SendGameFightJoinMessage(fighter.Character.Client, this.CanCancelFight(), !base.IsStarted, false, base.IsStarted, this.GetPlacementTimeLeft(), this.FightType);
		}
		protected override void SendGameFightJoinMessage(FightSpectator spectator)
		{
			ContextHandler.SendGameFightJoinMessage(spectator.Character.Client, false, false, true, base.IsStarted, this.GetPlacementTimeLeft(), this.FightType);
		}
		public int GetPlacementTimeLeft()
		{
			double num = (double)Fight.PlacementPhaseTime - (System.DateTime.Now - base.CreationTime).TotalMilliseconds;
			if (num < 0.0)
			{
				num = 0.0;
			}
			return (int)num;
		}
		protected override bool CanCancelFight()
		{
			return false;
		}
		public short CalculateEarnedHonor(CharacterFighter character)
		{
			short result;
			if (base.Draw)
			{
				result = 0;
			}
			else
			{
				if (character.OpposedTeam.AlignmentSide == AlignmentSideEnum.ALIGNMENT_NEUTRAL)
				{
					result = 0;
				}
				else
				{
					double num = (double)base.Winners.GetAllFightersWithLeavers().Sum((FightActor entry) => (int)entry.Level);
					double num2 = (double)base.Losers.GetAllFightersWithLeavers().Sum((FightActor entry) => (int)entry.Level);
					double num3 = System.Math.Floor(System.Math.Sqrt((double)character.Level) * 10.0 * (num2 / num));
					if (base.Losers == character.Team)
					{
						num3 = -num3;
					}
					result = (short)num3;
				}
			}
			return result;
		}
		public short CalculateEarnedDishonor(CharacterFighter character)
		{
			short result;
			if (base.Draw)
			{
				result = 0;
			}
			else
			{
				if (character.OpposedTeam.AlignmentSide != AlignmentSideEnum.ALIGNMENT_NEUTRAL)
				{
					result = 0;
				}
				else
				{
					result = 1;
				}
			}
			return result;
		}
	}
}
