using NLog;
using Stump.Core.Attributes;
using Stump.Core.Extensions;
using Stump.Core.Pool;
using Stump.Core.Reflection;
using Stump.Core.Timers;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Enums.HomeMade;
using Stump.DofusProtocol.Types;
using Stump.Server.BaseServer.Network;
using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Database.I18n;
using Stump.Server.WorldServer.Database.Items.Templates;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Challenges;
using Stump.Server.WorldServer.Game.Effects.Spells.Summon;
using Stump.Server.WorldServer.Game.Fights.Buffs;
using Stump.Server.WorldServer.Game.Fights.Results;
using Stump.Server.WorldServer.Game.Fights.Teams;
using Stump.Server.WorldServer.Game.Fights.Triggers;
using Stump.Server.WorldServer.Game.Maps;
using Stump.Server.WorldServer.Game.Maps.Cells;
using Stump.Server.WorldServer.Game.Maps.Pathfinding;
using Stump.Server.WorldServer.Game.Spells;
using Stump.Server.WorldServer.Handlers.Actions;
using Stump.Server.WorldServer.Handlers.Basic;
using Stump.Server.WorldServer.Handlers.Characters;
using Stump.Server.WorldServer.Handlers.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Stump.Server.WorldServer.Game.Fights
{
    public abstract class Fight : WorldObjectsContext, ICharacterContainer
    {
        public delegate void FightWinnersDelegate(Fight fight, FightTeam winners, FightTeam losers, bool draw);

        protected readonly Logger logger = LogManager.GetCurrentClassLogger();

        [Variable]
        public static int PlacementPhaseTime = 30000;

        [Variable]
        public static int TurnTime = 25000;

        [Variable]
        public static int TurnEndTimeOut = 5000;

        [Variable]
        public static int EndFightTimeOut = 10000;

        protected readonly ReversedUniqueIdProvider m_contextualIdProvider = new ReversedUniqueIdProvider(0);
        protected readonly UniqueIdProvider m_triggerIdProvider = new UniqueIdProvider();
        protected readonly List<Buff> m_buffs = new List<Buff>();
        protected readonly List<ChallengeChecker> m_challenges = new List<ChallengeChecker>();
        protected TimedTimerEntry m_placementTimer;
        protected TimedTimerEntry m_turnTimer;
        private bool m_isInitialized;
        private bool m_disposed;
        protected FightTeam[] m_teams;
        private SequenceTypeEnum m_lastSequenceAction;
        private int m_sequenceLevel;
        private readonly Stack<SequenceTypeEnum> m_sequences = new Stack<SequenceTypeEnum>();
        private readonly List<MarkTrigger> m_triggers = new List<MarkTrigger>();
        private readonly WorldClientCollection m_clients = new WorldClientCollection();
        private readonly WorldClientCollection m_spectatorClients = new WorldClientCollection();

        public event FightWinnersDelegate WinnersDetermined;

        public event Action<Fight, FightActor> TurnStarted;
        public event Action<Fight> FightStarted;
        public event Action<Fight> FightEnded;

        public int Id
        {
            get;
            private set;
        }
        public Map Map
        {
            get;
            private set;
        }
        public override Cell[] Cells
        {
            get
            {
                return this.Map.Cells;
            }
        }
        protected override IReadOnlyCollection<WorldObject> Objects
        {
            get
            {
                return this.Fighters.AsReadOnly();
            }
        }
        public abstract FightTypeEnum FightType
        {
            get;
        }
        public FightState State
        {
            get;
            private set;
        }
        public bool IsStarted
        {
            get;
            private set;
        }
        public DateTime CreationTime
        {
            get;
            private set;
        }
        public DateTime StartTime
        {
            get;
            private set;
        }
        public short AgeBonus
        {
            get;
            protected set;
        }
        public FightTeam RedTeam
        {
            get;
            private set;
        }
        public FightTeam BlueTeam
        {
            get;
            private set;
        }
        public FightTeam Winners
        {
            get;
            private set;
        }
        public FightTeam Losers
        {
            get;
            private set;
        }
        public bool Draw
        {
            get;
            private set;
        }
        public TimeLine TimeLine
        {
            get;
            private set;
        }
        public FightActor FighterPlaying
        {
            get
            {
                return this.TimeLine.Current;
            }
        }
        public DateTime TurnStartTime
        {
            get;
            protected set;
        }
        public ReadyChecker ReadyChecker
        {
            get;
            protected set;
        }
        public virtual ushort NumberChallenges
        {
            get
            {
                return 0;
            }
        }
        internal List<FightActor> Fighters
        {
            get
            {
                return this.TimeLine.Fighters;
            }
        }
        internal List<FightActor> Leavers
        {
            get;
            private set;
        }
        internal List<FightSpectator> Spectators
        {
            get;
            private set;
        }
        public bool SpectatorClosed
        {
            get;
            private set;
        }
        public bool BladesVisible
        {
            get;
            private set;
        }
        public Results.FightLoot TaxCollectorLoot
        {
            get;
            private set;
        }
        public SequenceTypeEnum Sequence
        {
            get;
            private set;
        }
        public bool IsSequencing
        {
            get;
            private set;
        }
        public bool WaitAcknowledgment
        {
            get;
            private set;
        }
        public WorldClientCollection Clients
        {
            get
            {
                return this.m_clients;
            }
        }
        public WorldClientCollection SpectatorClients
        {
            get
            {
                return this.m_spectatorClients;
            }
        }

        protected Fight(int id, Map fightMap, FightTeam blueTeam, FightTeam redTeam)
        {
            this.Id = id;
            this.Map = fightMap;
            this.BlueTeam = blueTeam;
            this.BlueTeam.Fight = this;
            this.RedTeam = redTeam;
            this.RedTeam.Fight = this;
            this.m_teams = new FightTeam[]
			{
				this.RedTeam,
				this.BlueTeam
			};
            this.TimeLine = new TimeLine(this);
            this.Leavers = new List<FightActor>();
            this.Spectators = new List<FightSpectator>();
            this.BlueTeam.FighterAdded += new Action<FightTeam, FightActor>(this.OnFighterAdded);
            this.BlueTeam.FighterRemoved += new Action<FightTeam, FightActor>(this.OnFighterRemoved);
            this.RedTeam.FighterAdded += new Action<FightTeam, FightActor>(this.OnFighterAdded);
            this.RedTeam.FighterRemoved += new Action<FightTeam, FightActor>(this.OnFighterRemoved);
            this.CreationTime = DateTime.Now;
            this.TaxCollectorLoot = new Results.FightLoot();
        }

        protected void SetFightState(FightState state)
        {
            this.State = state;
            this.UnBindFightersEvents();
            this.BindFightersEvents();
            this.OnStateChanged();
        }

        protected virtual void OnStateChanged()
        {
            if (this.State != FightState.Placement && this.BladesVisible)
            {
                this.HideBlades();
            }
        }

        public void Initialize()
        {
            if (!this.m_isInitialized)
            {
                this.ProcessInitialization();
                this.m_isInitialized = true;
            }
        }

        protected virtual void ProcessInitialization()
        {
        }

        public virtual void StartFighting()
        {
            if (this.State == FightState.Placement || this.State == FightState.NotStarted)
            {
                this.SetFightState(FightState.Fighting);
                this.StartTime = DateTime.Now;
                this.IsStarted = true;
                this.HideBlades();
                this.TimeLine.OrderLine();
                ContextHandler.SendGameEntitiesDispositionMessage(this.Clients, this.GetAllFighters());
                ContextHandler.SendGameFightStartMessage(this.Clients);
                ContextHandler.SendGameFightTurnListMessage(this.Clients, this);
                if (this.NumberChallenges > 0)
                {
                    Singleton<ChallengeManager>.Instance.GenerateChallenges(this);
                    this.m_challenges.ForEach(entry =>
                    {
                        if (!entry.Hidden)
                        {
                            entry.Initialize();
                        }
                    });
                }
                this.ForEach(delegate(Character entry)
                {
                    ContextHandler.SendGameFightSynchronizeMessage(entry.Client, this);
                }, true);
                this.OnFightStarted();
                this.StartTurn();
            }
        }

        public bool CheckFightEnd()
        {
            bool result;
            if (this.RedTeam.AreAllDead() || this.BlueTeam.AreAllDead() || this.Clients.Count <= 0)
            {
                this.EndFight();
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        public void CancelFight()
        {
            if (this.CanCancelFight())
            {
                if (this.State != FightState.Placement)
                {
                    this.EndFight();
                }
                else
                {
                    this.SetFightState(FightState.Ended);
                    ContextHandler.SendGameFightEndMessage(this.Clients, this);
                    foreach (Character current in this.GetCharactersAndSpectators())
                    {
                        current.RejoinMap();
                    }
                    this.Dispose();
                }
            }
        }

        public void EndFight()
        {
            if (this.State == FightState.Placement)
            {
                this.CancelFight();
            }
            if (this.State != FightState.Ended)
            {
                this.SetFightState(FightState.Ended);
                if (this.FighterPlaying != null)
                {
                    this.FighterPlaying.StopTurn();
                }
                if (this.m_turnTimer != null)
                {
                    this.m_turnTimer.Dispose();
                }
                this.EndAllSequences();
                if (this.ReadyChecker != null)
                {
                    this.ReadyChecker.Cancel();
                }
                this.ReadyChecker = ReadyChecker.RequestCheck(this, new Action(this.OnFightEnded), delegate(CharacterFighter[] actors)
                {
                    this.OnFightEnded();
                });
            }
        }

        protected virtual void OnFightStarted()
        {
            Action<Fight> fightStarted = this.FightStarted;
            if (fightStarted != null)
            {
                fightStarted(this);
            }

            foreach (FightActor current in this.Fighters)
            {
                current.FightStartPosition = current.Position.Clone();
            }

            this.TimeLine.SelectNextFighter();
        }

        protected virtual void OnFightEnded()
        {
            Action<Fight> fightEnded = this.FightEnded;
            if (fightEnded != null)
            {
                fightEnded(this);
            }

            this.ReadyChecker = null;
            this.DeterminsWinners();
            List<IFightResult> list = this.GenerateResults().ToList<IFightResult>();
            this.ApplyResults(list);
            ContextHandler.SendGameFightEndMessage(this.Clients, this,
                from entry in list
                select entry.GetFightResultListEntry());
            this.ResetFightersProperties();
            foreach (Character current in this.GetCharactersAndSpectators())
            {
                current.RejoinMap();
            }
            this.Dispose();
        }

        protected virtual void OnWinnersDetermined(FightTeam winners, FightTeam losers, bool draw)
        {
            FightWinnersDelegate winnersDetermined = this.WinnersDetermined;
            if (winnersDetermined != null)
            {
                winnersDetermined(this, winners, losers, draw);
            }
        }

        protected virtual void DeterminsWinners()
        {
            if (this.BlueTeam.AreAllDead() && !this.RedTeam.AreAllDead())
            {
                this.Winners = this.RedTeam;
                this.Losers = this.BlueTeam;
                this.Draw = false;
            }
            else
            {
                if (!this.BlueTeam.AreAllDead() && this.RedTeam.AreAllDead())
                {
                    this.Winners = this.BlueTeam;
                    this.Losers = this.RedTeam;
                    this.Draw = false;
                }
                else
                {
                    this.Draw = true;
                }
            }
            this.OnWinnersDetermined(this.Winners, this.Losers, this.Draw);
        }

        protected void ResetFightersProperties()
        {
            foreach (FightActor current in this.Fighters)
            {
                current.ResetFightProperties();
            }
        }

        protected abstract IEnumerable<IFightResult> GenerateResults();

        protected void ApplyResults(IEnumerable<IFightResult> results)
        {
            foreach (IFightResult current in results)
            {
                current.Apply();
            }
        }

        protected void Dispose()
        {
            if (!this.m_disposed)
            {
                this.m_disposed = true;
                foreach (FightActor current in this.Fighters)
                {
                    current.Delete();
                }
                this.OnDisposed();
                this.UnBindFightersEvents();
                this.Map.RemoveFight(this);
                Singleton<FightManager>.Instance.Remove(this);
                GC.SuppressFinalize(this);
            }
        }

        protected virtual void OnDisposed()
        {
            if (this.ReadyChecker != null)
            {
                this.ReadyChecker.Cancel();
            }
            if (this.m_placementTimer != null)
            {
                this.m_placementTimer.Dispose();
            }
            if (this.m_turnTimer != null)
            {
                this.m_turnTimer.Dispose();
            }
        }

        public virtual void StartPlacement()
        {
            if (this.State == FightState.NotStarted)
            {
                this.SetFightState(FightState.Placement);
                this.RandomnizePositions(this.RedTeam);
                this.RandomnizePositions(this.BlueTeam);
                this.ShowBlades();
                this.Map.AddFight(this);
            }
        }

        private void FindBladesPlacement()
        {
            if (this.RedTeam.Leader.MapPosition.Cell.Id != this.BlueTeam.Leader.MapPosition.Cell.Id)
            {
                this.RedTeam.BladePosition = this.RedTeam.Leader.MapPosition.Clone();
                this.BlueTeam.BladePosition = this.BlueTeam.Leader.MapPosition.Clone();
            }
            else
            {
                Cell randomAdjacentFreeCell = this.Map.GetRandomAdjacentFreeCell(this.RedTeam.Leader.MapPosition.Point, false);
                if (randomAdjacentFreeCell == null)
                {
                    this.RedTeam.BladePosition = this.RedTeam.Leader.MapPosition.Clone();
                }
                else
                {
                    ObjectPosition objectPosition = this.RedTeam.Leader.MapPosition.Clone();
                    objectPosition.Cell = randomAdjacentFreeCell;
                    this.RedTeam.BladePosition = objectPosition;
                }
                this.BlueTeam.BladePosition = this.BlueTeam.Leader.MapPosition.Clone();
            }
        }

        public void ShowBlades()
        {
            if (!this.BladesVisible && this.State == FightState.Placement)
            {
                if (this.RedTeam.BladePosition == null || this.BlueTeam.BladePosition == null)
                {
                    this.FindBladesPlacement();
                }
                ContextHandler.SendGameRolePlayShowChallengeMessage(this.Map.Clients, this);
                this.RedTeam.TeamOptionsChanged += new Action<FightTeam, FightOptionsEnum>(this.OnTeamOptionsChanged);
                this.BlueTeam.TeamOptionsChanged += new Action<FightTeam, FightOptionsEnum>(this.OnTeamOptionsChanged);
                this.BladesVisible = true;
            }
        }

        public void HideBlades()
        {
            if (this.BladesVisible)
            {
                ContextHandler.SendGameRolePlayRemoveChallengeMessage(this.Map.Clients, this);
                this.RedTeam.TeamOptionsChanged -= new Action<FightTeam, FightOptionsEnum>(this.OnTeamOptionsChanged);
                this.BlueTeam.TeamOptionsChanged -= new Action<FightTeam, FightOptionsEnum>(this.OnTeamOptionsChanged);
                this.BladesVisible = false;
            }
        }

        public void UpdateBlades(FightTeam team)
        {
            if (this.BladesVisible)
            {
                ContextHandler.SendGameFightUpdateTeamMessage(this.Map.Clients, this, team);
            }
        }

        private void OnTeamOptionsChanged(FightTeam team, FightOptionsEnum option)
        {
            ContextHandler.SendGameFightOptionStateUpdateMessage(this.Clients, team, option, team.GetOptionState(option));
            ContextHandler.SendGameFightOptionStateUpdateMessage(this.Map.Clients, team, option, team.GetOptionState(option));
        }

        public bool FindRandomFreeCell(FightActor fighter, out Cell cell, bool placement = true)
        {
            Cell[] array = (
                from entry in fighter.Team.PlacementCells
                where this.GetOneFighter(entry) == null || this.GetOneFighter(entry) == fighter
                select entry).ToArray<Cell>();
            Random random = new Random();
            bool result;
            if (array.Length == 0 && placement)
            {
                cell = null;
                result = false;
            }
            else
            {
                if (array.Length == 0 && !placement)
                {
                    List<int> cells = Enumerable.Range(0, 560).ToList<int>();
                    foreach (FightActor current in this.GetAllFighters((FightActor actor) => cells.Contains((int)actor.Cell.Id)))
                    {
                        cells.Remove((int)current.Cell.Id);
                    }
                    cell = this.Map.Cells[cells[random.Next(cells.Count)]];
                    result = true;
                }
                else
                {
                    cell = array[random.Next(array.Length)];
                    result = true;
                }
            }
            return result;
        }

        public bool RandomnizePosition(FightActor fighter)
        {
            if (this.State != FightState.Placement)
            {
                throw new Exception("State != Placement, cannot random placement position");
            }
            Cell cell;
            bool result;
            if (!this.FindRandomFreeCell(fighter, out cell, true))
            {
                fighter.LeaveFight();
                result = false;
            }
            else
            {
                fighter.ChangePrePlacement(cell);
                result = true;
            }
            return result;
        }

        public void RandomnizePositions(FightTeam team)
        {
            if (this.State != FightState.Placement)
            {
                throw new Exception("State != Placement, cannot random placement position");
            }
            IEnumerable<Cell> enumerable = team.PlacementCells.Shuffle<Cell>();
            IEnumerator<Cell> enumerator = enumerable.GetEnumerator();
            foreach (FightActor current in team.GetAllFighters())
            {
                enumerator.MoveNext();
                current.ChangePrePlacement(enumerator.Current);
            }
            enumerator.Dispose();
        }

        public DirectionsEnum FindPlacementDirection(FightActor fighter)
        {
            if (this.State != FightState.Placement)
            {
                throw new Exception("State != Placement, cannot give placement direction");
            }
            FightTeam fightTeam = (fighter.Team == this.RedTeam) ? this.BlueTeam : this.RedTeam;
            Tuple<Cell, uint> tuple = null;
            foreach (FightActor current in fightTeam.GetAllFighters())
            {
                MapPoint point = current.Position.Point;
                if (tuple == null)
                {
                    tuple = Tuple.Create<Cell, uint>(current.Cell, fighter.Position.Point.DistanceToCell(point));
                }
                else
                {
                    if (fighter.Position.Point.DistanceToCell(point) < tuple.Item2)
                    {
                        tuple = Tuple.Create<Cell, uint>(current.Cell, fighter.Position.Point.DistanceToCell(point));
                    }
                }
            }
            DirectionsEnum result;
            if (tuple == null)
            {
                result = fighter.Position.Direction;
            }
            else
            {
                result = fighter.Position.Point.OrientationTo(new MapPoint(tuple.Item1), false);
            }
            return result;
        }

        public bool KickFighter(FightActor fighter)
        {
            bool result;
            if (!this.Fighters.Contains(fighter))
            {
                result = false;
            }
            else
            {
                if (this.State != FightState.Placement)
                {
                    result = false;
                }
                else
                {
                    fighter.Team.RemoveFighter(fighter);
                    CharacterFighter characterFighter = fighter as CharacterFighter;
                    if (characterFighter != null)
                    {
                        characterFighter.Character.RejoinMap();
                    }
                    this.CheckFightEnd();
                    result = true;
                }
            }
            return result;
        }

        protected virtual void OnSetReady(FightActor fighter, bool isReady)
        {
            if (this.State == FightState.Placement)
            {
                ContextHandler.SendGameFightHumanReadyStateMessage(this.Clients, fighter);
                if (this.RedTeam.AreAllReady() && this.BlueTeam.AreAllReady())
                {
                    this.StartFighting();
                }
            }
        }

        public virtual bool CanChangePosition(FightActor fighter, Cell cell)
        {
            FightActor oneFighter = this.GetOneFighter(cell);
            return this.State == FightState.Placement && fighter.Team.PlacementCells.Contains(cell) && (oneFighter == fighter || oneFighter == null);
        }

        protected virtual void OnChangePreplacementPosition(FightActor fighter, ObjectPosition objectPosition)
        {
            this.UpdateFightersPlacementDirection();
            ContextHandler.SendGameEntitiesDispositionMessage(this.Clients, this.GetAllFighters());
        }

        protected void UpdateFightersPlacementDirection()
        {
            foreach (FightActor current in this.Fighters)
            {
                current.Position.Direction = this.FindPlacementDirection(current);
            }
        }

        protected virtual void OnFighterAdded(FightTeam team, FightActor actor)
        {
            if (actor is SummonedFighter)
            {
                this.OnSummonAdded(actor as SummonedFighter);
            }
            else
            {
                if (this.State == FightState.Ended)
                {
                    throw new Exception("Fight ended");
                }
                this.TimeLine.Fighters.Add(actor);
                this.BindFighterEvents(actor);
                if (this.State != FightState.Placement || this.RandomnizePosition(actor))
                {
                    if (actor is CharacterFighter)
                    {
                        this.OnCharacterAdded(actor as CharacterFighter);
                    }
                    this.ForEach(delegate(Character entry)
                    {
                        ContextHandler.SendGameFightShowFighterMessage(entry.Client, actor);
                    }, true);
                    if (this.BladesVisible)
                    {
                        this.UpdateBlades(team);
                    }
                    ContextHandler.SendGameFightTurnListMessage(this.Clients, this);
                }
            }
        }

        protected virtual void OnSummonAdded(SummonedFighter fighter)
        {
            this.TimeLine.InsertFighter(fighter, this.TimeLine.Fighters.IndexOf(fighter.Summoner) + 1);
            this.BindFighterEvents(fighter);
            ContextHandler.SendGameFightTurnListMessage(this.Clients, this);
        }

        protected virtual void OnCharacterAdded(CharacterFighter fighter)
        {
            Character character = fighter.Character;
            character.RealLook.RemoveAuras();
            character.RefreshActor();
            this.Clients.Add(character.Client);
            this.SendGameFightJoinMessage(fighter);
            if (this.State == FightState.Placement || this.State == FightState.NotStarted)
            {
                ContextHandler.SendGameFightPlacementPossiblePositionsMessage(character.Client, this, fighter.Team.Id);
            }
            ContextHandler.SendIdolFightPreparationUpdateMessage(character.Client);
            foreach (FightActor current in this.GetAllFighters())
            {
                ContextHandler.SendGameFightShowFighterMessage(character.Client, current);
            }
            ContextHandler.SendGameEntitiesDispositionMessage(character.Client, this.GetAllFighters());
            ContextHandler.SendGameFightUpdateTeamMessage(character.Client, this, this.RedTeam);
            ContextHandler.SendGameFightUpdateTeamMessage(character.Client, this, this.BlueTeam);
            ContextHandler.SendGameFightUpdateTeamMessage(this.Clients, this, fighter.Team);
        }

        protected virtual void OnFighterRemoved(FightTeam team, FightActor actor)
        {
            if (actor is SummonedFighter)
            {
                this.OnSummonRemoved(actor as SummonedFighter);
            }
            else
            {
                this.TimeLine.RemoveFighter(actor);
                this.UnBindFighterEvents(actor);
                if (actor is CharacterFighter)
                {
                    this.OnCharacterRemoved(actor as CharacterFighter);
                }
                if (this.State == FightState.Placement)
                {
                    ContextHandler.SendGameFightRemoveTeamMemberMessage(this.Clients, actor);
                }
                else
                {
                    if (this.State == FightState.Fighting)
                    {
                        ContextHandler.SendGameContextRemoveElementMessage(this.Clients, actor);
                    }
                }
                if (this.BladesVisible)
                {
                    this.UpdateBlades(team);
                }
            }
        }

        protected virtual void OnSummonRemoved(SummonedFighter fighter)
        {
            this.TimeLine.RemoveFighter(fighter);
            this.UnBindFighterEvents(fighter);
            ContextHandler.SendGameFightTurnListMessage(this.Clients, this);
        }

        protected virtual void OnCharacterRemoved(CharacterFighter fighter)
        {
            this.Clients.Remove(fighter.Character.Client);
        }

        public void ToggleSpectatorClosed(bool state)
        {
            this.SpectatorClosed = state;
            BasicHandler.SendTextInformationMessage(this.Clients, TextInformationTypeEnum.TEXT_INFORMATION_MESSAGE, Convert.ToInt16(this.SpectatorClosed ? 40 : 39));
            if (state)
            {
                this.RemoveAllSpectators();
            }
            ContextHandler.SendGameFightOptionStateUpdateMessage(this.Clients, this.RedTeam, FightOptionsEnum.FIGHT_OPTION_SET_SECRET, this.SpectatorClosed);
            ContextHandler.SendGameFightOptionStateUpdateMessage(this.Clients, this.BlueTeam, FightOptionsEnum.FIGHT_OPTION_SET_SECRET, this.SpectatorClosed);
        }

        public virtual bool CanSpectatorJoin(Character spectator)
        {
            return !this.SpectatorClosed && this.State == FightState.Fighting;
        }

        public bool AddSpectator(FightSpectator spectator)
        {
            bool result;
            if (!this.CanSpectatorJoin(spectator.Character))
            {
                result = false;
            }
            else
            {
                this.Spectators.Add(spectator);
                spectator.JoinTime = DateTime.Now;
                spectator.Left += new Action<FightSpectator>(this.OnSpectectorLeft);
                spectator.Character.LoggedOut += new Action<Character>(this.OnSpectatorLoggedOut);
                this.Clients.Add(spectator.Client);
                this.SpectatorClients.Add(spectator.Client);
                this.OnSpectatorAdded(spectator);
                result = true;
            }
            return result;
        }

        protected virtual void OnSpectatorAdded(FightSpectator spectator)
        {
            this.SendGameFightJoinMessage(spectator);
            foreach (FightActor current in this.GetAllFighters())
            {
                ContextHandler.SendGameFightShowFighterMessage(spectator.Client, current);
            }
            ContextHandler.SendGameFightTurnListMessage(spectator.Client, this);
            ContextHandler.SendGameFightSpectateMessage(spectator.Client, this);
            ContextHandler.SendGameFightNewRoundMessage(spectator.Client, (int)this.TimeLine.RoundNumber);
            CharacterHandler.SendCharacterStatsListMessage(spectator.Client);
            BasicHandler.SendTextInformationMessage(this.Clients, TextInformationTypeEnum.TEXT_INFORMATION_MESSAGE, 36, new string[]
			{
				spectator.Character.Name
			});
            if (this.TimeLine.Current != null)
            {
                ContextHandler.SendGameFightTurnResumeMessage(spectator.Client, this.FighterPlaying, this.GetTurnTimeLeft());
            }
        }

        protected virtual void OnSpectatorLoggedOut(Character character)
        {
            if (character.IsSpectator())
            {
                this.OnSpectectorLeft(character.Spectator);
            }
        }

        protected virtual void OnSpectectorLeft(FightSpectator spectator)
        {
            this.RemoveSpectator(spectator);
        }

        public void RemoveSpectator(FightSpectator spectator)
        {
            this.Spectators.Remove(spectator);
            this.Clients.Remove(spectator.Character.Client);
            this.SpectatorClients.Remove(spectator.Client);
            spectator.Left -= new Action<FightSpectator>(this.OnSpectectorLeft);
            spectator.Character.LoggedOut -= new Action<Character>(this.OnSpectatorLoggedOut);
            this.OnSpectatorRemoved(spectator);
        }

        protected virtual void OnSpectatorRemoved(FightSpectator spectator)
        {
            spectator.Character.RejoinMap();
        }

        public void RemoveAllSpectators()
        {
            foreach (FightSpectator current in this.Spectators.GetRange(0, this.Spectators.Count))
            {
                this.RemoveSpectator(current);
            }
        }

        public void StartTurn()
        {
            if (this.State == FightState.Fighting && !this.CheckFightEnd())
            {
                this.OnTurnStarted();
            }
        }

        protected virtual void OnTurnStarted()
        {
            this.StartSequence(SequenceTypeEnum.SEQUENCE_TURN_START);
            this.FighterPlaying.DecrementAllCastedBuffsDuration();
            this.FighterPlaying.TriggerBuffs(BuffTriggerType.TURN_BEGIN, null);
            this.DecrementGlyphDuration(this.FighterPlaying);
            this.TriggerMarks(this.FighterPlaying.Cell, this.FighterPlaying, TriggerTypeFlag.TURN_BEGIN);
            this.EndSequence(SequenceTypeEnum.SEQUENCE_TURN_START, false);
            if (!this.CheckFightEnd())
            {
                if (this.TimeLine.NewRound)
                {
                    ContextHandler.SendGameFightNewRoundMessage(this.Clients, (int)this.TimeLine.RoundNumber);
                }

                this.FighterPlaying.StartTurn();
                if (this.FighterPlaying.IsDead() || this.FighterPlaying.MustSkipTurn())
                {
                    this.PassTurn();
                }
                else
                {
                    ContextHandler.SendGameFightTurnStartMessage(this.Clients, this.FighterPlaying.Id, this.FighterPlaying.GetWaitTime());
                    this.FighterPlaying.RefreshFighterStatsListMessage();
                    this.ForEach(delegate(Character entry)
                    {
                        ContextHandler.SendGameFightSynchronizeMessage(entry.Client, this);
                    }, true);
                    this.ForEach(delegate(Character entry)
                    {
                        entry.RefreshStats();
                    });
                    this.FighterPlaying.TurnStartPosition = this.FighterPlaying.Position.Clone();
                    this.TurnStartTime = DateTime.Now;
                    this.m_turnTimer = this.Map.Area.CallDelayed((int)this.FighterPlaying.GetWaitTime(), new Action(this.StopTurn));
                    Action<Fight, FightActor> turnStarted = this.TurnStarted;
                    if (turnStarted != null)
                    {
                        turnStarted(this, this.FighterPlaying);
                    }
                }
            }
        }

        public void StopTurn()
        {
            if (this.State == FightState.Fighting)
            {
                if (this.m_turnTimer != null)
                {
                    this.m_turnTimer.Dispose();
                }
                if (this.ReadyChecker != null)
                {
                    this.logger.Debug("Last ReadyChecker was not disposed. (Stop Turn)");
                    this.ReadyChecker.Cancel();
                    this.ReadyChecker = null;
                }
                if (!this.CheckFightEnd())
                {
                    this.OnTurnStopped();
                    this.ReadyChecker = ReadyChecker.RequestCheck(this, new Action(this.PassTurn), new Action<CharacterFighter[]>(this.LagAndPassTurn));
                }
            }
        }

        protected virtual void OnTurnStopped()
        {
            this.StartSequence(SequenceTypeEnum.SEQUENCE_TURN_END);
            this.FighterPlaying.TriggerBuffs(BuffTriggerType.TURN_END, null);
            this.FighterPlaying.TriggerBuffsRemovedOnTurnEnd();
            this.TriggerMarks(this.FighterPlaying.Cell, this.FighterPlaying, TriggerTypeFlag.TURN_END);
            this.FighterPlaying.StopTurn();
            this.FighterPlaying.ResetUsedPoints();
            this.EndSequence(SequenceTypeEnum.SEQUENCE_TURN_END, false);
            if (!this.CheckFightEnd())
            {
                if (this.IsSequencing)
                {
                    this.EndSequence(this.Sequence, true);
                }
                if (this.WaitAcknowledgment)
                {
                    this.AcknowledgeAction();
                }
                ContextHandler.SendGameFightTurnEndMessage(this.Clients, this.FighterPlaying);
            }
        }

        protected void LagAndPassTurn(NamedFighter[] laggers)
        {
            this.OnLaggersSpotted(laggers);
            this.PassTurn();
        }

        protected void PassTurn()
        {
            if (this.State == FightState.Fighting)
            {
                this.ReadyChecker = null;
                if (!this.CheckFightEnd())
                {
                    this.FighterPlaying.RefreshWaitTime(this.m_turnTimer.GetRemainingSeconds());
                    if (!this.TimeLine.SelectNextFighter())
                    {
                        if (!this.CheckFightEnd())
                        {
                            this.logger.Error("Something goes wrong : no more actors are available to play but the fight is not ended");
                        }
                    }
                    else
                    {
                        this.OnTurnPassed();
                        this.StartTurn();
                    }
                }
            }
        }

        protected virtual void OnTurnPassed()
        {
            if (this.IsSequencing)
            {
                this.EndSequence(this.Sequence, true);
            }
            if (this.WaitAcknowledgment)
            {
                this.AcknowledgeAction();
            }
        }

        private void UnBindFightersEvents()
        {
            foreach (FightActor current in this.Fighters)
            {
                this.UnBindFighterEvents(current);
            }
        }

        private void UnBindFighterEvents(FightActor actor)
        {
            actor.ReadyStateChanged -= new Action<FightActor, bool>(this.OnSetReady);
            actor.PrePlacementChanged -= new Action<FightActor, ObjectPosition>(this.OnChangePreplacementPosition);
            actor.FighterLeft -= new Action<FightActor>(this.OnPlayerLeft);
            actor.StartMoving -= new Action<ContextActor, Path>(this.OnStartMoving);
            actor.StopMoving -= new Action<ContextActor, Path, bool>(this.OnStopMoving);
            actor.PositionChanged -= new Action<ContextActor, ObjectPosition>(this.OnPositionChanged);
            actor.FightPointsVariation -= new FightActor.FightPointsVariationHandler(this.OnFightPointsVariation);
            actor.LifePointsChanged -= new Action<FightActor, int, int, FightActor>(this.OnLifePointsChanged);
            actor.DamageReducted -= new Action<FightActor, FightActor, int>(this.OnDamageReducted);
            actor.SpellCasting -= new FightActor.SpellCastingHandler(this.OnSpellCasting);
            actor.SpellCasted -= new FightActor.SpellCastingHandler(this.OnSpellCasted);
            actor.WeaponUsed -= new Action<FightActor, WeaponTemplate, Cell, FightSpellCastCriticalEnum, bool>(this.OnCloseCombat);
            actor.BuffAdded -= new Action<FightActor, Buff>(this.OnBuffAdded);
            actor.BuffRemoved -= new Action<FightActor, Buff>(this.OnBuffRemoved);
            actor.Dead -= new Action<FightActor, FightActor>(this.OnDead);
            CharacterFighter characterFighter = actor as CharacterFighter;
            if (characterFighter != null)
            {
                characterFighter.Character.LoggedOut -= new Action<Character>(this.OnPlayerLoggout);
            }
        }

        private void BindFightersEvents()
        {
            foreach (FightActor current in this.Fighters)
            {
                this.BindFighterEvents(current);
            }
        }

        private void BindFighterEvents(FightActor actor)
        {
            if (this.State == FightState.Placement)
            {
                actor.FighterLeft += new Action<FightActor>(this.OnPlayerLeft);
                actor.ReadyStateChanged += new Action<FightActor, bool>(this.OnSetReady);
                actor.PrePlacementChanged += new Action<FightActor, ObjectPosition>(this.OnChangePreplacementPosition);
            }
            if (this.State == FightState.Fighting)
            {
                actor.FighterLeft += new Action<FightActor>(this.OnPlayerLeft);
                actor.StartMoving += new Action<ContextActor, Path>(this.OnStartMoving);
                actor.StopMoving += new Action<ContextActor, Path, bool>(this.OnStopMoving);
                actor.PositionChanged += new Action<ContextActor, ObjectPosition>(this.OnPositionChanged);
                actor.FightPointsVariation += new FightActor.FightPointsVariationHandler(this.OnFightPointsVariation);
                actor.LifePointsChanged += new Action<FightActor, int, int, FightActor>(this.OnLifePointsChanged);
                actor.DamageReducted += new Action<FightActor, FightActor, int>(this.OnDamageReducted);
                actor.SpellCasting += new FightActor.SpellCastingHandler(this.OnSpellCasting);
                actor.SpellCasted += new FightActor.SpellCastingHandler(this.OnSpellCasted);
                actor.SpellCastFailed += new Action<FightActor, Spell, Cell>(this.OnSpellCastFailed);
                actor.WeaponUsed += new Action<FightActor, WeaponTemplate, Cell, FightSpellCastCriticalEnum, bool>(this.OnCloseCombat);
                actor.BuffAdded += new Action<FightActor, Buff>(this.OnBuffAdded);
                actor.BuffRemoved += new Action<FightActor, Buff>(this.OnBuffRemoved);
                actor.Dead += new Action<FightActor, FightActor>(this.OnDead);
            }
            CharacterFighter characterFighter = actor as CharacterFighter;
            if (characterFighter != null)
            {
                characterFighter.Character.LoggedOut += new Action<Character>(this.OnPlayerLoggout);
            }
        }

        protected virtual void OnDead(FightActor fighter, FightActor killedBy)
        {
            this.StartSequence(SequenceTypeEnum.SEQUENCE_CHARACTER_DEATH);
            ActionsHandler.SendGameActionFightDeathMessage(this.Clients, fighter);
            fighter.KillAllBombs();
            fighter.KillAllSummons();
            fighter.RemoveAndDispellAllBuffs();
            fighter.RemoveAllCastedBuffs();
            this.EndSequence(SequenceTypeEnum.SEQUENCE_CHARACTER_DEATH, false);
            MarkTrigger[] array = this.m_triggers.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                MarkTrigger markTrigger = array[i];
                if (markTrigger.Caster == fighter)
                {
                    this.RemoveTrigger(markTrigger);
                }
            }
            if(fighter is SummonedMonster)
            {
                var summon = fighter as SummonedMonster;
                
            }
        }

        protected virtual void OnStartMoving(ContextActor actor, Path path)
        {
            FightActor fighter = actor as FightActor;
            Character character = (actor is CharacterFighter) ? (actor as CharacterFighter).Character : null;
            if ((fighter == null || fighter.IsFighterTurn()) && (!path.IsEmpty() && path.MPCost != 0))
            {
                this.StartSequence(SequenceTypeEnum.SEQUENCE_MOVE);
                if (fighter != null && (fighter.GetTackledMP() > 0 || fighter.GetTackledAP() > 0))
                {
                    this.OnTackled(fighter, path);
                    if (path.IsEmpty() || path.MPCost == 0)
                    {
                        this.EndSequence(SequenceTypeEnum.SEQUENCE_MOVE, false);
                        return;
                    }
                }
                Cell[] path2 = path.GetPath();
                if (fighter != null)
                {
                    List<short> opponentsCells = (
                        from entry in fighter.OpposedTeam.GetAllFighters((FightActor entry) => entry.IsAlive() && entry.IsVisibleFor(fighter))
                        select entry.Cell.Id).ToList<short>();
                    List<short> list = (
                        from entry in this.GetAllFighters((FightActor entry) => entry != fighter && entry.IsAlive())
                        select entry.Cell.Id).ToList<short>();
                    for (int i = 0; i < path2.Length; i++)
                    {
                        if (i > 0 && this.ShouldTriggerOnMove(path2[i]))
                        {
                            path.CutPath(i + 1);
                            break;
                        }
                        bool arg_223_0;
                        if (i > 0 && fighter is CharacterFighter && fighter.VisibleState == GameActionFightInvisibilityStateEnum.VISIBLE)
                        {
                            arg_223_0 = !new MapPoint(path2[i]).GetAdjacentCells((short entry) => true).Any((MapPoint entry) => opponentsCells.Contains(entry.CellId));
                        }
                        else
                        {
                            arg_223_0 = true;
                        }
                        if (!arg_223_0)
                        {
                            path.CutPath(i + 1);
                            break;
                        }
                        if (list.Contains(path2[i].Id))
                        {
                            if (actor is FightActor)
                            {
                                var caster = actor as FightActor;
                                if (!caster.IsCarrying)
                                {
                                    if (character != null)
                                    {
                                        character.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 276, new object[0]);
                                    }
                                    path.CutPath(i);
                                    break;
                                }
                            }
                        }
                    }
                }
                IEnumerable<short> movementsKeys = path.GetServerPathKeys();
                this.ForEach(delegate(Character entry)
                {
                    if (entry.CanSee(fighter))
                    {
                        ContextHandler.SendGameMapMovementMessage(entry.Client, movementsKeys, fighter);
                    }
                }, true);
                actor.StopMove();
                this.EndSequence(SequenceTypeEnum.SEQUENCE_MOVE, false);
            }
        }

        protected virtual void OnTackled(FightActor actor, Path path)
        {
            FightActor[] tacklers = actor.GetTacklers();
            int tackledMP = actor.GetTackledMP();
            int tackledAP = actor.GetTackledAP();
            if (actor.MP - tackledMP < 0)
            {
                this.logger.Error<int, int>("Cannot apply tackle : mp tackled ({0}) > available mp ({1})", tackledMP, actor.MP);
            }
            else
            {
                ActionsHandler.SendGameActionFightTackledMessage(this.Clients, actor, tacklers);
                actor.LostAP((short)tackledAP);
                actor.LostMP((short)tackledMP);
                if (path.MPCost > actor.MP)
                {
                    path.CutPath(actor.MP + 1);
                }
            }
        }

        protected virtual void OnStopMoving(ContextActor actor, Path path, bool canceled)
        {
            FightActor fightActor = actor as FightActor;
            if ((fightActor == null || fightActor.IsFighterTurn()) && !canceled && fightActor != null)
            {
                fightActor.UseMP((short)path.MPCost);
                fightActor.TriggerBuffs(BuffTriggerType.MOVE, path);
            }
        }

        protected virtual void OnPositionChanged(ContextActor actor, ObjectPosition objectPosition)
        {
            FightActor fightActor = actor as FightActor;
            if (fightActor != null)
            {
                this.TriggerMarks(fightActor.Cell, fightActor, TriggerTypeFlag.MOVE);
            }
        }

        public void SwitchFighters(FightActor fighter1, FightActor fighter2)
        {
        }

        protected virtual void OnLifePointsChanged(FightActor actor, int delta, int permanentDamages, FightActor from)
        {
            short loss = (short)(-(short)delta);
            ActionsHandler.SendGameActionFightLifePointsLostMessage(this.Clients, from ?? actor, actor, loss, (short)permanentDamages);
        }

        protected virtual void OnFightPointsVariation(FightActor actor, ActionsEnum action, FightActor source, FightActor target, short delta)
        {
            ActionsHandler.SendGameActionFightPointsVariationMessage(this.Clients, action, source, target, delta);
        }

        protected virtual void OnDamageReducted(FightActor fighter, FightActor source, int reduction)
        {
            ActionsHandler.SendGameActionFightReduceDamagesMessage(this.Clients, source, fighter, reduction);
        }

        protected virtual void OnCloseCombat(FightActor caster, WeaponTemplate weapon, Cell targetCell, FightSpellCastCriticalEnum critical, bool silentCast)
        {
            FightActor target = this.GetOneFighter(targetCell);
            this.ForEach(delegate(Character entry)
            {
                ActionsHandler.SendGameActionFightCloseCombatMessage(entry.Client, caster, target, targetCell, critical, !caster.IsVisibleFor(entry) || silentCast, weapon);
            }, true);
        }

        protected virtual void OnSpellCasting(FightActor caster, Spell spell, Cell targetCell, FightSpellCastCriticalEnum critical, bool silentCast)
        {
            FightActor target = this.GetOneFighter(targetCell);
            this.ForEach(delegate(Character entry)
            {
                ContextHandler.SendGameActionFightSpellCastMessage(entry.Client, ActionsEnum.ACTION_FIGHT_CAST_SPELL, caster, target, targetCell, critical, !caster.IsVisibleFor(entry) || silentCast, spell);
            }, true);
        }

        protected virtual void OnSpellCasted(FightActor caster, Spell spell, Cell target, FightSpellCastCriticalEnum critical, bool silentCast)
        {
            this.EndSequence(SequenceTypeEnum.SEQUENCE_SPELL, false);
            this.CheckFightEnd();
        }

        protected virtual void OnSpellCastFailed(FightActor caster, Spell spell, Cell target)
        {
            ContextHandler.SendGameActionFightNoSpellCastMessage(this.Clients, spell);
        }

        public IEnumerable<Buff> GetBuffs()
        {
            return this.m_buffs;
        }

        public void UpdateBuff(Buff buff)
        {
            ContextHandler.SendGameActionFightDispellableEffectMessage(this.Clients, buff, true);
        }

        protected virtual void OnBuffAdded(FightActor target, Buff buff)
        {
            this.m_buffs.Add(buff);
            ContextHandler.SendGameActionFightDispellableEffectMessage(this.Clients, buff, false);
        }

        protected virtual void OnBuffRemoved(FightActor target, Buff buff)
        {
            this.m_buffs.Remove(buff);
            if (buff.Duration > 0 || buff.Duration == -1)
            {
                ActionsHandler.SendGameActionFightDispellEffectMessage(this.Clients, target, target, buff);
            }
        }

        public bool StartSequence(SequenceTypeEnum sequenceType)
        {
            this.m_lastSequenceAction = sequenceType;
            this.m_sequenceLevel++;
            bool result;
            if (this.IsSequencing)
            {
                result = false;
            }
            else
            {
                this.IsSequencing = true;
                this.Sequence = sequenceType;
                this.m_sequences.Push(sequenceType);
                ActionsHandler.SendSequenceStartMessage(this.Clients, this.TimeLine.Current, sequenceType);
                result = true;
            }
            return result;
        }

        public bool EndSequence(SequenceTypeEnum sequenceType, bool force = false)
        {
            bool result;
            if (!this.IsSequencing)
            {
                result = false;
            }
            else
            {
                this.m_sequenceLevel--;
                if (this.m_sequenceLevel > 0 && !force)
                {
                    result = false;
                }
                else
                {
                    this.IsSequencing = false;
                    this.WaitAcknowledgment = true;
                    SequenceTypeEnum sequenceTypeEnum = this.m_sequences.Pop();
                    if (sequenceTypeEnum != sequenceType)
                    {
                        this.logger.Debug<SequenceTypeEnum, SequenceTypeEnum>("Popped Sequence different ({0} != {1})", sequenceTypeEnum, sequenceType);
                    }
                    ActionsHandler.SendSequenceEndMessage(this.Clients, this.TimeLine.Current, sequenceType, this.m_lastSequenceAction);
                    result = true;
                }
            }
            return result;
        }

        public void EndAllSequences()
        {
            this.m_sequenceLevel = 0;
            this.IsSequencing = false;
            this.WaitAcknowledgment = false;
            while (this.m_sequences.Count > 0)
            {
                SequenceTypeEnum sequenceType = this.m_sequences.Pop();
                ActionsHandler.SendSequenceEndMessage(this.Clients, this.TimeLine.Current, sequenceType, this.m_lastSequenceAction);
            }
        }

        public virtual void AcknowledgeAction()
        {
            this.WaitAcknowledgment = false;
        }

        protected virtual void OnPlayerLeft(FightActor fighter)
        {
            if (this.State == FightState.Placement)
            {
                if (!(this is FightDuel))
                {
                    fighter.Stats.Health.DamageTaken += (int)((short)(fighter.LifePoints - 1));
                }
                if (!this.CheckFightEnd())
                {
                    fighter.Team.RemoveFighter(fighter);
                    if (fighter is CharacterFighter)
                    {
                        Character character = ((CharacterFighter)fighter).Character;
                        character.RejoinMap();
                    }
                }
            }
            else
            {
                fighter.Die();
                if (fighter is CharacterFighter && (fighter as CharacterFighter).Character.IsLoggedIn)
                {
                    ReadyChecker readyChecker = new ReadyChecker(this, new CharacterFighter[]
					{
						(CharacterFighter)fighter
					});
                    readyChecker.Success += delegate(ReadyChecker obj)
                    {
                        this.OnPlayerReadyToLeave(fighter as CharacterFighter);
                    };
                    readyChecker.Timeout += delegate(ReadyChecker obj, CharacterFighter[] laggers)
                    {
                        this.OnPlayerReadyToLeave(fighter as CharacterFighter);
                    };
                    ((CharacterFighter)fighter).PersonalReadyChecker = readyChecker;
                    readyChecker.Start();
                }
                else
                {
                    bool flag = fighter.IsFighterTurn();
                    ContextHandler.SendGameFightLeaveMessage(this.Clients, fighter);
                    if (!this.CheckFightEnd() && flag)
                    {
                        this.StopTurn();
                    }
                    fighter.ResetFightProperties();
                    fighter.Team.RemoveFighter(fighter);
                    fighter.Team.AddLeaver(fighter);
                    this.Leavers.Add(fighter);
                }
            }
        }

        protected virtual void OnPlayerReadyToLeave(CharacterFighter fighter)
        {
            fighter.PersonalReadyChecker = null;
            bool flag = fighter.IsFighterTurn();
            ContextHandler.SendGameFightLeaveMessage(this.Clients, fighter);
            if (!this.CheckFightEnd() && flag)
            {
                this.StopTurn();
            }
            fighter.ResetFightProperties();
            fighter.Character.RejoinMap();
            fighter.Team.RemoveFighter(fighter);
            fighter.Team.AddLeaver(fighter);
            this.Leavers.Add(fighter);
        }

        protected virtual void OnPlayerLoggout(Character character)
        {
            if (character.IsFighting() && character.Fight == this)
            {
                character.Fighter.EnterDisconnectedState();
                //character.Fighter.LeaveFight();

                BasicHandler.SendTextInformationMessage(this.Clients, TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 182, character.Name, character.Fighter.RemainingRounds.Value);
            }
        }

        public IEnumerable<MarkTrigger> GetTriggers()
        {
            return this.m_triggers;
        }

        public bool ShouldTriggerOnMove(Cell cell)
        {
            return this.m_triggers.Any((MarkTrigger entry) => (entry.TriggerType & TriggerTypeFlag.MOVE) == TriggerTypeFlag.MOVE && entry.ContainsCell(cell));
        }

        public MarkTrigger[] GetTriggers(Cell cell)
        {
            return (
                from entry in this.m_triggers
                where entry.CenterCell.Id == cell.Id
                select entry).ToArray<MarkTrigger>();
        }

        public void AddTrigger(MarkTrigger trigger)
        {
            trigger.Triggered += new Action<MarkTrigger, FightActor, Spell>(this.OnMarkTriggered);
            this.m_triggers.Add(trigger);
            foreach (CharacterFighter current in this.GetAllFighters<CharacterFighter>())
            {
                ContextHandler.SendGameActionFightMarkCellsMessage(current.Character.Client, trigger, trigger.DoesSeeTrigger(current));
            }
        }

        public void RemoveTrigger(MarkTrigger trigger)
        {
            trigger.Triggered -= new Action<MarkTrigger, FightActor, Spell>(this.OnMarkTriggered);
            this.m_triggers.Remove(trigger);
            ContextHandler.SendGameActionFightUnmarkCellsMessage(this.Clients, trigger);
        }

        public void TriggerMarks(Cell cell, FightActor trigger, TriggerTypeFlag triggerType)
        {
            MarkTrigger[] source = this.m_triggers.ToArray();
            foreach (MarkTrigger current in
                from markTrigger in source
                where (markTrigger.TriggerType & triggerType) == triggerType && markTrigger.ContainsCell(cell)
                select markTrigger)
            {
                this.StartSequence(SequenceTypeEnum.SEQUENCE_GLYPH_TRAP);
                if (current is Trap)
                {
                    this.RemoveTrigger(current);
                }
                current.Trigger(trigger);
                this.EndSequence(SequenceTypeEnum.SEQUENCE_GLYPH_TRAP, false);
            }
        }

        public void DecrementGlyphDuration(FightActor caster)
        {
            List<MarkTrigger> list = (
                from trigger in this.m_triggers
                where trigger.Caster == caster
                where trigger.DecrementDuration()
                select trigger).ToList<MarkTrigger>();
            if (list.Count != 0)
            {
                this.StartSequence(SequenceTypeEnum.SEQUENCE_GLYPH_TRAP);
                foreach (MarkTrigger current in list)
                {
                    this.RemoveTrigger(current);
                }
                this.EndSequence(SequenceTypeEnum.SEQUENCE_GLYPH_TRAP, false);
            }
        }

        public int PopNextTriggerId()
        {
            return this.m_triggerIdProvider.Pop();
        }

        public void FreeTriggerId(int id)
        {
            this.m_triggerIdProvider.Push(id);
        }

        private void OnMarkTriggered(MarkTrigger markTrigger, FightActor trigger, Spell triggerSpell)
        {
            ContextHandler.SendGameActionFightTriggerGlyphTrapMessage(this.Clients, markTrigger, trigger, triggerSpell);
        }

        protected virtual void OnLaggersSpotted(NamedFighter[] laggers)
        {
            if (laggers.Length == 1)
            {
                BasicHandler.SendTextInformationMessage(this.Clients, TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 28, new string[]
				{
					laggers[0].Name
				});
            }
            else
            {
                if (laggers.Length > 1)
                {
                    IPacketReceiver arg_7A_0 = this.Clients;
                    TextInformationTypeEnum arg_7A_1 = TextInformationTypeEnum.TEXT_INFORMATION_ERROR;
                    short arg_7A_2 = 29;
                    string[] array = new string[1];
                    array[0] = string.Join(",",
                        from entry in laggers
                        select entry.Name);
                    BasicHandler.SendTextInformationMessage(arg_7A_0, arg_7A_1, arg_7A_2, array);
                }
            }
        }

        protected abstract void SendGameFightJoinMessage(CharacterFighter fighter);

        protected abstract void SendGameFightJoinMessage(FightSpectator spectator);

        protected void GameFightReconnectMessage(CharacterFighter fighter)
        {
            var character = fighter.Character;

            this.Clients.Add(character.Client);
            ContextHandler.SendGameFightStartingMessage(fighter.Character.Client, this.FightType);
            this.SendGameFightJoinMessage(fighter);
            if (this.State == FightState.Placement || this.State == FightState.NotStarted)
            {
                ContextHandler.SendGameFightPlacementPossiblePositionsMessage(character.Client, this, fighter.Team.Id);
            }
            ContextHandler.SendIdolFightPreparationUpdateMessage(character.Client);
            foreach (FightActor current in this.GetAllFighters())
            {
                ContextHandler.SendGameFightShowFighterMessage(character.Client, current);
            }
            ContextHandler.SendGameEntitiesDispositionMessage(character.Client, this.GetAllFighters());
            ContextHandler.SendGameFightResumeMessage(character.Client, this, fighter);
            ContextHandler.SendGameFightTurnListMessage(character.Client, this);
            ContextHandler.SendGameFightSynchronizeMessage(character.Client, this);
            ContextHandler.SendGameFightTurnResumeMessage(character.Client, this.FighterPlaying, 0);
            //[15:24:47:467] [ServerConnection] [RCV] ChallengeInfoMessage @424
            //[15:24:47:467] [ServerConnection] [RCV] CharacterStatsListMessage @425
            ContextHandler.SendGameFightNewRoundMessage(character.Client, this.TimeLine.RoundNumber);
            //[15:24:47:467] [ServerConnection] [RCV] GameFightOptionStateUpdateMessage @427

            //character.Client.Send(new Stump.DofusProtocol.Messages.MapComplementaryInformationsDataMessage((ushort)this.Map.SubArea.Id, this.Map.Id, new HouseInformations[0],
            //    new GameRolePlayActorInformations[0], new InteractiveElement[0], new StatedElement[0], new MapObstacle[0], new FightCommonInformations[0]));

            character.Client.Send(character.Map.GetMapComplementaryInformationsDataMessage(character));
        }

        public IEnumerable<Character> GetAllCharacters()
        {
            return this.GetAllCharacters(false);
        }

        public IEnumerable<Character> GetAllCharacters(bool withSpectators = false)
        {
            IEnumerable<Character> result;
            if (!withSpectators)
            {
                result = from entry 
                         in this.Fighters.OfType<CharacterFighter>()
                         select entry.Character;
            }
            else
            {
                result = (from entry in this.Fighters.OfType<CharacterFighter>() select entry.Character).Concat( from entry in this.Spectators select entry.Character);
            }
            return result;
        }

        public void ForEach(Action<Character> action)
        {
            foreach (Character current in this.GetAllCharacters())
            {
                action(current);
            }
        }

        public void ForEach(Action<Character> action, bool withSpectators = false)
        {
            foreach (Character current in this.GetAllCharacters(withSpectators))
            {
                action(current);
            }
        }

        public void ForEach(Action<Character> action, Character except, bool withSpectators = false)
        {
            foreach (Character current in
                from character in this.GetAllCharacters(withSpectators)
                where character != except
                select character)
            {
                action(current);
            }
        }

        protected abstract bool CanCancelFight();

        public bool IsCellFree(Cell cell)
        {
            return cell.Walkable && !cell.NonWalkableDuringFight && this.GetOneFighter(cell) == null;
        }

        public int GetFightDuration()
        {
            return (!this.IsStarted) ? 0 : ((int)(DateTime.Now - this.StartTime).TotalMilliseconds);
        }

        public int GetTurnTimeLeft()
        {
            int result;
            if (this.TimeLine.Current == null)
            {
                result = 0;
            }
            else
            {
                double totalMilliseconds = (DateTime.Now - this.TurnStartTime).TotalMilliseconds;
                result = ((totalMilliseconds > 0.0) ? (TurnTime - (int)totalMilliseconds) : 0);
            }
            return result;
        }

        public sbyte GetNextContextualId()
        {
            return (sbyte)this.m_contextualIdProvider.Pop();
        }

        public void FreeContextualId(sbyte id)
        {
            this.m_contextualIdProvider.Push((int)id);
        }

        public void AddChallenge(ChallengeChecker challenge)
        {
            this.m_challenges.Add(challenge);
        }
        public ChallengeChecker GetChallenge(ushort challengeId)
        {
            return this.m_challenges.SingleOrDefault(entry => entry.ChallengeId == challengeId);
        }
        public IEnumerable<ChallengeChecker> GetChallenges()
        {
            return this.m_challenges;
        }

        public FightActor GetOneFighter(int id)
        {
            return this.Fighters.SingleOrDefault((FightActor entry) => entry.Id == id);
        }

        public FightActor GetOneFighter(Cell cell)
        {
            var carrier = this.Fighters.Where((FightActor entry) => entry.IsCarrying && entry.Cell.Id == cell.Id).FirstOrDefault();
            if(carrier == null)
            {
                return this.Fighters.SingleOrDefault((FightActor entry) => entry.IsAlive() && Equals(entry.Cell, cell));
            }
            else
            {
                return carrier;
            }
        }

        public FightActor GetOneFighter(Predicate<FightActor> predicate)
        {
            IEnumerable<FightActor> enumerable =
                from entry in this.Fighters
                where predicate(entry)
                select entry;
            FightActor[] source = (enumerable as FightActor[]) ?? enumerable.ToArray<FightActor>();
            return (source.Count<FightActor>() != 0) ? null : source.SingleOrDefault<FightActor>();
        }

        public T GetOneFighter<T>(int id) where T : FightActor
        {
            return this.Fighters.OfType<T>().SingleOrDefault((T entry) => entry.Id == id);
        }
        public T GetOneFighter<T>(Cell cell) where T : FightActor
        {
            return this.Fighters.OfType<T>().SingleOrDefault((T entry) => entry.IsAlive() && Equals(entry.Position.Cell, cell));
        }
        public T GetOneFighter<T>(Predicate<T> predicate) where T : FightActor
        {
            return this.Fighters.OfType<T>().SingleOrDefault((T entry) => predicate(entry));
        }
        public T GetFirstFighter<T>(int id) where T : FightActor
        {
            return this.Fighters.OfType<T>().FirstOrDefault((T entry) => entry.Id == id);
        }
        public T GetFirstFighter<T>(Cell cell) where T : FightActor
        {
            return this.Fighters.OfType<T>().FirstOrDefault((T entry) => entry.IsAlive() && Equals(entry.Position.Cell, cell));
        }
        public T GetFirstFighter<T>(Predicate<T> predicate) where T : FightActor
        {
            return this.Fighters.OfType<T>().FirstOrDefault((T entry) => predicate(entry));
        }
        public System.Collections.ObjectModel.ReadOnlyCollection<FightActor> GetAllFighters()
        {
            return this.Fighters.AsReadOnly();
        }
        public System.Collections.ObjectModel.ReadOnlyCollection<FightActor> GetLeavers()
        {
            return this.Leavers.AsReadOnly();
        }
        public CharacterFighter GetLeaver(int characterId)
        {
            return this.Leavers.OfType<CharacterFighter>().SingleOrDefault((CharacterFighter x) => x.Id == characterId);
        }

        public System.Collections.ObjectModel.ReadOnlyCollection<FightSpectator> GetSpectators()
        {
            return this.Spectators.AsReadOnly();
        }

        public IEnumerable<Character> GetCharactersAndSpectators()
        {
            return this.GetAllCharacters().Concat(
                from entry in this.GetSpectators()
                select entry.Character);
        }

        public IEnumerable<FightActor> GetFightersAndLeavers()
        {
            return this.Fighters.Concat(this.Leavers);
        }

        public IEnumerable<FightActor> GetAllFighters(Cell[] cells)
        {
            return this.GetAllFighters<FightActor>((FightActor entry) => entry.IsAlive() && cells.Contains(entry.Position.Cell));
        }

        public IEnumerable<FightActor> GetAllFighters(IEnumerable<Cell> cells)
        {
            return this.GetAllFighters(cells.ToArray<Cell>());
        }

        public IEnumerable<FightActor> GetAllFighters(Predicate<FightActor> predicate)
        {
            return
                from entry in this.Fighters
                where predicate(entry)
                select entry;
        }

        public IEnumerable<T> GetAllFighters<T>() where T : FightActor
        {
            return this.Fighters.OfType<T>();
        }

        public IEnumerable<T> GetAllFighters<T>(Predicate<T> predicate) where T : FightActor
        {
            return
                from entry in this.Fighters.OfType<T>()
                where predicate(entry)
                select entry;
        }

        public IEnumerable<int> GetDeadFightersIds()
        {
            return
                from entry in this.GetFightersAndLeavers()
                where entry.IsDead()
                select entry.Id;
        }

        public IEnumerable<int> GetAliveFightersIds()
        {
            return from entry in this.GetAllFighters<FightActor>((FightActor entry) => entry.IsAlive() && entry.IsVisibleInTimeline)
                   select entry.Id;
        }

        public int GetFightBonus()
        {
            return this.AgeBonus + this.m_challenges.Sum(entry => entry.GetChallengeBonus());
        }

        public FightCommonInformations GetFightCommonInformations()
        {
            return new FightCommonInformations(this.Id, (sbyte)this.FightType,
                from entry in this.m_teams
                select entry.GetFightTeamInformations(), this.m_teams.Select((FightTeam entry) => (ushort)entry.BladePosition.Cell.Id), this.m_teams.Select((FightTeam entry) => entry.GetFightOptionsInformations()));
        }

        public FightExternalInformations GetFightExternalInformations()
        {
            return new FightExternalInformations(this.Id, (sbyte)FightType, this.StartTime.GetUnixTimeStamp(), this.SpectatorClosed || this.State != FightState.Fighting,
                from entry in this.m_teams
                select entry.GetFightTeamLightInformations(), this.m_teams.Select((FightTeam entry) => entry.GetFightOptionsInformations()));
        }

        public void Reconnect(CharacterFighter characterFighter)
        {
            if (characterFighter.Fight == this)
            {
                if (this.Fighters.Contains(characterFighter))
                {
                    characterFighter.LeaveDisconnectedState();

                    this.GameFightReconnectMessage(characterFighter);
                }
            }
        }

        //public bool VerifyTargetMask(int pCasterId, int pTargetId, Effects.Instances.EffectBase pEffect, int pSpellImpactCell, double pTriggeringSpellCasterId = 0)
        //{
        //    //HOLY SHIT THAT'S AWFUL 

        //    Regex r = null;
        //    //exclusiveMasks = null;
        //    //string exclusiveMask = null;
        //    string exclusiveMaskParam = null;
        //    bool exclusiveMaskCasterOnly = false;
        //    bool verify = false;
        //    //var summonedTargetCanPlay:* = false;
        //    int maskState = 0;
        //    Dictionary<string, int> multipleMasks = null;
        //    List<string> masksTypes= null;
        //    string maskType = null;
        //    List<string> verifiedMasks = null;
        //    bool isMultipleMask = false;
        //    string lastMaskType = null;
        //    int multipleMaskCount = 0;
        //    //var fef:FightEntitiesFrame = Kernel.getWorker().getFrame(FightEntitiesFrame) as FightEntitiesFrame;
        //    //if (!fef)
        //    //{
        //    //    return true;
        //    //}
        //    if ((pEffect == null) || (pEffect.Delay > 0) || (string.IsNullOrEmpty(pEffect.TargetMask)))
        //    {
        //        return false;
        //    }
        //    //var target:TiphonSprite = DofusEntities.getEntity(pTargetId) as AnimatedCharacter;
        //    bool targetIsCaster = pTargetId == pCasterId;

        //    FightActor casterInfos = GetOneFighter(pCasterId);
        //    FightActor targetInfos = GetOneFighter(pTargetId);
        //    FightActor monsterInfo = targetInfos as MonsterFighter;
        //    bool targetIsCarried = targetInfos.IsCarried;
        //    //var casterStates = casterInfos.sta
        //    //var casterStates:Array = FightersStateManager.getInstance().getStates(pCasterId);
        //    //var targetStates:Array = FightersStateManager.getInstance().getStates(pTargetId);
        //    bool isTargetAlly = targetInfos.Team.Id == casterInfos.Team.Id;
        //    string targetMaskPattern = "";
        //    //if ((pCasterId == CurrentPlayedFighterManager.getInstance().currentFighterId) /*&& (pEffect.category == 0)*/ && (pEffect.targetMask == "C"))
        //    //{
        //    //    return true;
        //    //}
        //    if (targetIsCaster)
        //    {
        //        if (pEffect.EffectId == EffectsEnum.Effect_GiveHPPercent) //90
        //        {
        //            return true;
        //        }
        //        if (pEffect.TargetMask.IndexOf("g") == -1)
        //        {
        //            /*if (verifySpellEffectZone(pCasterId, pEffect, pSpellImpactCell, targetInfos.Position.Cell.Id))
        //            {
        //                targetMaskPattern = "caC";
        //            }
        //            else
        //            {
        //                targetMaskPattern = "C";
        //            }*/
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    else
        //    {
        //        if ((targetIsCarried) && (!(pEffect.ZoneShape == SpellShapeEnum.A))) /*&& (!(pEffect.ZoneShape == SpellShapeEnum.a)))*/
        //        {
        //            return false;
        //        }
        //        /*if ((targetInfos.Stats.summoned) && (monsterInfo) && (!Monster.getMonsterById(monsterInfo.creatureGenericId).canPlay))
        //        {
        //            targetMaskPattern = isTargetAlly ? "sj" : "SJ";
        //        }
        //        else if (targetInfos.stats.summoned)
        //        {
        //            targetMaskPattern = isTargetAlly ? "ij" : "IJ";
        //            summonedTargetCanPlay = true;
        //        }
        //        else if (targetInfos is GameFightCompanionInformations)
        //        {
        //            targetMaskPattern = isTargetAlly ? "dl" : "DL";
        //        }*/


        //        if (monsterInfo != null)
        //        {
        //            targetMaskPattern = targetMaskPattern + (isTargetAlly ? "ag" : "A");
        //            /*if (!summonedTargetCanPlay)
        //            {
        //                targetMaskPattern = targetMaskPattern + (isTargetAlly ? "m" : "M");
        //            }*/
        //        }
        //        else
        //        {
        //            targetMaskPattern = targetMaskPattern + (isTargetAlly ? "aghl" : "AHL");
        //        }
        //    }
        //    var AT_LEAST_MASK_TYPES = new string[] { "B", "F", "Z" };
        //    r = new Regex("[" + targetMaskPattern + "]");
        //    Regex exclusiveTargetMasks = new Regex("\\*?[bBeEfFzZKoOPpTWUvV][0 - 9]*");
        //    verify = r.IsMatch(pEffect.TargetMask);
        //    if (verify)
        //    {
        //        var exclusiveMasks = exclusiveTargetMasks.Matches(pEffect.TargetMask);
        //        if (exclusiveMasks.Count > 0)
        //        {
        //            verify = false;
        //            multipleMasks = new Dictionary<string, int>();
        //            masksTypes = new List<string>();
        //            foreach(Match exclusiveMask in exclusiveMasks)
        //            {
        //                maskType = Convert.ToString(exclusiveMask.Value[0]);
        //                if (maskType == "*")
        //                {
        //                    maskType = exclusiveMask.Value.Substring(0, 2);
        //                }
        //                if (AT_LEAST_MASK_TYPES.Any(x => x == maskType))
        //                {
        //                    if (masksTypes.Any(x => x == maskType))
        //                    {
        //                        if (!multipleMasks.ContainsKey(maskType))
        //                        {
        //                            multipleMasks.Add(maskType, 2);
        //                        }
        //                        else
        //                        {
        //                            multipleMasks[maskType]++;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        masksTypes.Add(maskType);
        //                    }
        //                }
        //            }
        //            verifiedMasks = new List<string>();
        //            foreach(Match exclusiveMask in exclusiveMasks)
        //            {
        //                exclusiveMaskCasterOnly = exclusiveMask.Value[0] == '*';
        //                string eM = exclusiveMaskCasterOnly ? exclusiveMask.Value.Substring(1, exclusiveMask.Length - 1) : exclusiveMask.Value;
        //                exclusiveMaskParam = eM.Length > 1 ? eM.Substring(1, eM.Length - 1) : null;
        //                eM = Convert.ToString(eM[0]);
        //                switch (eM)
        //                {
        //                    case "b":
        //                        break;
        //                    case "B":
        //                        break;
        //                    case "e":
        //                        maskState = Int32.Parse(exclusiveMaskParam);
        //                        if (exclusiveMaskCasterOnly)
        //                        {
        //                            verify = !casterInfos.HasState(maskState);
        //                        }
        //                        else
        //                        {
        //                            verify = !targetInfos.HasState(maskState);
        //                        }
        //                        break;
        //                    case "E":
        //                        maskState = Int32.Parse(exclusiveMaskParam);
        //                        if (exclusiveMaskCasterOnly)
        //                        {
        //                            verify = casterInfos.HasState(maskState);
        //                        }
        //                        else
        //                        {
        //                            verify = targetInfos.HasState(maskState);
        //                        }
        //                        break;
        //                    case "f":
        //                        //verify = (monsterInfo != null) || (!(monsterInfo.GenericId == Int32.Parse(exclusiveMaskParam)));
        //                        break;
        //                    case "F":
        //                        //verify = (monsterInfo != null) && (monsterInfo.creatureGenericId == Int32.Parse(exclusiveMaskParam));
        //                        break;
        //                    case "z":
        //                        break;
        //                    case "Z":
        //                        break;
        //                    case "K":
        //                        break;
        //                    case "o":
        //                        verify = (!(pTriggeringSpellCasterId == 0)) && (pTargetId == pTriggeringSpellCasterId); /* && (verifySpellEffectZone(pTriggeringSpellCasterId, pEffect, pSpellImpactCell, casterInfos.disposition.cellId));*/
        //                        break;
        //                    case "O":
        //                        verify = (!(pTriggeringSpellCasterId == 0)) && (pTargetId == pTriggeringSpellCasterId);
        //                        break;
        //                    case "p":
        //                        break;
        //                    case "P":
        //                        break;
        //                    case "T":
        //                        break;
        //                    case "W":
        //                        break;
        //                    case "U":
        //                        break;
        //                    case "v":
        //                        verify = targetInfos.Stats.Health.TotalMax / targetInfos.Stats.Health.TotalMax * 100 > Int32.Parse(exclusiveMaskParam);
        //                        break;
        //                    case "V":
        //                        verify = targetInfos.Stats.Health.TotalMax / targetInfos.Stats.Health.TotalMax * 100 <= Int32.Parse(exclusiveMaskParam);
        //                        break;
        //                }
        //                maskType = exclusiveMaskCasterOnly ? "*" + eM : eM;
        //                isMultipleMask = Convert.ToBoolean(multipleMasks[maskType]);
        //                if ((!string.IsNullOrEmpty(lastMaskType)) || (maskType == lastMaskType))
        //                {
        //                    multipleMaskCount++;
        //                }
        //                else
        //                {
        //                    multipleMaskCount = 0;
        //                }
        //                lastMaskType = maskType;
        //                if ((verify) && (isMultipleMask) && (verifiedMasks.Any(x => x == maskType)))
        //                {
        //                    verifiedMasks.Add(maskType);
        //                }
        //                if (!verify)
        //                {
        //                    if (!isMultipleMask)
        //                    {
        //                        return false;
        //                    }
        //                    if (verifiedMasks.Any(x => x == maskType))
        //                    {
        //                        verify = true;
        //                    }
        //                    else if (multipleMasks[maskType] == multipleMaskCount)
        //                    {
        //                        return false;
        //                    }

        //                }
        //            }
        //        }
        //    }
        //    return verify;
        //}
    }
}