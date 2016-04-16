using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Fights.Results;
using Stump.Server.WorldServer.Game.Fights.Teams;
using Stump.Server.WorldServer.Game.Formulas;
using Stump.Server.WorldServer.Game.Items;
using Stump.Server.WorldServer.Game.Maps;
using Stump.Server.WorldServer.Handlers.Context;
using System;
using System.Linq;

namespace Stump.Server.WorldServer.Game.Fights
{
	public class FightPvM : Fight
	{
        // FIELDS
		private bool m_ageBonusDefined;
		
        // PROPERTIES
        public override FightTypeEnum FightType
		{
			get
			{
				return FightTypeEnum.FIGHT_TYPE_PvM;
			}
		}
        public override ushort NumberChallenges
        {
            get
            {
                if (base.Map.IsDungeonSpawn)
                {
                    return 2;
                }
                else
                {
                    return 1;
                }
            }
        }

        // CONSTRUCTORS
        public FightPvM(int id, Map fightMap, FightTeam blueTeam, FightTeam redTeam) : base(id, fightMap, blueTeam, redTeam)
		{
		}
		
        // METHODS
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
		protected override void OnFighterAdded(FightTeam team, FightActor actor)
		{
			base.OnFighterAdded(team, actor);
			if (team is FightMonsterTeam && !this.m_ageBonusDefined)
			{
				MonsterFighter monsterFighter = team.Leader as MonsterFighter;
				if (monsterFighter != null)
				{
					base.AgeBonus = monsterFighter.Monster.Group.AgeBonus;
				}
				this.m_ageBonusDefined = true;
			}
		}

		protected override System.Collections.Generic.IEnumerable<IFightResult> GenerateResults()
		{
			System.Collections.Generic.List<IFightResult> list = new System.Collections.Generic.List<IFightResult>();

			list.AddRange(
				from entry in base.GetFightersAndLeavers()
				where !(entry is SummonedFighter)
				select entry.GetFightResult());

			if (base.Map.TaxCollector != null && base.Map.TaxCollector.CanGatherLoots())
			{
				list.Add(new TaxCollectorFightResult(base.Map.TaxCollector, this));
			}

			FightTeam[] teams = this.m_teams;
			for (int i = 0; i < teams.Length; i++)
			{
				FightTeam team = teams[i];
				System.Collections.Generic.IEnumerable<FightActor> enumerable = ((team == base.RedTeam) ? base.BlueTeam : base.RedTeam).GetAllFighters((FightActor entry) => entry.IsDead()).ToList<FightActor>();

				IOrderedEnumerable<IFightResult> orderedEnumerable = 
					from x in list
					where x.CanLoot(team)
					select x into entry
					orderby (entry is TaxCollectorFightResult) ? -1 : entry.Prospecting descending
					select entry;

				int teamPP = team.GetAllFighters().Sum((FightActor entry) => entry.Stats[PlayerFields.Prospecting].Total);
                int challengeSum = base.m_challenges.Sum((entry => entry.GetChallengeBonus()));
                long baseKamas = enumerable.Sum((FightActor entry) => (long)((ulong)entry.GetDroppedKamas()));

				using (System.Collections.Generic.IEnumerator<IFightResult> enumerator = orderedEnumerable.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						IFightResult looter = enumerator.Current;

						looter.Loot.Kamas = FightFormulas.AdjustDroppedKamas(looter, teamPP, baseKamas);
						System.Collections.Generic.IEnumerable<FightActor> arg_1F0_0 = enumerable;
                        Func<FightActor, System.Collections.Generic.IEnumerable<DroppedItem>> selector = (FightActor dropper) => dropper.RollLoot(looter, challengeSum);
						foreach (DroppedItem current in arg_1F0_0.SelectMany(selector))
						{
							looter.Loot.AddItem(current);
						}

						if (looter is IExperienceResult)
						{
							(looter as IExperienceResult).AddEarnedExperience(FightFormulas.CalculateWinExp(looter, team.GetAllFighters(), enumerable));
						}
					}
				}
			}
			return list;
		}
		protected override void SendGameFightJoinMessage(CharacterFighter fighter)
		{
			ContextHandler.SendGameFightJoinMessage(fighter.Character.Client, true, !base.IsStarted, false, base.IsStarted, this.GetPlacementTimeLeft(), this.FightType);
		}
		protected override void SendGameFightJoinMessage(FightSpectator spectator)
		{
			ContextHandler.SendGameFightJoinMessage(spectator.Character.Client, false, !base.IsStarted, true, base.IsStarted, this.GetPlacementTimeLeft(), this.FightType);
		}
		protected override bool CanCancelFight()
		{
			return false;
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
	}
}
