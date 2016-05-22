using Stump.Core.Pool;
using Stump.Core.Reflection;
using Stump.Core.Threading;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Messages;
using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Database.Items.Templates;
using Stump.Server.WorldServer.Database.Spells;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Interfaces;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Actors.Stats;
using Stump.Server.WorldServer.Game.Effects;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Fights;
using Stump.Server.WorldServer.Game.Fights.Buffs;
using Stump.Server.WorldServer.Game.Fights.Buffs.Customs;
using Stump.Server.WorldServer.Game.Fights.History;
using Stump.Server.WorldServer.Game.Fights.Results;
using Stump.Server.WorldServer.Game.Fights.Teams;
using Stump.Server.WorldServer.Game.Fights.Triggers;
using Stump.Server.WorldServer.Game.Items;
using Stump.Server.WorldServer.Game.Maps;
using Stump.Server.WorldServer.Game.Maps.Cells;
using Stump.Server.WorldServer.Game.Maps.Cells.Shapes;
using Stump.Server.WorldServer.Game.Spells;
using Stump.Server.WorldServer.Game.Spells.Casts;
using Stump.Server.WorldServer.Handlers.Actions;
using Stump.Server.WorldServer.Handlers.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.Server.WorldServer.Game.Maps.Pathfinding;
using Stump.DofusProtocol.Enums.HomeMade;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Monsters;
using Stump.Server.WorldServer.Game.Spells.Sadida;

namespace Stump.Server.WorldServer.Game.Actors.Fight
{
	public abstract class FightActor : ContextActor, IStatsOwner
	{
        // FIELDS
		public delegate void SpellCastingHandler(FightActor caster, Spell spell, Cell target, FightSpellCastCriticalEnum critical, bool silentCast);
		public delegate void FightPointsVariationHandler(FightActor actor, ActionsEnum action, FightActor source, FightActor target, short delta);

		private readonly System.Collections.Generic.Dictionary<Spell, short> m_buffedSpells = new System.Collections.Generic.Dictionary<Spell, short>();
		private readonly UniqueIdProvider m_buffIdProvider = new UniqueIdProvider();
		private readonly System.Collections.Generic.List<Buff> m_buffList = new System.Collections.Generic.List<Buff>();
		private readonly System.Collections.Generic.List<SummonedFighter> m_summons = new System.Collections.Generic.List<SummonedFighter>();
        private readonly System.Collections.Generic.List<BombFighter> m_bombs = new System.Collections.Generic.List<BombFighter>();
		private readonly System.Collections.Generic.List<SpellState> m_states = new System.Collections.Generic.List<SpellState>();
        private bool m_left;
        protected uint? _waitTime;
        
		public event Action<FightActor, bool> ReadyStateChanged;
		public event Action<FightActor, Cell, bool> CellShown;
		public event Action<FightActor, int, int, FightActor> LifePointsChanged;
		public event Action<FightActor, Damage> BeforeDamageInflicted;
		public event Action<FightActor, Damage> DamageInflicted;
		public event Action<FightActor, FightActor, int> DamageReducted;
		public event Action<FightActor, FightActor, int> DamageReflected;
		public event Action<FightActor> FighterLeft;
		public event Action<FightActor, ObjectPosition> PrePlacementChanged;
        public event Action<FightActor> TurnStarted;
		public event Action<FightActor> TurnPassed;
		public event FightActor.SpellCastingHandler SpellCasting;
		public event FightActor.SpellCastingHandler SpellCasted;
		public event Action<FightActor, Spell, Cell> SpellCastFailed;
		public event Action<FightActor, WeaponTemplate, Cell, FightSpellCastCriticalEnum, bool> WeaponUsed;
		public event Action<FightActor, Buff> BuffAdded;
		public event Action<FightActor, Buff> BuffRemoved;
		public event Action<FightActor, FightActor> Dead;
		public event FightActor.FightPointsVariationHandler FightPointsVariation;
		public event Action<FightActor, short> ApUsed;
		public event Action<FightActor, short> MpUsed;

        // PROPERTIES
		public Fights.Fight Fight
		{
			get
			{
				return this.Team.Fight;
			}
		}
		public FightTeam Team
		{
			get;
			private set;
		}
		public FightTeam OpposedTeam
		{
			get;
			private set;
		}
		public override ICharacterContainer CharacterContainer
		{
			get
			{
				return this.Fight;
			}
		}
		public abstract ObjectPosition MapPosition
		{
			get;
		}
		public virtual bool IsReady
		{
			get;
			protected set;
		}
		public SpellHistory SpellHistory
		{
			get;
			private set;
		}
		public ObjectPosition TurnStartPosition
		{
			get;
			internal set;
		}
		public ObjectPosition FightStartPosition
		{
			get;
			internal set;
		}
		public override bool BlockSight
		{
			get
			{
				return !this.IsDead();
			}
		}
		public abstract short Level
		{
			get;
		}
		public int LifePoints
		{
			get
			{
				return this.Stats.Health.TotalSafe;
			}
		}
		public int MaxLifePoints
		{
			get
			{
				return this.Stats.Health.TotalMax;
			}
		}
		public int DamageTaken
		{
			get
			{
				return this.Stats.Health.DamageTaken;
			}
			set
			{
				this.Stats.Health.DamageTaken = value;
			}
		}
		public int AP
		{
			get
			{
				return this.Stats.AP.Total;
			}
		}
		public short UsedAP
		{
			get
			{
				return this.Stats.AP.Used;
			}
		}
		public int MP
		{
			get
			{
				return this.Stats.MP.Total;
			}
		}
		public short UsedMP
		{
			get
			{
				return this.Stats.MP.Used;
			}
		}
		public abstract StatsFields Stats
		{
			get;
		}
		public Stump.Server.WorldServer.Game.Fights.Results.FightLoot Loot
		{
			get;
			private set;
		}
		public int SummonedCount
		{
			get;
			private set;
		}
        public int BombsCount
        {
            get
            {
                return this.m_bombs.Count;
            }
        }
		public GameActionFightInvisibilityStateEnum VisibleState
		{
			get;
			private set;
		}
        public FightActor Carrier
        {
            get;
            private set;
        }
        public FightActor Carried
        {
            get;
            private set;
        }
        public bool IsCarrying
        {
            get
            {
                return (this.Carrier == this);
            }
        }
        public bool IsCarried
        {
            get
            {
                return (this.Carried == this);
            }
        }
        public override ObjectPosition Position
        {
            get
            {
                return base.Position;
            }

            protected set
            {
                this.PreviousPosition = base.Position;
                base.Position = value;
                if(this.IsCarrying)
                {
                    this.Carried.Position = value;
                }
                
            }
        }
        public ObjectPosition PreviousPosition
        {
            get;
            private set;
        }
        public double DamageMultiplicator
        {
            get;
            set;
        }
        public virtual bool IsVisibleInTimeline
        {
            get
            {
                return true;
            }
        }


        // CONSTRUCTORS
        protected FightActor(FightTeam team)
        {
            this.Team = team;
            this.OpposedTeam = ((this.Fight.BlueTeam == this.Team) ? this.Fight.RedTeam : this.Fight.BlueTeam);
            this.VisibleState = GameActionFightInvisibilityStateEnum.VISIBLE;
            this.Loot = new Stump.Server.WorldServer.Game.Fights.Results.FightLoot();
            this.SpellHistory = new SpellHistory(this);
            this._waitTime = null;
            this.DamageMultiplicator = 1.0;
        }

        // METHODS
		protected virtual void OnReadyStateChanged(bool isReady)
		{
			Action<FightActor, bool> readyStateChanged = this.ReadyStateChanged;
			if (readyStateChanged != null)
			{
				readyStateChanged(this, isReady);
			}
		}
		protected virtual void OnCellShown(Cell cell, bool team)
		{
			Action<FightActor, Cell, bool> cellShown = this.CellShown;
			if (cellShown != null)
			{
				this.CellShown(this, cell, team);
			}
		}
		protected virtual void OnLifePointsChanged(int delta, int permanentDamages, FightActor from)
		{
			Action<FightActor, int, int, FightActor> lifePointsChanged = this.LifePointsChanged;
			if (lifePointsChanged != null)
			{
				lifePointsChanged(this, delta, permanentDamages, from);
			}
		}
		protected virtual void OnBeforeDamageInflicted(Damage damage)
		{
			Action<FightActor, Damage> beforeDamageInflicted = this.BeforeDamageInflicted;
			if (beforeDamageInflicted != null)
			{
				beforeDamageInflicted(this, damage);
			}
		}
		protected virtual void OnDamageInflicted(Damage damage)
		{
			Action<FightActor, Damage> damageInflicted = this.DamageInflicted;
			if (damageInflicted != null)
			{
				damageInflicted(this, damage);
			}
		}
		protected virtual void OnDamageReducted(FightActor source, int reduction)
		{
			Action<FightActor, FightActor, int> damageReducted = this.DamageReducted;
			if (damageReducted != null)
			{
				damageReducted(this, source, reduction);
			}
		}
		protected internal virtual void OnDamageReflected(FightActor target, int reflected)
		{
			ActionsHandler.SendGameActionFightReflectDamagesMessage(this.Fight.Clients, this, target, reflected);
			Action<FightActor, FightActor, int> damageReflected = this.DamageReflected;
			if (damageReflected != null)
			{
				damageReflected(this, target, reflected);
			}
		}
		protected virtual void OnPrePlacementChanged(ObjectPosition position)
		{
			Action<FightActor, ObjectPosition> prePlacementChanged = this.PrePlacementChanged;
			if (prePlacementChanged != null)
			{
				prePlacementChanged(this, position);
			}
		}
        protected virtual void OnTurnStarted()
        {
            System.Action<FightActor> turnStarted = this.TurnStarted;
            if (turnStarted != null)
            {
                turnStarted(this);
            }
        }
		protected virtual void OnTurnPassed()
		{
			System.Action<FightActor> turnPassed = this.TurnPassed;
			if (turnPassed != null)
			{
				turnPassed(this);
			}
		}
		protected virtual void OnSpellCasting(Spell spell, Cell target, FightSpellCastCriticalEnum critical, bool silentCast)
		{
			FightActor.SpellCastingHandler spellCasting = this.SpellCasting;
			if (spellCasting != null)
			{
				spellCasting(this, spell, target, critical, silentCast);
			}
		}
		protected virtual void OnSpellCasted(Spell spell, Cell target, FightSpellCastCriticalEnum critical, bool silentCast)
		{
			if (spell.CurrentSpellLevel.Effects.All((EffectDice effect) => effect.EffectId != EffectsEnum.Effect_Invisibility) && this.VisibleState == GameActionFightInvisibilityStateEnum.INVISIBLE)
			{
				this.ShowCell(base.Cell, false);
				if (!this.IsInvisibleSpellCast(spell) && !this.DispellInvisibilityBuff())
				{
					this.SetInvisibilityState(GameActionFightInvisibilityStateEnum.VISIBLE);
				}
			}
			this.SpellHistory.RegisterCastedSpell(spell.CurrentSpellLevel, this.Fight.GetOneFighter(target));
			FightActor.SpellCastingHandler spellCasted = this.SpellCasted;
			if (spellCasted != null)
			{
				spellCasted(this, spell, target, critical, silentCast);
			}
		}
		protected virtual void OnSpellCastFailed(Spell spell, Cell cell)
		{
			Action<FightActor, Spell, Cell> spellCastFailed = this.SpellCastFailed;
			if (spellCastFailed != null)
			{
				spellCastFailed(this, spell, cell);
			}
		}
		protected virtual void OnWeaponUsed(WeaponTemplate weapon, Cell cell, FightSpellCastCriticalEnum critical, bool silentCast)
		{
			if (this.VisibleState == GameActionFightInvisibilityStateEnum.INVISIBLE)
			{
				this.ShowCell(base.Cell, false);
				if (!this.DispellInvisibilityBuff())
				{
					this.SetInvisibilityState(GameActionFightInvisibilityStateEnum.VISIBLE);
				}
			}
			Action<FightActor, WeaponTemplate, Cell, FightSpellCastCriticalEnum, bool> weaponUsed = this.WeaponUsed;
			if (weaponUsed != null)
			{
				weaponUsed(this, weapon, cell, critical, silentCast);
			}
		}
		protected virtual void OnBuffAdded(Buff buff)
		{
			Action<FightActor, Buff> buffAdded = this.BuffAdded;
			if (buffAdded != null)
			{
				buffAdded(this, buff);
			}
		}
		protected virtual void OnBuffRemoved(Buff buff)
		{
			Action<FightActor, Buff> buffRemoved = this.BuffRemoved;
			if (buffRemoved != null)
			{
				buffRemoved(this, buff);
			}
		}
		protected virtual void OnDead(FightActor killedBy)
		{
			this.RemoveAndDispellAllBuffs();
			Action<FightActor, FightActor> dead = this.Dead;
			if (dead != null)
			{
				dead(this, killedBy);
			}
		}
		protected virtual void OnFightPointsVariation(ActionsEnum action, FightActor source, FightActor target, short delta)
		{
			if (action != ActionsEnum.ACTION_CHARACTER_ACTION_POINTS_USE)
			{
				if (action == ActionsEnum.ACTION_CHARACTER_MOVEMENT_POINTS_USE)
				{
					this.OnMpUsed(Convert.ToInt16(-delta));
				}
			}
			else
			{
				this.OnApUsed(Convert.ToInt16(-delta));
			}
			FightActor.FightPointsVariationHandler fightPointsVariation = this.FightPointsVariation;
			if (fightPointsVariation != null)
			{
				fightPointsVariation(this, action, source, target, delta);
			}
		}
		protected virtual void OnApUsed(short amount)
		{
			Action<FightActor, short> apUsed = this.ApUsed;
			if (apUsed != null)
			{
				apUsed(this, amount);
			}
		}
		protected virtual void OnMpUsed(short amount)
		{
			Action<FightActor, short> mpUsed = this.MpUsed;
			if (mpUsed != null)
			{
				mpUsed(this, amount);
			}
		}
        public override bool StartMove(Path movementPath)
        {
            if(this.IsCarried)
            {
                this.Carrier.DepositCarried(movementPath.SecondCell);
                movementPath = new Path(movementPath.Map, movementPath.GetPath().Skip(1));
            }
            return base.StartMove(movementPath);
        }

        public abstract Spell GetSpell(int id);
		public abstract bool HasSpell(int id);
		public void ToggleReady(bool ready)
		{
			this.IsReady = ready;
			this.OnReadyStateChanged(ready);
		}
		public void ChangePrePlacement(Cell cell)
		{
			if (this.Fight.CanChangePosition(this, cell))
			{
				this.Position.Cell = cell;
				this.OnPrePlacementChanged(this.Position);
			}
		}
		public virtual ObjectPosition GetLeaderBladePosition()
		{
			return this.MapPosition.Clone();
		}
        public void StartTurn()
        {
            if (this.IsFighterTurn())
            {
                if (this.m_bombs.Count > 0)
                {
                    if (this.m_bombs.Any(entry => entry.CanBeBoosted))
                    {
                        this.Fight.StartSequence(SequenceTypeEnum.SEQUENCE_TRIGGERED);
                        this.m_bombs.ForEach(entry => entry.BuffBomb());
                        this.Fight.EndSequence(SequenceTypeEnum.SEQUENCE_TRIGGERED);
                    }
                }

                this.OnTurnStarted();
            }
        }
		public void PassTurn()
		{
			if (this.IsFighterTurn())
			{
				this.Fight.StopTurn();
				this.OnTurnPassed();
			}
		}
        public void StopTurn()
        {
            if (this.IsFighterTurn())
            {
                this.OnTurnPassed();
            }
        }

		public void LeaveFight()
		{
			if (!this.HasLeft())
			{
				this.m_left = true;
				this.OnLeft();
			}
		}
		protected virtual void OnLeft()
		{
			System.Action<FightActor> fighterLeft = this.FighterLeft;
			if (fighterLeft != null)
			{
				fighterLeft(this);
			}
		}
		public void ShowCell(Cell cell, bool team = true)
		{
			if (team)
			{
				using (System.Collections.Generic.IEnumerator<CharacterFighter> enumerator = this.Team.GetAllFighters<CharacterFighter>().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						CharacterFighter current = enumerator.Current;
						ContextHandler.SendShowCellMessage(current.Character.Client, this, cell);
					}
					goto IL_5C;
				}
			}
			ContextHandler.SendShowCellMessage(this.Fight.Clients, this, cell);
			IL_5C:
			this.OnCellShown(cell, team);
		}
		public virtual bool UseAP(short amount)
		{
			bool result;
			if (this.Stats[PlayerFields.AP].Total - (int)amount < 0)
			{
				result = false;
			}
			else
			{
				StatsAP expr_2A = this.Stats.AP;
				expr_2A.Used += amount;
				this.OnFightPointsVariation(ActionsEnum.ACTION_CHARACTER_ACTION_POINTS_USE, this, this, Convert.ToInt16(-amount));
				result = true;
			}
			return result;
		}
		public virtual bool UseMP(short amount)
		{
			bool result;
			if (this.Stats[PlayerFields.MP].Total - (int)amount < 0)
			{
				result = false;
			}
			else
			{
				StatsMP expr_2A = this.Stats.MP;
				expr_2A.Used += amount;
				this.OnFightPointsVariation(ActionsEnum.ACTION_CHARACTER_MOVEMENT_POINTS_USE, this, this, Convert.ToInt16(-amount));
				result = true;
			}
			return result;
		}
		public virtual bool LostAP(short amount)
		{
			bool result;
			if (this.Stats[PlayerFields.AP].Total - (int)amount < 0)
			{
				result = false;
			}
			else
			{
				StatsAP expr_2A = this.Stats.AP;
				expr_2A.Used += amount;
				this.OnFightPointsVariation(ActionsEnum.ACTION_CHARACTER_ACTION_POINTS_LOST, this, this, Convert.ToInt16(-amount));
				result = true;
			}
			return result;
		}
		public virtual bool LostMP(short amount)
		{
			bool result;
			if (this.Stats[PlayerFields.MP].Total - (int)amount < 0)
			{
				result = false;
			}
			else
			{
				StatsMP expr_2A = this.Stats.MP;
				expr_2A.Used += amount;
				this.OnFightPointsVariation(ActionsEnum.ACTION_CHARACTER_MOVEMENT_POINTS_LOST, this, this, Convert.ToInt16(-amount));
				result = true;
			}
			return result;
		}
		public virtual bool RegainAP(short amount)
		{
			StatsAP expr_0B = this.Stats.AP;
			expr_0B.Used -= amount;
			this.OnFightPointsVariation(ActionsEnum.ACTION_CHARACTER_ACTION_POINTS_WIN, this, this, amount);
			return true;
		}
		public virtual bool RegainMP(short amount)
		{
			StatsMP expr_0B = this.Stats.MP;
			expr_0B.Used -= amount;
			this.OnFightPointsVariation(ActionsEnum.ACTION_CHARACTER_MOVEMENT_POINTS_WIN, this, this, amount);
			return true;
		}
		public virtual void ResetUsedPoints()
		{
			this.Stats.AP.Used = 0;
			this.Stats.MP.Used = 0;
		}
		public virtual SpellCastResult CanCastSpell(Spell spell, Cell cell)
		{
			SpellCastResult result;
			if (!this.IsFighterTurn() || this.IsDead())
			{
				result = SpellCastResult.CANNOT_PLAY;
			}
			else
			{
				if (!this.HasSpell(spell.Id))
				{
					result = SpellCastResult.HAS_NOT_SPELL;
				}
				else
				{
					SpellLevelTemplate currentSpellLevel = spell.CurrentSpellLevel;
					if (!cell.Walkable || cell.NonWalkableDuringFight)
					{
						result = SpellCastResult.UNWALKABLE_CELL;
					}
					else
					{
						if (this.AP < currentSpellLevel.ApCost)
						{
							result = SpellCastResult.NOT_ENOUGH_AP;
						}
						else
						{
							bool flag = this.Fight.IsCellFree(cell);
							if ((currentSpellLevel.NeedFreeCell && !flag) || (currentSpellLevel.NeedTakenCell && flag))
							{
								result = SpellCastResult.CELL_NOT_FREE;
							}
							else
							{
								if (currentSpellLevel.StatesForbidden.Any(new Func<int, bool>(this.HasState)))
								{
									result = SpellCastResult.STATE_FORBIDDEN;
								}
								else
								{
									if (currentSpellLevel.StatesRequired.Any((int state) => !this.HasState(state)))
									{
										result = SpellCastResult.STATE_REQUIRED;
									}
									else
									{
										Cell[] castZone = this.GetCastZone(currentSpellLevel);
										if (!castZone.Contains(cell))
										{
											result = SpellCastResult.NOT_IN_ZONE;
										}
										else
										{
											if (!this.SpellHistory.CanCastSpell(currentSpellLevel, cell))
											{
												result = SpellCastResult.HISTORY_ERROR;
											}
											else
											{
												if (spell.CurrentSpellLevel.CastTestLos && !this.Fight.CanBeSeen(base.Cell, cell, false))
												{
													result = SpellCastResult.NO_LOS;
												}
												else
												{
													result = SpellCastResult.OK;
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}
		public virtual Cell[] GetCastZone(SpellLevelTemplate spellLevel)
		{
			uint num = spellLevel.Range;
			if (spellLevel.RangeCanBeBoosted)
			{
				num += (uint)this.Stats[PlayerFields.Range].Total;
				if (num < spellLevel.MinRange)
				{
					num = spellLevel.MinRange;
				}
				num = System.Math.Min(num, 280u);
			}
			IShape shape;
			if (spellLevel.CastInDiagonal && spellLevel.CastInLine)
			{
				shape = new Cross((byte)spellLevel.MinRange, (byte)num)
				{
					AllDirections = true
				};
			}
			else
			{
				if (spellLevel.CastInLine)
				{
					shape = new Cross((byte)spellLevel.MinRange, (byte)num);
				}
				else
				{
					if (spellLevel.CastInDiagonal)
					{
						shape = new Cross((byte)spellLevel.MinRange, (byte)num)
						{
							Diagonal = true
						};
					}
					else
					{
						shape = new Lozenge((byte)spellLevel.MinRange, (byte)num);
					}
				}
			}
			return shape.GetCells(base.Cell, base.Map);
		}
		public int GetSpellRange(SpellLevelTemplate spell)
		{
			return (int)((ulong)spell.Range + (ulong)((long)(spell.RangeCanBeBoosted ? this.Stats[PlayerFields.Range].Total : 0)));
		}
		public virtual bool CastSpell(Spell spell, Cell cell)
		{
			bool result;
			if (!this.IsFighterTurn() || this.IsDead())
			{
				result = false;
			}
			else
			{
				SpellLevelTemplate currentSpellLevel = spell.CurrentSpellLevel;
                if (this.CanCastSpell(spell, cell) != SpellCastResult.OK)
                {
                    this.OnSpellCastFailed(spell, cell);
                    result = false;
                }
                else
                {
                    this.Fight.StartSequence(SequenceTypeEnum.SEQUENCE_SPELL);
                    FightSpellCastCriticalEnum fightSpellCastCriticalEnum = this.RollCriticalDice(currentSpellLevel);

                    SpellCastHandler spellCastHandler = Singleton<SpellManager>.Instance.GetSpellCastHandler(this, spell, cell, fightSpellCastCriticalEnum == FightSpellCastCriticalEnum.CRITICAL_HIT);
                    spellCastHandler.Initialize();
                    this.OnSpellCasting(spell, cell, fightSpellCastCriticalEnum, spellCastHandler.SilentCast);
                    this.UseAP((short)currentSpellLevel.ApCost);
                    spellCastHandler.Execute();
                    this.OnSpellCasted(spell, cell, fightSpellCastCriticalEnum, spellCastHandler.SilentCast);
                    result = true;
                }
			}
			return result;
		}
        public virtual bool ForceCastSpell(Spell spell, Cell cell)
        {
            bool result;
            if (!this.IsFighterTurn() || this.IsDead())
            {
                result = false;
            }
            else
            {
                SpellLevelTemplate currentSpellLevel = spell.CurrentSpellLevel;
                this.Fight.StartSequence(SequenceTypeEnum.SEQUENCE_SPELL);
                FightSpellCastCriticalEnum fightSpellCastCriticalEnum = this.RollCriticalDice(currentSpellLevel);

                SpellCastHandler spellCastHandler = Singleton<SpellManager>.Instance.GetSpellCastHandler(this, spell, cell, fightSpellCastCriticalEnum == FightSpellCastCriticalEnum.CRITICAL_HIT);
                spellCastHandler.Initialize();
                this.OnSpellCasting(spell, cell, fightSpellCastCriticalEnum, spellCastHandler.SilentCast);
                this.UseAP((short)currentSpellLevel.ApCost);
                spellCastHandler.Execute();
                this.OnSpellCasted(spell, cell, fightSpellCastCriticalEnum, spellCastHandler.SilentCast);
                result = true;
            }
            return result;
        }

		public SpellReflectionBuff GetBestReflectionBuff()
		{
			return (
				from entry in this.m_buffList.OfType<SpellReflectionBuff>()
				orderby entry.ReflectedLevel descending
				select entry).FirstOrDefault<SpellReflectionBuff>();
		}
		public void Die()
		{
			this.DamageTaken += this.LifePoints;
			this.OnDead(this);
		}
		public int InflictDirectDamage(int damage, FightActor from)
		{
			return this.InflictDamage(new Damage(damage)
			{
				Source = from,
				School = EffectSchoolEnum.Unknown,
				IgnoreDamageBoost = true,
				IgnoreDamageReduction = true
			});
		}
		public int InflictDirectDamage(int damage)
		{
			return this.InflictDirectDamage(damage, this);
		}
		public virtual int InflictDamage(Damage damage)
		{
            
			FractionGlyph fractionGlyph = this.Fight.GetTriggers().FirstOrDefault((MarkTrigger x) => x is FractionGlyph && x.ContainsCell(base.Cell)) as FractionGlyph;
			int result;
			if (fractionGlyph != null && !(damage.MarkTrigger is FractionGlyph))
			{
				result = fractionGlyph.DispatchDamages(damage);
			}
			else
			{
				this.OnBeforeDamageInflicted(damage);
				this.TriggerBuffs(BuffTriggerType.BEFORE_ATTACKED, damage);
				damage.GenerateDamages();

                if (!damage.IgnoreDamageReduction)
                    damage.Amount = Convert.ToInt32(damage.Amount * DamageMultiplicator);
				if (this.HasState(SpellStatesEnum.Invulnerable))
				{
					this.OnDamageReducted(damage.Source, damage.Amount);
					this.TriggerBuffs(BuffTriggerType.AFTER_ATTACKED, damage);
					result = 0;
				}
				else
				{
					if (damage.Source != null && !damage.IgnoreDamageBoost)
					{
						if (damage.Spell != null)
						{
							damage.Amount += (int)damage.Source.GetSpellBoost(damage.Spell);
						}
						damage.Amount = damage.Source.CalculateDamage(damage.Amount, damage.School);
					}
					int num = this.CalculateErosionDamage(damage.Amount);
					if (!damage.IgnoreDamageReduction)
					{
						damage.Amount = this.CalculateDamageResistance(damage.Amount, damage.School, damage.PvP);
						int num2 = this.CalculateArmorReduction(damage.School);
						if (damage.Amount - num2 < num)
						{
							num2 = damage.Amount - num;
						}
						if (num2 > 0)
						{
							this.OnDamageReducted(damage.Source, num2);
						}
						if (damage.Source != null && !damage.ReflectedDamages)
						{
							int num3 = this.CalculateDamageReflection(damage.Amount);
							if (num3 > 0)
							{
								damage.Source.InflictDirectDamage(num3, this);
								this.OnDamageReflected(damage.Source, num3);
							}
						}
						if (num2 > 0)
						{
							damage.Amount -= num2;
						}
					}
					if (damage.Amount <= 0)
					{
						damage.Amount = 0;
					}
					if (damage.Amount > this.LifePoints)
					{
						damage.Amount = this.LifePoints;
					}
					int num4 = this.CalculateErosionDamage(damage.Amount);
					damage.Amount -= num4;
					this.Stats.Health.DamageTaken += damage.Amount;
					this.Stats.Health.PermanentDamages += num4;
					this.OnLifePointsChanged(-(damage.Amount + num4), num4, damage.Source);
					if (this.IsDead())
					{
						this.OnDead(damage.Source);
					}
					this.OnDamageInflicted(damage);
					this.TriggerBuffs(BuffTriggerType.AFTER_ATTACKED, damage);
					result = damage.Amount;
				}
			}
			return result;
		}
        public virtual void AddContextHealt(int ratio)
        {
            var baseHealt = this.Stats[PlayerFields.Health].Base * ratio;
            this.Stats.Health.Base = baseHealt;
        }
		public virtual int HealDirect(int healPoints, FightActor from)
		{
			int result;
			if (this.HasState((int)SpellStatesEnum.Unhealable))
			{
				this.OnLifePointsChanged(0, 0, from);
				result = 0;
			}
			else
			{
				if (this.LifePoints + healPoints > this.MaxLifePoints)
				{
					healPoints = this.MaxLifePoints - this.LifePoints;
				}
				this.DamageTaken -= healPoints;
				this.OnLifePointsChanged(healPoints, 0, from);
				result = healPoints;
			}
			return result;
		}
		public int Heal(int healPoints, FightActor from, bool withBoost = true)
		{
			if (healPoints < 0)
			{
				healPoints = 0;
			}
			if (withBoost)
			{
				healPoints = from.CalculateHeal(healPoints);
			}
			return this.HealDirect(healPoints, from);
		}
		public void ExchangePositions(FightActor with)
		{
			Cell cell = base.Cell;
			base.Cell = with.Cell;
			with.Cell = cell;
			ActionsHandler.SendGameActionFightExchangePositionsMessage(this.Fight.Clients, this, with);
		}
		public virtual int CalculateDamage(int damage, EffectSchoolEnum type)
		{
			int result;
			switch (type)
			{
			case EffectSchoolEnum.Neutral:
				result = (int)((double)(damage * (100 + this.Stats[PlayerFields.Strength].TotalSafe + this.Stats[PlayerFields.DamageBonusPercent].TotalSafe + this.Stats[PlayerFields.DamageMultiplicator].TotalSafe * 100)) / 100.0 + (double)(this.Stats[PlayerFields.DamageBonus].TotalSafe + this.Stats[PlayerFields.PhysicalDamage].TotalSafe + this.Stats[PlayerFields.NeutralDamageBonus].TotalSafe));
				break;
			case EffectSchoolEnum.Earth:
				result = (int)((double)(damage * (100 + this.Stats[PlayerFields.Strength].TotalSafe + this.Stats[PlayerFields.DamageBonusPercent].TotalSafe + this.Stats[PlayerFields.DamageMultiplicator].TotalSafe * 100)) / 100.0 + (double)(this.Stats[PlayerFields.DamageBonus].TotalSafe + this.Stats[PlayerFields.PhysicalDamage].TotalSafe + this.Stats[PlayerFields.EarthDamageBonus].TotalSafe));
				break;
			case EffectSchoolEnum.Water:
				result = (int)((double)(damage * (100 + this.Stats[PlayerFields.Chance].TotalSafe + this.Stats[PlayerFields.DamageBonusPercent].TotalSafe + this.Stats[PlayerFields.DamageMultiplicator].TotalSafe * 100)) / 100.0 + (double)(this.Stats[PlayerFields.DamageBonus].TotalSafe + this.Stats[PlayerFields.MagicDamage].TotalSafe + this.Stats[PlayerFields.WaterDamageBonus].TotalSafe));
				break;
			case EffectSchoolEnum.Air:
				result = (int)((double)(damage * (100 + this.Stats[PlayerFields.Agility].TotalSafe + this.Stats[PlayerFields.DamageBonusPercent].TotalSafe + this.Stats[PlayerFields.DamageMultiplicator].TotalSafe * 100)) / 100.0 + (double)(this.Stats[PlayerFields.DamageBonus].TotalSafe + this.Stats[PlayerFields.MagicDamage].TotalSafe + this.Stats[PlayerFields.AirDamageBonus].TotalSafe));
				break;
			case EffectSchoolEnum.Fire:
				result = (int)((double)(damage * (100 + this.Stats[PlayerFields.Intelligence].TotalSafe + this.Stats[PlayerFields.DamageBonusPercent].TotalSafe + this.Stats[PlayerFields.DamageMultiplicator].TotalSafe * 100)) / 100.0 + (double)(this.Stats[PlayerFields.DamageBonus].TotalSafe + this.Stats[PlayerFields.MagicDamage].TotalSafe + this.Stats[PlayerFields.FireDamageBonus].TotalSafe));
				break;
			default:
				result = damage;
				break;
			}
			return result;
		}
		public virtual int CalculateDamageResistance(int damage, EffectSchoolEnum type, bool pvp)
		{
			double num;
			double num2;
			int result;
			switch (type)
			{
			case EffectSchoolEnum.Neutral:
				num = (double)(this.Stats[PlayerFields.NeutralResistPercent].Total + (pvp ? this.Stats[PlayerFields.PvpNeutralResistPercent].Total : 0));
				num2 = (double)(this.Stats[PlayerFields.NeutralElementReduction].Total + (pvp ? this.Stats[PlayerFields.PvpNeutralElementReduction].Total : 0) + this.Stats[PlayerFields.PhysicalDamageReduction]);
				break;
			case EffectSchoolEnum.Earth:
				num = (double)(this.Stats[PlayerFields.EarthResistPercent].Total + (pvp ? this.Stats[PlayerFields.PvpEarthResistPercent].Total : 0));
				num2 = (double)(this.Stats[PlayerFields.EarthElementReduction].Total + (pvp ? this.Stats[PlayerFields.PvpEarthElementReduction].Total : 0) + this.Stats[PlayerFields.PhysicalDamageReduction]);
				break;
			case EffectSchoolEnum.Water:
				num = (double)(this.Stats[PlayerFields.WaterResistPercent].Total + (pvp ? this.Stats[PlayerFields.PvpWaterResistPercent].Total : 0));
				num2 = (double)(this.Stats[PlayerFields.WaterElementReduction].Total + (pvp ? this.Stats[PlayerFields.PvpWaterElementReduction].Total : 0) + this.Stats[PlayerFields.MagicDamageReduction]);
				break;
			case EffectSchoolEnum.Air:
				num = (double)(this.Stats[PlayerFields.AirResistPercent].Total + (pvp ? this.Stats[PlayerFields.PvpAirResistPercent].Total : 0));
				num2 = (double)(this.Stats[PlayerFields.AirElementReduction].Total + (pvp ? this.Stats[PlayerFields.PvpAirElementReduction].Total : 0) + this.Stats[PlayerFields.MagicDamageReduction]);
				break;
			case EffectSchoolEnum.Fire:
				num = (double)(this.Stats[PlayerFields.FireResistPercent].Total + (pvp ? this.Stats[PlayerFields.PvpFireResistPercent].Total : 0));
				num2 = (double)(this.Stats[PlayerFields.FireElementReduction].Total + (pvp ? this.Stats[PlayerFields.PvpFireElementReduction].Total : 0) + this.Stats[PlayerFields.MagicDamageReduction]);
				break;
			default:
				result = damage;
				return result;
			}
			result = (int)((1.0 - num / 100.0) * ((double)damage - num2));
			return result;
		}
		public virtual int CalculateDamageReflection(int damage)
		{
			int num = this.Stats[PlayerFields.DamageReflection].Context * (1 + this.Stats[PlayerFields.Wisdom].TotalSafe / 100) + (this.Stats[PlayerFields.DamageReflection].TotalSafe - this.Stats[PlayerFields.DamageReflection].Context);
			int result;
			if ((double)num > (double)damage / 2.0)
			{
				result = (int)((double)damage / 2.0);
			}
			else
			{
				result = num;
			}
			return result;
		}
		public virtual int CalculateHeal(int heal)
		{
			return (int)((double)(heal * (100 + this.Stats[PlayerFields.Intelligence].TotalSafe)) / 100.0 + (double)this.Stats[PlayerFields.HealBonus].TotalSafe);
		}
		public virtual int CalculateArmorValue(int reduction)
		{
			return (int)((double)(reduction * (int)(100 + 5 * this.Level)) / 100.0);
		}
		public virtual int CalculateErosionDamage(int damages)
		{
			int num = this.Stats[PlayerFields.Erosion].TotalSafe;
			if (num > 50)
			{
				num = 50;
			}
			return (int)((double)damages * ((double)num / 100.0));
		}
		public virtual int CalculateArmorReduction(EffectSchoolEnum damageType)
		{
			int totalSafe;
			int result;
			switch (damageType)
			{
			case EffectSchoolEnum.Neutral:
				totalSafe = this.Stats[PlayerFields.NeutralDamageArmor].TotalSafe;
				break;
			case EffectSchoolEnum.Earth:
				totalSafe = this.Stats[PlayerFields.EarthDamageArmor].TotalSafe;
				break;
			case EffectSchoolEnum.Water:
				totalSafe = this.Stats[PlayerFields.WaterDamageArmor].TotalSafe;
				break;
			case EffectSchoolEnum.Air:
				totalSafe = this.Stats[PlayerFields.AirDamageArmor].TotalSafe;
				break;
			case EffectSchoolEnum.Fire:
				totalSafe = this.Stats[PlayerFields.FireDamageArmor].TotalSafe;
				break;
			default:
				result = 0;
				return result;
			}
			result = totalSafe + this.Stats[PlayerFields.GlobalDamageReduction].Total;
			return result;
		}
		public virtual int CalculateCriticRate(uint baseRate)
		{
            var result = (int)baseRate + this.Stats[PlayerFields.CriticalHit].Total;
            if (result < 1)
            {
                result = 1;
            }

            return result;
		}
        public virtual FightSpellCastCriticalEnum RollCriticalDice(SpellLevelTemplate spell)
        {
            AsyncRandom asyncRandom = new AsyncRandom();

            FightSpellCastCriticalEnum result = FightSpellCastCriticalEnum.NORMAL;
            if (spell.CriticalHitProbability != 0u && asyncRandom.Next(100) < this.CalculateCriticRate(spell.CriticalHitProbability))
            {
                result = FightSpellCastCriticalEnum.CRITICAL_HIT;
            }

            return result;
        }
		public virtual int CalculateReflectedDamageBonus(int spellBonus)
		{
			return (int)((double)spellBonus * (1.0 + (double)this.Stats[PlayerFields.Wisdom].TotalSafe / 100.0) + (double)this.Stats[PlayerFields.DamageReflection].TotalSafe);
		}
		public virtual bool RollAPLose(FightActor from)
		{
			int num = (from.Stats[PlayerFields.APAttack].Total > 1) ? from.Stats[PlayerFields.APAttack].TotalSafe : 1;
			int num2 = (this.Stats[PlayerFields.DodgeAPProbability].Total > 1) ? from.Stats[PlayerFields.DodgeAPProbability].TotalSafe : 1;
			double num3 = (double)num / (double)num2 * ((double)this.Stats.AP.TotalMax / (double)(this.Stats.AP.TotalMax - (int)this.Stats.AP.Used) / 2.0);
			if (num3 < 0.1)
			{
				num3 = 0.1;
			}
			else
			{
				if (num3 > 0.9)
				{
					num3 = 0.9;
				}
			}
			double num4 = new AsyncRandom().NextDouble();
			return num4 < num3;
		}
		public virtual bool RollMPLose(FightActor from)
		{
			int num = (from.Stats[PlayerFields.MPAttack].Total > 1) ? from.Stats[PlayerFields.MPAttack].TotalSafe : 1;
			int num2 = (this.Stats[PlayerFields.DodgeMPProbability].Total > 1) ? from.Stats[PlayerFields.DodgeMPProbability].TotalSafe : 1;
			double num3 = (double)num / (double)num2 * ((double)this.Stats.AP.TotalMax / (double)(this.Stats.AP.TotalMax - (int)this.Stats.AP.Used) / 2.0);
			if (num3 < 0.1)
			{
				num3 = 0.1;
			}
			else
			{
				if (num3 > 0.9)
				{
					num3 = 0.9;
				}
			}
			double num4 = new AsyncRandom().NextDouble();
			return num4 < num3;
		}
		public FightActor[] GetTacklers()
		{
			return this.OpposedTeam.GetAllFighters((FightActor entry) => entry.IsAlive() && entry.IsVisibleFor(this) && entry.Position.Point.IsAdjacentTo(this.Position.Point)).ToArray<FightActor>();
		}
		public virtual int GetTackledMP()
		{
			int result;
			if (this.VisibleState != GameActionFightInvisibilityStateEnum.VISIBLE)
			{
				result = 0;
			}
			else
			{
				FightActor[] tacklers = this.GetTacklers();
				if (tacklers.Length <= 0)
				{
					result = 0;
				}
				else
				{
					double num = 0.0;
					for (int i = 0; i < tacklers.Length; i++)
					{
						FightActor tackler = tacklers[i];
						if (i == 0)
						{
							num = this.GetTacklePercent(tackler);
						}
						else
						{
							num *= this.GetTacklePercent(tackler);
						}
					}
					num = 1.0 - num;
					if (num < 0.0)
					{
						num = 0.0;
					}
					else
					{
						if (num > 1.0)
						{
							num = 1.0;
						}
					}
					result = (int)System.Math.Ceiling((double)this.MP * num);
				}
			}
			return result;
		}
		public virtual int GetTackledAP()
		{
			int result;
			if (this.VisibleState != GameActionFightInvisibilityStateEnum.VISIBLE)
			{
				result = 0;
			}
			else
			{
				FightActor[] tacklers = this.GetTacklers();
				if (tacklers.Length <= 0)
				{
					result = 0;
				}
				else
				{
					double num = 0.0;
					for (int i = 0; i < tacklers.Length; i++)
					{
						FightActor tackler = tacklers[i];
						if (i == 0)
						{
							num = this.GetTacklePercent(tackler);
						}
						else
						{
							num *= this.GetTacklePercent(tackler);
						}
					}
					num = 1.0 - num;
					if (num < 0.0)
					{
						num = 0.0;
					}
					else
					{
						if (num > 1.0)
						{
							num = 1.0;
						}
					}
					result = (int)System.Math.Ceiling((double)this.AP * num);
				}
			}
			return result;
		}
		private double GetTacklePercent(FightActor tackler)
		{
			double result;
			if (tackler.Stats[PlayerFields.TackleBlock].Total == -2)
			{
				result = 0.0;
			}
			else
			{
				result = (double)(this.Stats[PlayerFields.TackleEvade].Total + 2) / (2.0 * (double)(tackler.Stats[PlayerFields.TackleBlock].Total + 2));
			}
			return result;
		}
		public int PopNextBuffId()
		{
			return this.m_buffIdProvider.Pop();
		}
		public void FreeBuffId(int id)
		{
			this.m_buffIdProvider.Push(id);
		}
		public System.Collections.Generic.IEnumerable<Buff> GetBuffs()
		{
			return this.m_buffList;
		}
		public System.Collections.Generic.IEnumerable<Buff> GetBuffs(System.Predicate<Buff> predicate)
		{
			return 
				from entry in this.m_buffList
				where predicate(entry)
				select entry;
		}
		public bool BuffMaxStackReached(Buff buff)
		{
			return buff.Spell.CurrentSpellLevel.MaxStack > 0 && buff.Spell.CurrentSpellLevel.MaxStack <= this.m_buffList.Count((Buff entry) => entry.Spell == buff.Spell && entry.Effect.EffectId == buff.Effect.EffectId);
		}
		public bool AddAndApplyBuff(Buff buff, bool freeIdIfFail = true)
		{
			bool result;
			if (this.BuffMaxStackReached(buff))
			{
				if (freeIdIfFail)
				{
					this.FreeBuffId(buff.Id);
				}
				result = false;
			}
			else
			{
				this.AddBuff(buff, true);
				if (!(buff is TriggerBuff) || ((buff as TriggerBuff).Trigger & BuffTriggerType.BUFF_ADDED) == BuffTriggerType.BUFF_ADDED)
				{
					buff.Apply();
				}
				result = true;
			}
			return result;
		}
		public bool AddBuff(Buff buff, bool freeIdIfFail = true)
		{
			bool result;
			if (this.BuffMaxStackReached(buff))
			{
				if (freeIdIfFail)
				{
					this.FreeBuffId(buff.Id);
				}
				result = false;
			}
			else
			{
				this.m_buffList.Add(buff);
				this.OnBuffAdded(buff);
				result = true;
			}
			return result;
		}
		public void RemoveAndDispellBuff(Buff buff)
		{
			this.RemoveBuff(buff);
			buff.Dispell();
		}
		public void RemoveBuff(Buff buff)
		{
			this.m_buffList.Remove(buff);
			this.OnBuffRemoved(buff);
			this.FreeBuffId(buff.Id);
		}
		public void RemoveAndDispellAllBuffs()
		{
			Buff[] array = this.m_buffList.ToArray();
			Buff[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				Buff buff = array2[i];
				this.RemoveAndDispellBuff(buff);
			}
		}
		public void RemoveAndDispellAllBuffs(FightActor caster)
		{
			Buff[] array = this.m_buffList.ToArray();
			Buff[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				Buff buff = array2[i];
				if (buff.Caster == caster)
				{
					this.RemoveAndDispellBuff(buff);
				}
			}
		}
		public void RemoveAllCastedBuffs()
		{
			foreach (FightActor current in this.Fight.GetAllFighters())
			{
				current.RemoveAndDispellAllBuffs(this);
			}
		}
		public void TriggerBuffs(BuffTriggerType trigger, object token = null)
		{
			Buff[] source = this.m_buffList.ToArray();
			foreach (TriggerBuff current in 
				from triggerBuff in source.OfType<TriggerBuff>()
				where (triggerBuff.Trigger & trigger) == trigger
                where triggerBuff.IsReady() // added line
				select triggerBuff)
			{
				this.Fight.StartSequence(SequenceTypeEnum.SEQUENCE_TRIGGERED);
				current.Apply(trigger, token);
				this.Fight.EndSequence(SequenceTypeEnum.SEQUENCE_TRIGGERED, false);
			}
		}
		public void DecrementBuffsDuration(FightActor caster)
		{
			System.Collections.Generic.List<Buff> list = (
				from buff in this.m_buffList
				where buff.Caster == caster
                where buff.Duration != -1
				where buff.DecrementDuration()
				select buff).ToList<Buff>();

			foreach (Buff current in list)
			{
				if (current is TriggerBuff && (current as TriggerBuff).Trigger.HasFlag(BuffTriggerType.BUFF_ENDED))
				{
					(current as TriggerBuff).Apply(BuffTriggerType.BUFF_ENDED);
				}
				if (!(current is TriggerBuff) || !(current as TriggerBuff).Trigger.HasFlag(BuffTriggerType.BUFF_ENDED_TURNEND))
				{
					this.RemoveAndDispellBuff(current);
				}
			}
		}
		public void TriggerBuffsRemovedOnTurnEnd()
		{
			Buff[] array = (
				from entry in this.m_buffList
				where entry.Duration <= 0 && entry is TriggerBuff && (entry as TriggerBuff).Trigger.HasFlag(BuffTriggerType.BUFF_ENDED_TURNEND)
				select entry).ToArray<Buff>();
			for (int i = 0; i < array.Length; i++)
			{
				Buff buff = array[i];
				buff.Apply();
				this.RemoveAndDispellBuff(buff);
			}
		}
		public void DecrementAllCastedBuffsDuration()
		{
			foreach (FightActor current in this.Fight.GetAllFighters())
			{
				current.DecrementBuffsDuration(this);
			}
		}
		public void BuffSpell(Spell spell, short boost)
		{
			if (!this.m_buffedSpells.ContainsKey(spell))
			{
				this.m_buffedSpells.Add(spell, boost);
			}
			else
			{
				System.Collections.Generic.Dictionary<Spell, short> buffedSpells;
				(buffedSpells = this.m_buffedSpells)[spell] =  Convert.ToInt16(buffedSpells[spell] + boost);
			}
		}
		public void UnBuffSpell(Spell spell, short boost)
		{
			if (this.m_buffedSpells.ContainsKey(spell))
			{
				System.Collections.Generic.Dictionary<Spell, short> buffedSpells;
				(buffedSpells = this.m_buffedSpells)[spell] =  Convert.ToInt16(buffedSpells[spell] - boost);
				if (this.m_buffedSpells[spell] == 0)
				{
					this.m_buffedSpells.Remove(spell);
				}
			}
		}
		public short GetSpellBoost(Spell spell)
		{
			short result;
			if (!this.m_buffedSpells.ContainsKey(spell))
			{
				result = 0;
			}
			else
			{
				result = this.m_buffedSpells[spell];
			}
			return result;
		}
		public bool MustSkipTurn()
		{
			return this.GetBuffs((Buff x) => x is SkipTurnBuff).Any<Buff>();
		}
		public System.Collections.ObjectModel.ReadOnlyCollection<SummonedFighter> GetSummons()
		{
			return this.m_summons.AsReadOnly();
		}

		public bool CanSummon()
		{
			return this.SummonedCount < this.Stats[PlayerFields.SummonLimit].Total;
		}
        public void AddSummon(SummonedFighter summon)
        {
            var monsterSummmon = summon as SummonedMonster;
            if (monsterSummmon != null && monsterSummmon.Monster.Template.UseSummonSlot && !monsterSummmon.IsTree)
            {
                this.SummonedCount++;
            }
            this.m_summons.Add(summon);
        }
		public void RemoveSummon(SummonedFighter summon)
		{
			if (summon is SummonedMonster && (summon as SummonedMonster).Monster.Template.UseSummonSlot)
			{
				this.SummonedCount--;
			}
			this.m_summons.Remove(summon);
		}
		public void RemoveAllSummons()
		{
			this.m_summons.Clear();
		}
		public void KillAllSummons()
		{
			SummonedFighter[] array = this.m_summons.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				SummonedFighter summonedFighter = array[i];
				summonedFighter.Die();
			}
		}

        //ROUBLARD
        public bool CanPutBomb()
        {
            return this.m_bombs.Count < 3;
        }
        public void AddBomb(BombFighter bomb)
        {
            this.m_bombs.Add(bomb);
        }
        public void RemoveBomb(BombFighter bomb)
        {
            this.m_bombs.Remove(bomb);
        }
        public void RemoveAllBombs()
        {
            this.m_bombs.Clear();
        }
        public void KillAllBombs()
        {
            BombFighter[] array = this.m_bombs.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                BombFighter bombFighter = array[i];
                bombFighter.Die();
            }
        }
        public IEnumerable<BombFighter> BombsOfType(Database.Monsters.MonsterTemplate monsterTemplate)
        {
            return this.m_bombs.Where(entry => entry.Monster.Template == monsterTemplate);
        }

        //PANDAWA
        public void Carry(FightActor target)
        {
            this.Carrier = this;
            this.Carried = target;

            target.Carrier = this;
            target.Carried = target;
            target.Cell = this.Cell;
        }
        public void Throw(Cell cell)
        {
            Carried.Cell = cell;
            Carried.Carrier = null;
            Carried.Carried = null;

            this.Carrier = null;
            this.Carried = null;
        }
        public void DepositCarried(Cell cell)
        {
            var buff = this.GetBuffs().Where((x) => x.Spell.Id == (int)SpellIdEnum.Karcham).First();
            this.Carrier.RemoveAndDispellBuff(buff);
            this.Carried.RemoveAndDispellBuff(buff);

            this.Fight.Clients.Send(new GameActionFightThrowCharacterMessage((ushort)ActionsEnum.ACTION_THROW_CARRIED_CHARACTER, this.Carrier.Id, this.Carried.Id, cell.Id));
            this.Carried.Cell = cell;

            this.Carried.Carrier = null;
            this.Carried.Carried = null;
            this.Carrier.Carried = null;
            this.Carrier.Carrier = null;
        }

        //SADIDA
        public void SpawnTreeAfterSummonDeath(Cell cell)
        {
            var spell = this.GetSpell((int)SpellIdEnum.Tree);
            var monsterGrade = Singleton<MonsterManager>.Instance.GetMonsterGrade((int)MonsterEnum.SADIDA_TREE, spell.CurrentLevel);
            var summon = new SummonedMonster(Fight.GetNextContextualId(), this.Team, this, monsterGrade, cell, false);
            summon.Stats.Health.Base /= TreeHandler.LIFE_RATIO;

            this.AddSummon(summon);
            this.Team.AddFighter(summon);

            ActionsHandler.SendGameActionFightSummonMessage(this.Fight.Clients, summon);

            var id = summon.PopNextBuffId();
            var effect = new EffectDice() { Duration = TreeHandler.DURATION }; //TODO : Add EffectId
            var actionId = (short)ActionsEnum.ACTION_793;
            var buff = new TriggerBuff(id, summon, summon, effect, spell, false, true, BuffTriggerType.TURN_END, new TriggerBuffApplyHandler(TreeHandler.TreeTrigger), actionId);

            summon.AddAndApplyBuff(buff);
            ContextHandler.SendGameActionFightDispellableEffectMessage(this.Fight.Clients, buff);
        }

	    public void AddState(SpellState state)
		{
			this.m_states.Add(state);
		}
		public void RemoveState(SpellState state)
		{
			this.m_states.Remove(state);
		}
		public bool HasState(int stateId)
		{
			return this.m_states.Any((SpellState entry) => entry.Id == stateId);
		}
		public bool HasState(SpellState state)
		{
			return this.HasState(state.Id);
		}
        public bool HasState(SpellStatesEnum state)
        {
            return this.HasState((int)state);
        }
		public bool HasSpellBlockerState()
		{
			return this.m_states.Any((SpellState entry) => entry.PreventsSpellCast);
		}
		public bool HasFightBlockerState()
		{
			return this.m_states.Any((SpellState entry) => entry.PreventsFight);
		}
        public void AddTelefragState(FightActor source, Spell spell)
        {
            var id = this.PopNextBuffId();
            var effect = new EffectBase { Duration = 1 };
            var stateId = (uint)SpellStatesEnum.Telefrag;
            var state = Singleton<SpellManager>.Instance.GetSpellState(stateId);

            StateBuff casterBuff = new StateBuff(id, this , source, effect, spell, false, state);
            this.AddAndApplyBuff(casterBuff);
        }
        public void RemoveTelefragState()
        {
            var buff = this.GetBuffs().Where((x) => x.Id == (int)SpellStatesEnum.Telefrag).FirstOrDefault();
            if (buff != null)
                this.RemoveAndDispellBuff(buff);
        }
		public void SetInvisibilityState(GameActionFightInvisibilityStateEnum state)
		{
			GameActionFightInvisibilityStateEnum visibleState = this.VisibleState;
			this.VisibleState = state;
			this.OnVisibleStateChanged(this, visibleState);
		}
		public void SetInvisibilityState(GameActionFightInvisibilityStateEnum state, FightActor source)
		{
			GameActionFightInvisibilityStateEnum visibleState = this.VisibleState;
			this.VisibleState = state;
			this.OnVisibleStateChanged(source, visibleState);
		}
		public bool IsInvisibleSpellCast(Spell spell)
		{
			SpellLevelTemplate currentSpellLevel = spell.CurrentSpellLevel;
			bool result;
			if (!(this is CharacterFighter))
			{
				result = true;
			}
			else
			{
				bool arg_AC_0;
				if (!currentSpellLevel.Effects.Any((EffectDice entry) => entry.EffectId == EffectsEnum.Effect_Trap))
				{
					if (!currentSpellLevel.Effects.Any((EffectDice entry) => entry.EffectId == EffectsEnum.Effect_Summon) && spell.Template.Id != 74 && spell.Template.Id != 62 && spell.Template.Id != 66)
					{
						arg_AC_0 = (spell.Template.Id == 67);
						goto IL_AC;
					}
				}
				arg_AC_0 = true;
				IL_AC:
				result = arg_AC_0;
			}
			return result;
		}
		public bool DispellInvisibilityBuff()
		{
			Buff[] array = this.GetBuffs((Buff entry) => entry is InvisibilityBuff).ToArray<Buff>();
			Buff[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				Buff buff = array2[i];
				this.RemoveAndDispellBuff(buff);
			}
			return array.Any<Buff>();
		}
		public GameActionFightInvisibilityStateEnum GetVisibleStateFor(FightActor fighter)
		{
			int arg_1D_0;
			if (fighter.IsFriendlyWith(this))
			{
				if (this.VisibleState != GameActionFightInvisibilityStateEnum.VISIBLE)
				{
					arg_1D_0 = 2;
					return (GameActionFightInvisibilityStateEnum)arg_1D_0;
				}
			}
			arg_1D_0 = (int)this.VisibleState;
			return (GameActionFightInvisibilityStateEnum)arg_1D_0;
		}
		public GameActionFightInvisibilityStateEnum GetVisibleStateFor(Character character)
		{
			GameActionFightInvisibilityStateEnum result;
			if (!character.IsFighting() || character.Fight != this.Fight)
			{
				result = this.VisibleState;
			}
			else
			{
				GameActionFightInvisibilityStateEnum arg_46_0;
				if (character.Fighter.IsFriendlyWith(this))
				{
					if (this.VisibleState != GameActionFightInvisibilityStateEnum.VISIBLE)
					{
						arg_46_0 = GameActionFightInvisibilityStateEnum.DETECTED;
						goto IL_46;
					}
				}
				arg_46_0 = this.VisibleState;
				IL_46:
				result = arg_46_0;
			}
			return result;
		}
		public bool IsVisibleFor(FightActor fighter)
		{
			return this.GetVisibleStateFor(fighter) != GameActionFightInvisibilityStateEnum.INVISIBLE;
		}
		public bool IsVisibleFor(Character character)
		{
			return this.GetVisibleStateFor(character) != GameActionFightInvisibilityStateEnum.INVISIBLE;
		}
		protected virtual void OnVisibleStateChanged(FightActor source, GameActionFightInvisibilityStateEnum lastState)
		{
			this.Fight.ForEach(delegate(Character entry)
			{
				ActionsHandler.SendGameActionFightInvisibilityMessage(entry.Client, source, this, this.GetVisibleStateFor(entry));
			}, true);
			if (lastState == GameActionFightInvisibilityStateEnum.INVISIBLE)
			{
				this.Fight.ForEach(delegate(Character entry)
				{
					ContextHandler.SendGameFightRefreshFighterMessage(entry.Client, this);
				});
			}
		}
		public virtual void ResetFightProperties()
		{
			this.ResetUsedPoints();
			this.RemoveAndDispellAllBuffs();
			foreach (System.Collections.Generic.KeyValuePair<PlayerFields, StatsData> current in this.Stats.Fields)
			{
				current.Value.Context = 0;
			}
			this.Stats.Health.PermanentDamages = 0;
		}
		public virtual System.Collections.Generic.IEnumerable<DroppedItem> RollLoot(IFightResult looter, int challengeBonus)
		{
			return new DroppedItem[0];
		}
		public virtual uint GetDroppedKamas()
		{
			return 0u;
		}
		public virtual int GetGivenExperience()
		{
			return 0;
		}
		public virtual IFightResult GetFightResult()
		{
			return new FightResult(this, this.GetFighterOutcome(), this.Loot);
		}
		protected FightOutcomeEnum GetFighterOutcome()
		{
			bool flag = this.Team.AreAllDead();
			bool flag2 = this.OpposedTeam.AreAllDead();
			FightOutcomeEnum result;
			if (!flag && flag2)
			{
				result = FightOutcomeEnum.RESULT_VICTORY;
			}
			else
			{
				if (flag && !flag2)
				{
					result = FightOutcomeEnum.RESULT_LOST;
				}
				else
				{
					result = FightOutcomeEnum.RESULT_DRAW;
				}
			}
			return result;
		}
		public bool IsAlive()
		{
			return this.Stats.Health.Total > 0 && !this.HasLeft();
		}
		public bool IsDead()
		{
			return !this.IsAlive();
		}
		public bool HasLeft()
		{
			return this.m_left;
		}
		public bool HasLost()
		{
			return this.Fight.Losers == this.Team;
		}
		public bool HasWin()
		{
			return this.Fight.Winners == this.Team;
		}
		public bool IsTeamLeader()
		{
			return this.Team.Leader == this;
		}
		public bool IsFighterTurn()
		{
			return this.Fight.TimeLine.Current == this;
		}
		public bool IsFriendlyWith(FightActor actor)
		{
			return actor.Team == this.Team;
		}
		public bool IsEnnemyWith(FightActor actor)
		{
			return !this.IsFriendlyWith(actor);
		}
		public override bool CanMove()
		{
			return this.IsFighterTurn() && this.IsAlive() && this.MP > 0;
		}
        public virtual bool CanBeMove()
        {
            var immovabliesStates = this.m_states.Where((x) => x.Id == (int)SpellStatesEnum.Leafy || x.Id == (int)SpellStatesEnum.Rooted).SingleOrDefault(); //need to make a better method
            return (immovabliesStates == null);
        }
		public virtual bool CanPlay()
		{
			return this.IsAlive() && !this.HasLeft();
		}
		public override bool CanSee(WorldObject obj)
		{
			return base.CanSee(obj);
		}
		public override bool CanBeSee(WorldObject obj)
		{
			FightActor fightActor = obj as FightActor;
			Character character = obj as Character;
			if (character != null && character.IsFighting())
			{
				fightActor = character.Fighter;
			}
			bool result;
			if (fightActor == null || fightActor.Fight != this.Fight)
			{
				result = (base.CanBeSee(obj) && this.VisibleState != GameActionFightInvisibilityStateEnum.INVISIBLE);
			}
			else
			{
				result = (this.GetVisibleStateFor(fightActor) != GameActionFightInvisibilityStateEnum.INVISIBLE && this.IsAlive());
			}
			return result;
		}
		public override EntityDispositionInformations GetEntityDispositionInformations()
		{
			return this.GetEntityDispositionInformations(null);
		}
		public virtual EntityDispositionInformations GetEntityDispositionInformations(WorldClient client = null)
		{
			return new FightEntityDispositionInformations(Convert.ToInt16((client != null) ? (this.IsVisibleFor(client.Character) ? base.Cell.Id : -1) : base.Cell.Id), (sbyte)base.Direction, (this.Carried != null) ? this.Carried.Id : 0);
		}
		public virtual GameFightMinimalStats GetGameFightMinimalStats()
		{
			return this.GetGameFightMinimalStats(null);
		}
		public virtual GameFightMinimalStats GetGameFightMinimalStats(WorldClient client = null)
		{
            return new GameFightMinimalStats((uint)this.Stats.Health.Total, (uint)this.Stats.Health.TotalMax, (uint)this.Stats.Health.Base, (uint)this.Stats[PlayerFields.PermanentDamagePercent].Total, (uint)0, (short)this.Stats.AP.Total, (short)this.Stats.AP.TotalMax, (short)this.Stats.MP.Total, (short)this.Stats.MP.TotalMax, 0, false, (short)this.Stats[PlayerFields.NeutralResistPercent].Total, (short)this.Stats[PlayerFields.EarthResistPercent].Total, (short)this.Stats[PlayerFields.WaterResistPercent].Total, (short)this.Stats[PlayerFields.AirResistPercent].Total, (short)this.Stats[PlayerFields.FireResistPercent].Total, (short)this.Stats[PlayerFields.NeutralElementReduction].Total, (short)this.Stats[PlayerFields.EarthElementReduction].Total, (short)this.Stats[PlayerFields.WaterElementReduction].Total, (short)this.Stats[PlayerFields.AirElementReduction].Total, (short)this.Stats[PlayerFields.FireElementReduction].Total, (short)this.Stats[PlayerFields.PushDamageReduction].Total, (short)this.Stats[PlayerFields.CriticalDamageReduction].Total, (ushort)this.Stats[PlayerFields.DodgeAPProbability].Total, (ushort)this.Stats[PlayerFields.DodgeMPProbability].Total, (short)this.Stats[PlayerFields.TackleBlock].Total, (short)this.Stats[PlayerFields.TackleEvade].Total, (sbyte)((client == null) ? this.VisibleState : this.GetVisibleStateFor(client.Character)));
		}
		public virtual FightTeamMemberInformations GetFightTeamMemberInformations()
		{
			return new FightTeamMemberInformations(this.Id);
		}
		public virtual GameFightFighterInformations GetGameFightFighterInformations()
		{
			return this.GetGameFightFighterInformations(null);
		}
		public virtual GameFightFighterInformations GetGameFightFighterInformations(WorldClient client = null)
		{
			return new GameFightFighterInformations(this.Id, this.Look.GetEntityLook(), this.GetEntityDispositionInformations(client), this.Team.Id, 0, this.IsAlive(), this.GetGameFightMinimalStats(client), Enumerable.Empty<ushort>());
		}
		public override GameContextActorInformations GetGameContextActorInformations(Character character)
		{
			return this.GetGameFightFighterInformations();
		}

        public abstract GameFightFighterLightInformations GetGameFightFighterLightInformations();

		public abstract string GetMapRunningFighterName();

        public uint GetWaitTime()
        {
            if (this._waitTime == null)
            {
                this._waitTime = (uint)Fights.Fight.TurnTime;
            }

            return this._waitTime.Value;
        }

        public virtual void RefreshWaitTime(uint remainingDurationSeconds) { }
        public virtual void RefreshFighterStatsListMessage() { }

        public virtual CharacterCharacteristicsInformations GetSlaveStats(FightActor master)
        {
            return new CharacterCharacteristicsInformations(0,
                  0,
                  0,
                  0,
                  0,
                  0,
                  0,
                  new ActorExtendedAlignmentInformations(),
                  (uint)Stats.Health.Total,
                  (uint)Stats.Health.TotalMax,
                  0,
                  0,
                  (short)Stats[PlayerFields.AP].Total,
                  (short)Stats[PlayerFields.MP].Total,
                  Stats[PlayerFields.Initiative],
                  Stats[PlayerFields.Prospecting],
                  Stats[PlayerFields.AP],
                  Stats[PlayerFields.MP],
                  Stats[PlayerFields.Strength],
                  Stats[PlayerFields.Vitality],
                  Stats[PlayerFields.Wisdom],
                  Stats[PlayerFields.Chance],
                  Stats[PlayerFields.Agility],
                  Stats[PlayerFields.Intelligence],
                  Stats[PlayerFields.Range],
                  Stats[PlayerFields.SummonLimit],
                  Stats[PlayerFields.DamageReflection],
                  Stats[PlayerFields.CriticalHit],
                  (ushort)0,
                  Stats[PlayerFields.CriticalMiss],
                  Stats[PlayerFields.HealBonus],
                  Stats[PlayerFields.DamageBonus],
                  Stats[PlayerFields.WeaponDamageBonus],
                  Stats[PlayerFields.DamageBonusPercent],
                  Stats[PlayerFields.TrapBonus],
                  Stats[PlayerFields.TrapBonusPercent],
                  Stats[PlayerFields.GlyphBonusPercent],
                  Stats[PlayerFields.PermanentDamagePercent],
                  Stats[PlayerFields.TackleBlock],
                  Stats[PlayerFields.TackleEvade],
                  Stats[PlayerFields.APAttack],
                  Stats[PlayerFields.MPAttack],
                  Stats[PlayerFields.PushDamageBonus],
                  Stats[PlayerFields.CriticalDamageBonus],
                  Stats[PlayerFields.NeutralDamageBonus],
                  Stats[PlayerFields.EarthDamageBonus],
                  Stats[PlayerFields.WaterDamageBonus],
                  Stats[PlayerFields.AirDamageBonus],
                  Stats[PlayerFields.FireDamageBonus],
                  Stats[PlayerFields.DodgeAPProbability],
                  Stats[PlayerFields.DodgeMPProbability],
                  Stats[PlayerFields.NeutralResistPercent],
                  Stats[PlayerFields.EarthResistPercent],
                  Stats[PlayerFields.WaterResistPercent],
                  Stats[PlayerFields.AirResistPercent],
                  Stats[PlayerFields.FireResistPercent],
                  Stats[PlayerFields.NeutralElementReduction],
                  Stats[PlayerFields.EarthElementReduction],
                  Stats[PlayerFields.WaterElementReduction],
                  Stats[PlayerFields.AirElementReduction],
                  Stats[PlayerFields.FireElementReduction],
                  Stats[PlayerFields.PushDamageReduction],
                  Stats[PlayerFields.CriticalDamageReduction],
                  Stats[PlayerFields.PvpNeutralResistPercent],
                  Stats[PlayerFields.PvpEarthResistPercent],
                  Stats[PlayerFields.PvpWaterResistPercent],
                  Stats[PlayerFields.PvpAirResistPercent],
                  Stats[PlayerFields.PvpFireResistPercent],
                  Stats[PlayerFields.PvpNeutralElementReduction],
                  Stats[PlayerFields.PvpEarthElementReduction],
                  Stats[PlayerFields.PvpWaterElementReduction],
                  Stats[PlayerFields.PvpAirElementReduction],
                  Stats[PlayerFields.PvpFireElementReduction],
                  new List<CharacterSpellModification>(), 0);
        }
    }
}
