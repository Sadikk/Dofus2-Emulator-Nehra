using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Maps.Cells;
using System;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Fights.Teams
{
	public abstract class FightTeam
	{
        // FIELDS
		private readonly System.Collections.Generic.List<FightActor> m_fighters = new System.Collections.Generic.List<FightActor>();
		private readonly System.Collections.Generic.List<FightActor> m_leavers = new System.Collections.Generic.List<FightActor>();
		private readonly object m_locker = new object();

        public event Action<FightTeam, FightActor> FighterAdded;
        public event Action<FightTeam, FightActor> FighterRemoved;
        public event Action<FightTeam, FightOptionsEnum> TeamOptionsChanged;
        //public event Action<FightTeam, FightActor, FightActor> FighterDied;

        // PROPERTIES
		public sbyte Id
		{
			get;
			private set;
		}
		public ObjectPosition BladePosition
		{
			get;
			set;
		}
		public Cell[] PlacementCells
		{
			get;
			private set;
		}
		public AlignmentSideEnum AlignmentSide
		{
			get;
			private set;
		}
		public abstract TeamTypeEnum TeamType
		{
			get;
		}
		public Fight Fight
		{
			get;
			internal set;
		}
		public virtual FightActor Leader
		{
			get
			{
				return (this.m_fighters.Count > 0) ? this.m_fighters.First<FightActor>() : null;
			}
		}
		public bool IsSecret
		{
			get;
			private set;
		}
		public bool IsRestrictedToParty
		{
			get;
			private set;
		}
		public bool IsClosed
		{
			get;
			private set;
		}
		public bool IsAskingForHelp
		{
			get;
			private set;
		}
		public System.Collections.ObjectModel.ReadOnlyCollection<FightActor> Fighters
		{
			get
			{
				return this.m_fighters.AsReadOnly();
			}
		}
		public System.Collections.ObjectModel.ReadOnlyCollection<FightActor> Leavers
		{
			get
			{
				return this.m_leavers.AsReadOnly();
			}
		}

        // CONSTRUCTORS
        protected FightTeam(sbyte id, Cell[] placementCells)
        {
            this.Id = id;
            this.PlacementCells = placementCells;
            this.AlignmentSide = AlignmentSideEnum.ALIGNMENT_WITHOUT;
        }
        protected FightTeam(sbyte id, Cell[] placementCells, AlignmentSideEnum alignmentSide)
        {
            this.Id = id;
            this.PlacementCells = placementCells;
            this.AlignmentSide = alignmentSide;
        }

        // METHODS
		protected virtual void OnFighterAdded(FightActor fighter)
		{
			Action<FightTeam, FightActor> fighterAdded = this.FighterAdded;
			if (fighterAdded != null)
			{
				fighterAdded(this, fighter);
			}
		}
		protected virtual void OnTeamOptionsChanged(FightOptionsEnum option)
		{
			Action<FightTeam, FightOptionsEnum> teamOptionsChanged = this.TeamOptionsChanged;
			if (teamOptionsChanged != null)
			{
				teamOptionsChanged(this, option);
			}
		}
		protected virtual void OnFighterRemoved(FightActor fighter)
		{
			Action<FightTeam, FightActor> fighterRemoved = this.FighterRemoved;
			if (fighterRemoved != null)
			{
				fighterRemoved(this, fighter);
			}
		}
        //protected virtual void OnFighterDied(FightActor fighter, FightActor killedBy)
        //{
        //    Action<FightTeam, FightActor, FightActor> fighterDied = this.FighterDied;
        //    if (fighterDied != null)
        //    {
        //        fighterDied(this, fighter, killedBy);
        //    }
        //}

		public virtual bool ChangeLeader(FightActor leader)
		{
			bool result;
			if (!this.m_fighters.Contains(leader))
			{
				result = false;
			}
			else
			{
				if (this.m_fighters.Count > 1)
				{
					this.m_fighters.Remove(leader);
					this.m_fighters.Insert(0, leader);
				}
				else
				{
					this.m_fighters.Add(leader);
				}
				result = true;
			}
			return result;
		}
		public void ToggleOption(FightOptionsEnum option)
		{
			switch (option)
			{
			case FightOptionsEnum.FIGHT_OPTION_SET_SECRET:
				this.IsSecret = !this.IsSecret;
				break;
			case FightOptionsEnum.FIGHT_OPTION_SET_TO_PARTY_ONLY:
				this.IsRestrictedToParty = !this.IsRestrictedToParty;
				break;
			case FightOptionsEnum.FIGHT_OPTION_SET_CLOSED:
				this.IsClosed = !this.IsClosed;
				break;
			case FightOptionsEnum.FIGHT_OPTION_ASK_FOR_HELP:
				this.IsAskingForHelp = !this.IsAskingForHelp;
				break;
			}
			this.OnTeamOptionsChanged(option);
		}
		public bool GetOptionState(FightOptionsEnum option)
		{
			bool result;
			switch (option)
			{
			case FightOptionsEnum.FIGHT_OPTION_SET_SECRET:
				result = this.IsSecret;
				break;
			case FightOptionsEnum.FIGHT_OPTION_SET_TO_PARTY_ONLY:
				result = this.IsRestrictedToParty;
				break;
			case FightOptionsEnum.FIGHT_OPTION_SET_CLOSED:
				result = this.IsClosed;
				break;
			case FightOptionsEnum.FIGHT_OPTION_ASK_FOR_HELP:
				result = this.IsAskingForHelp;
				break;
			default:
				result = false;
				break;
			}
			return result;
		}
		public bool AreAllReady()
		{
			return this.m_fighters.All((FightActor entry) => entry.IsReady);
		}
		public bool AreAllDead()
		{
			bool arg_39_0;
			if (this.m_fighters.Count > 0)
			{
				arg_39_0 = this.m_fighters.All((FightActor entry) => entry.IsDead() || entry.HasLeft());
			}
			else
			{
				arg_39_0 = true;
			}
			return arg_39_0;
		}
		public bool IsFull()
		{
			return this.Fight.State == FightState.Placement && this.m_fighters.Count > this.PlacementCells.Count<Cell>();
		}
		public virtual FighterRefusedReasonEnum CanJoin(Character character)
		{
			FighterRefusedReasonEnum result;
			if (this.Fight.State != FightState.Placement)
			{
				result = FighterRefusedReasonEnum.TOO_LATE;
			}
			else
			{
				if (this.IsFull())
				{
					result = FighterRefusedReasonEnum.TEAM_FULL;
				}
				else
				{
					if (this.IsClosed)
					{
						result = FighterRefusedReasonEnum.TEAM_LIMITED_BY_MAINCHARACTER;
					}
					else
					{
						if (this.IsSecret)
						{
							result = FighterRefusedReasonEnum.TEAM_LIMITED_BY_MAINCHARACTER;
						}
						else
						{
							if (this.AlignmentSide != AlignmentSideEnum.ALIGNMENT_WITHOUT && character.AlignmentSide != this.AlignmentSide)
							{
								result = FighterRefusedReasonEnum.WRONG_ALIGNMENT;
							}
							else
							{
								if (this.AlignmentSide != AlignmentSideEnum.ALIGNMENT_WITHOUT && !character.PvPEnabled)
								{
									result = FighterRefusedReasonEnum.INSUFFICIENT_RIGHTS;
								}
								else
								{
									result = FighterRefusedReasonEnum.FIGHTER_ACCEPTED;
								}
							}
						}
					}
				}
			}
			return result;
		}
		public bool AddFighter(FightActor actor)
		{
			bool result;
			if (this.IsFull())
			{
				result = false;
			}
			else
			{
				lock (this.m_locker)
				{
					this.m_fighters.Add(actor);
					this.OnFighterAdded(actor);
					result = true;
				}
			}
			return result;
		}
		public bool RemoveFighter(FightActor actor)
		{
			bool result;
			if (this.IsFull())
			{
				result = false;
			}
			else
			{
				lock (this.m_locker)
				{
					if (!this.m_fighters.Remove(actor))
					{
						result = false;
					}
					else
					{
						this.OnFighterRemoved(actor);
						result = true;
					}
				}
			}
			return result;
		}
		public void RemoveAllFighters()
		{
			lock (this.m_locker)
			{
				foreach (FightActor current in this.m_fighters)
				{
					this.OnFighterRemoved(current);
				}
				this.m_fighters.Clear();
			}
		}
		public void AddLeaver(FightActor leaver)
		{
			this.m_leavers.Add(leaver);
		}
		public bool RemoveLeaver(FightActor leaver)
		{
			return this.m_leavers.Remove(leaver);
		}
		public FightActor GetOneFighter(int id)
		{
			return this.m_fighters.SingleOrDefault((FightActor entry) => entry.Id == id);
		}
		public FightActor GetOneFighter(Cell cell)
		{
			return this.m_fighters.SingleOrDefault((FightActor entry) => object.Equals(entry.Position.Cell, cell));
		}
		public FightActor GetOneFighter(System.Predicate<FightActor> predicate)
		{
			return this.m_fighters.SingleOrDefault((FightActor entry) => predicate(entry));
		}
		public T GetOneFighter<T>(int id) where T : FightActor
		{
			return this.m_fighters.OfType<T>().SingleOrDefault((T entry) => entry.Id == id);
		}
		public T GetOneFighter<T>(System.Predicate<T> predicate) where T : FightActor
		{
			return this.m_fighters.OfType<T>().SingleOrDefault((T entry) => predicate(entry));
		}
		public System.Collections.Generic.IEnumerable<FightActor> GetAllFighters()
		{
			return this.m_fighters;
		}
		public System.Collections.Generic.IEnumerable<FightActor> GetAllFightersWithLeavers()
		{
			return this.m_fighters.Concat(this.m_leavers);
		}
		public System.Collections.Generic.IEnumerable<FightActor> GetAllFighters(Cell[] cells)
		{
			return this.GetAllFighters<FightActor>((FightActor entry) => cells.Contains(entry.Position.Cell));
		}
		public System.Collections.Generic.IEnumerable<FightActor> GetAllFighters(System.Predicate<FightActor> predicate)
		{
			return 
				from entry in this.GetAllFighters()
				where predicate(entry)
				select entry;
		}
		public System.Collections.Generic.IEnumerable<FightActor> GetAllFightersWithLeavers(System.Predicate<FightActor> predicate)
		{
			return 
				from entry in this.GetAllFightersWithLeavers()
				where predicate(entry)
				select entry;
		}
		public System.Collections.Generic.IEnumerable<T> GetAllFighters<T>() where T : FightActor
		{
			return this.m_fighters.OfType<T>();
		}
		public System.Collections.Generic.IEnumerable<T> GetAllFighters<T>(System.Predicate<T> predicate) where T : FightActor
		{
			return 
				from entry in this.m_fighters.OfType<T>()
				where predicate(entry)
				select entry;
		}
		public FightTeamInformations GetFightTeamInformations()
		{
			return new FightTeamInformations(this.Id, (this.Leader != null) ? this.Leader.Id : 0, (sbyte)this.AlignmentSide, (sbyte)this.TeamType, 0,
                from entry in this.m_fighters
				select entry.GetFightTeamMemberInformations());
		}
		public FightOptionsInformations GetFightOptionsInformations()
		{
			return new FightOptionsInformations(this.IsSecret, this.IsRestrictedToParty, this.IsClosed, this.IsAskingForHelp);
		}
		public FightTeamLightInformations GetFightTeamLightInformations()
		{
			return new FightTeamLightInformations(this.Id, (this.Leader == null) ? 0 : this.Leader.Id, (sbyte)this.AlignmentSide, (sbyte)this.TeamType, 0, false, false, false, false, false, (sbyte)this.m_fighters.Count, 1);
		}
	}
}
