using NLog;
using Stump.Core.Attributes;
using Stump.Core.Extensions;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Fights.Results;
using Stump.Server.WorldServer.Game.Fights.Teams;
using Stump.Server.WorldServer.Game.Formulas;
using Stump.Server.WorldServer.Game.Items.TaxCollector;
using Stump.Server.WorldServer.Game.Maps;
using Stump.Server.WorldServer.Handlers.Context;
using Stump.Server.WorldServer.Handlers.TaxCollector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Fights
{
	public class FightPvT : Fight
	{
		private new static readonly Logger logger = LogManager.GetCurrentClassLogger();
		[Variable]
		public static int PvTAttackersPlacementPhaseTime = 30000;
		[Variable]
		public static int PvTDefendersPlacementPhaseTime = 10000;
		private bool m_isAttackersPlacementPhase;
		private readonly System.Collections.Generic.List<Character> m_defendersQueue = new System.Collections.Generic.List<Character>();
		private readonly System.Collections.Generic.Dictionary<Character, Map> m_defendersMaps = new System.Collections.Generic.Dictionary<Character, Map>();
		public TaxCollectorFighter TaxCollector
		{
			get;
			private set;
		}
		public FightTaxCollectorAttackersTeam AttackersTeam
		{
			get
			{
				return (FightTaxCollectorAttackersTeam)base.RedTeam;
			}
		}
		public FightTaxCollectorDefenderTeam DefendersTeam
		{
			get
			{
				return (FightTaxCollectorDefenderTeam)base.BlueTeam;
			}
		}
		public System.Collections.ObjectModel.ReadOnlyCollection<Character> DefendersQueue
		{
			get
			{
				return this.m_defendersQueue.AsReadOnly();
			}
		}
		public bool IsAttackersPlacementPhase
		{
			get
			{
				return this.m_isAttackersPlacementPhase && (base.State == FightState.Placement || base.State == FightState.NotStarted);
			}
			private set
			{
				this.m_isAttackersPlacementPhase = value;
			}
		}
		public bool IsDefendersPlacementPhase
		{
			get
			{
				return !this.m_isAttackersPlacementPhase && (base.State == FightState.Placement || base.State == FightState.NotStarted);
			}
			private set
			{
				this.m_isAttackersPlacementPhase = !value;
			}
		}
		public override FightTypeEnum FightType
		{
			get
			{
				return FightTypeEnum.FIGHT_TYPE_PvT;
			}
		}
		public FightPvT(int id, Map fightMap, FightTaxCollectorDefenderTeam blueTeam, FightTaxCollectorAttackersTeam redTeam) : base(id, fightMap, blueTeam, redTeam)
		{
		}
		public override void StartPlacement()
		{
			base.StartPlacement();
			this.m_isAttackersPlacementPhase = true;
			this.m_placementTimer = base.Map.Area.CallDelayed(FightPvT.PvTAttackersPlacementPhaseTime, new Action(this.StartDefendersPlacement));
			TaxCollectorHandler.SendTaxCollectorAttackedMessage(this.TaxCollector.TaxCollectorNpc.Guild.Clients, this.TaxCollector.TaxCollectorNpc);
		}
        public void StartDefendersPlacement()
        {
            if (this.State != FightState.Placement)
                return;
            this.m_placementTimer.Dispose();
            this.m_placementTimer = null;
            this.m_isAttackersPlacementPhase = false;
            if (this.DefendersQueue.Count == 0)
                this.StartFighting();
            foreach (Character character in this.DefendersQueue)
            {
                Character defender = character;
                this.m_defendersMaps.Add(defender, defender.Map);
                Character defender1 = defender;
                defender.Area.ExecuteInContext((Action)(() =>
                {
                    defender1.Teleport(this.Map, defender1.Cell);
                    defender1.ResetDefender();
                    this.Map.Area.ExecuteInContext((Action)(() =>
                    {
                        this.DefendersTeam.AddFighter((FightActor)defender.CreateFighter((FightTeam)this.DefendersTeam));
                        if (!Enumerable.All<Character>((IEnumerable<Character>)this.DefendersQueue, (Func<Character, bool>)(x => Enumerable.Any<CharacterFighter>(Enumerable.OfType<CharacterFighter>((IEnumerable)this.DefendersTeam.Fighters), (Func<CharacterFighter, bool>)(y => y.Character == x)))))
                            return;
                        this.m_placementTimer = this.Map.Area.CallDelayed(FightPvT.PvTDefendersPlacementPhaseTime, new Action(((Fight)this).StartFighting));
                    }));
                }));
            }
        }

		public override void StartFighting()
		{
			this.m_placementTimer.Dispose();
			base.StartFighting();
		}
		public FighterRefusedReasonEnum AddDefender(Character character)
		{
			FighterRefusedReasonEnum result;
			if (character.TaxCollectorDefendFight != null)
			{
				result = FighterRefusedReasonEnum.IM_OCCUPIED;
			}
			else
			{
				if (!this.IsAttackersPlacementPhase)
				{
					result = FighterRefusedReasonEnum.TOO_LATE;
				}
				else
				{
					if (character.Guild == null || character.Guild != this.TaxCollector.TaxCollectorNpc.Guild)
					{
						result = FighterRefusedReasonEnum.WRONG_GUILD;
					}
					else
					{
						if (this.m_defendersQueue.Count >= 7)
						{
							result = FighterRefusedReasonEnum.TEAM_FULL;
						}
						else
						{
							if (this.m_defendersQueue.Any((Character x) => x.Client.IP == character.Client.IP))
							{
								result = FighterRefusedReasonEnum.MULTIACCOUNT_NOT_ALLOWED;
							}
							else
							{
								if (this.m_defendersQueue.Contains(character))
								{
									result = FighterRefusedReasonEnum.MULTIACCOUNT_NOT_ALLOWED;
								}
								else
								{
									this.m_defendersQueue.Add(character);
									character.SetDefender(this);
									TaxCollectorHandler.SendGuildFightPlayersHelpersJoinMessage(character.Guild.Clients, this.TaxCollector.TaxCollectorNpc, character);
									result = FighterRefusedReasonEnum.FIGHTER_ACCEPTED;
								}
							}
						}
					}
				}
			}
			return result;
		}
		public bool RemoveDefender(Character character)
		{
			bool result;
			if (!this.m_defendersQueue.Remove(character))
			{
				result = false;
			}
			else
			{
				character.ResetDefender();
				TaxCollectorHandler.SendGuildFightPlayersHelpersLeaveMessage(character.Guild.Clients, this.TaxCollector.TaxCollectorNpc, character);
				result = true;
			}
			return result;
		}
		public int GetDefendersLeftSlot()
		{
			return (7 - this.m_defendersQueue.Count > 0) ? (7 - this.m_defendersQueue.Count) : 0;
		}
		public override bool CanChangePosition(FightActor fighter, Cell cell)
		{
			int arg_41_0;
			if (base.CanChangePosition(fighter, cell))
			{
				if (this.IsAttackersPlacementPhase)
				{
					if (fighter.Team == this.AttackersTeam)
					{
						arg_41_0 = 1;
						return arg_41_0 != 0;
					}
				}
				arg_41_0 = ((this.IsDefendersPlacementPhase && fighter.Team == this.DefendersTeam) ? 1 : 0);
			}
			else
			{
				arg_41_0 = 0;
			}
			return arg_41_0 != 0;
		}
		protected override void OnFighterAdded(FightTeam team, FightActor actor)
		{
			if (actor is TaxCollectorFighter)
			{
				if (this.TaxCollector != null)
				{
					FightPvT.logger.Error("There is already a tax collector in this fight !");
				}
				else
				{
					this.TaxCollector = (actor as TaxCollectorFighter);
				}
			}
			if (base.State == FightState.Placement && team == this.AttackersTeam)
			{
				TaxCollectorHandler.SendGuildFightPlayersEnemiesListMessage(this.TaxCollector.TaxCollectorNpc.Guild.Clients, this.TaxCollector.TaxCollectorNpc, 
					from x in this.AttackersTeam.Fighters.OfType<CharacterFighter>()
					select x.Character);
			}
			base.OnFighterAdded(team, actor);
		}
		protected override void OnFighterRemoved(FightTeam team, FightActor actor)
		{
			if (base.State == FightState.Placement && team == this.AttackersTeam && actor is CharacterFighter)
			{
				TaxCollectorHandler.SendGuildFightPlayersEnemyRemoveMessage(this.TaxCollector.TaxCollectorNpc.Guild.Clients, this.TaxCollector.TaxCollectorNpc, (actor as CharacterFighter).Character);
			}
			if (actor is TaxCollectorFighter && actor.IsAlive())
			{
				(actor as TaxCollectorFighter).TaxCollectorNpc.RejoinMap();
			}
			base.OnFighterRemoved(team, actor);
		}
		protected override void OnWinnersDetermined(FightTeam winners, FightTeam losers, bool draw)
		{
			TaxCollectorHandler.SendTaxCollectorAttackedResultMessage(this.TaxCollector.TaxCollectorNpc.Guild.Clients, base.Winners != this.DefendersTeam && !draw, this.TaxCollector.TaxCollectorNpc);
			if (base.Winners == this.DefendersTeam || draw)
			{
				this.TaxCollector.TaxCollectorNpc.RejoinMap();
				using (System.Collections.Generic.List<Character>.Enumerator enumerator = this.m_defendersQueue.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Character current = enumerator.Current;
						if (this.m_defendersMaps.ContainsKey(current))
						{
							current.NextMap = this.m_defendersMaps[current];
						}
					}
					goto IL_C4;
				}
			}
			this.TaxCollector.TaxCollectorNpc.Delete();
			IL_C4:
			base.OnWinnersDetermined(winners, losers, draw);
		}
		protected override System.Collections.Generic.IEnumerable<IFightResult> GenerateResults()
		{
			System.Collections.Generic.IEnumerable<IFightResult> result;
			if (base.Winners == this.DefendersTeam || base.Draw)
			{
				result = Enumerable.Empty<IFightResult>();
			}
			else
			{
				System.Collections.Generic.List<IFightResult> list = new System.Collections.Generic.List<IFightResult>();
				list.AddRange(
					from entry in this.AttackersTeam.GetAllFighters<CharacterFighter>()
					select entry.GetFightResult());
				IOrderedEnumerable<IFightResult> orderedEnumerable = 
					from entry in list
					orderby entry.Prospecting descending
					select entry;
				int num = this.AttackersTeam.GetAllFighters().Sum((FightActor entry) => entry.Stats[PlayerFields.Prospecting].Total);
				int gatheredKamas = this.TaxCollector.TaxCollectorNpc.GatheredKamas;
				foreach (IFightResult current in orderedEnumerable)
				{
					current.Loot.Kamas = FightFormulas.AdjustDroppedKamas(current, num, (long)gatheredKamas);
				}
				int num2 = 0;
				System.Collections.Generic.List<int> list2 = this.TaxCollector.TaxCollectorNpc.Bag.SelectMany((TaxCollectorItem x) => Enumerable.Repeat<int>(x.Template.Id, (int)x.Stack)).Shuffle<int>().ToList<int>();
				foreach (IFightResult current in orderedEnumerable)
				{
					int num3 = (int)System.Math.Ceiling((double)list2.Count * ((double)current.Prospecting / (double)num));
					while (num2 < num3 && num2 < list2.Count)
					{
						current.Loot.AddItem(list2[num2]);
						num2++;
					}
				}
				result = list;
			}
			return result;
		}
		protected override void SendGameFightJoinMessage(CharacterFighter fighter)
		{
			int timeMaxBeforeFightStart = (int)this.GetPlacementTimeLeft(fighter).TotalMilliseconds;
			ContextHandler.SendGameFightJoinMessage(fighter.Character.Client, this.CanCancelFight(), (fighter.Team == this.AttackersTeam && this.IsAttackersPlacementPhase) || (fighter.Team == this.DefendersTeam && this.IsDefendersPlacementPhase), false, base.IsStarted, timeMaxBeforeFightStart, this.FightType);
		}
		protected override void SendGameFightJoinMessage(FightSpectator spectator)
		{
			ContextHandler.SendGameFightJoinMessage(spectator.Character.Client, false, false, true, base.IsStarted, 0, this.FightType);
		}
		protected override bool CanCancelFight()
		{
			return false;
		}
		public System.TimeSpan GetAttackersPlacementTimeLeft()
		{
			System.TimeSpan result;
			if (this.IsAttackersPlacementPhase)
			{
				result = this.m_placementTimer.NextTick - System.DateTime.Now;
			}
			else
			{
				result = System.TimeSpan.Zero;
			}
			return result;
		}
		public System.TimeSpan GetDefendersWaitTimeForPlacement()
		{
			return System.TimeSpan.FromMilliseconds((double)FightPvT.PvTAttackersPlacementPhaseTime);
		}
		public System.TimeSpan GetPlacementTimeLeft(FightActor fighter)
		{
			System.TimeSpan result;
			if (base.State == FightState.NotStarted && fighter.Team == this.AttackersTeam)
			{
				result = System.TimeSpan.FromMilliseconds((double)FightPvT.PvTAttackersPlacementPhaseTime);
			}
			else
			{
				if (fighter.Team == this.DefendersTeam && this.m_placementTimer == null)
				{
					result = System.TimeSpan.FromMilliseconds((double)FightPvT.PvTDefendersPlacementPhaseTime);
				}
				else
				{
					if ((fighter.Team == this.AttackersTeam && this.IsAttackersPlacementPhase) || (fighter.Team == this.DefendersTeam && this.IsDefendersPlacementPhase))
					{
						result = this.m_placementTimer.NextTick - System.DateTime.Now;
					}
					else
					{
						result = System.TimeSpan.Zero;
					}
				}
			}
			return result;
		}
	}
}
