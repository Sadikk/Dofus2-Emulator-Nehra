using Stump.Core.Attributes;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Actors.Interfaces;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Fights;
using Stump.Server.WorldServer.Game.Fights.Teams;
using Stump.Server.WorldServer.Game.Maps.Cells;
using Stump.Server.WorldServer.Game.Maps.Pathfinding;
using Stump.Server.WorldServer.Handlers.Context;
using System;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Actors.RolePlay.Monsters
{
	public sealed class MonsterGroup : RolePlayActor, IAutoMovedEntity, IContextDependant
	{
		public const short ClientStarsBonusLimit = 200;
		[Variable(true)]
		public static int StarsBonusInterval = 300;
		[Variable(true)]
		public static short StarsBonusIncrementation = 2;
		[Variable(true)]
		public static short StarsBonusLimit = 300;
		private readonly System.Collections.Generic.List<Monster> m_monsters = new System.Collections.Generic.List<Monster>();
		public event Action<MonsterGroup, Character> EnterFight;
        public Fights.Fight Fight
		{
			get;
			private set;
		}
		public override int Id
		{
			get
			{
				return this.ContextualId;
			}
			protected set
			{
				this.ContextualId = value;
			}
		}
		public int ContextualId
		{
			get;
			private set;
		}
		public Monster Leader
		{
			get;
			private set;
		}
		public short AgeBonus
		{
			get
			{
				double num = (System.DateTime.Now - this.CreationDate).TotalSeconds / ((double)MonsterGroup.StarsBonusInterval / (double)MonsterGroup.StarsBonusIncrementation);
				if (num > (double)MonsterGroup.StarsBonusLimit)
				{
					num = (double)MonsterGroup.StarsBonusLimit;
				}
				return (short)num;
			}
		}
		public System.DateTime NextMoveDate
		{
			get;
			set;
		}
		public System.DateTime LastMoveDate
		{
			get;
			private set;
		}
		public System.DateTime CreationDate
		{
			get;
			private set;
		}
		public MonsterGroup(int id, ObjectPosition position)
		{
			this.ContextualId = id;
			this.Position = position;
			this.CreationDate = System.DateTime.Now;
		}
		public override bool CanMove()
		{
			return true;
		}
		public override bool IsMoving()
		{
			return false;
		}
		public override bool StartMove(Path movementPath)
		{
			bool result;
			if (!this.CanMove() || movementPath.IsEmpty())
			{
				result = false;
			}
			else
			{
				this.Position = movementPath.EndPathPosition;
				System.Collections.Generic.IEnumerable<short> keys = movementPath.GetServerPathKeys();
				base.Map.ForEach(delegate(Character entry)
				{
					ContextHandler.SendGameMapMovementMessage(entry.Client, keys, this);
				});
				this.StopMove();
				this.LastMoveDate = System.DateTime.Now;
				result = true;
			}
			return result;
		}
		public override bool StopMove()
		{
			return false;
		}
		public override bool StopMove(ObjectPosition currentObjectPosition)
		{
			return false;
		}
		public override bool MoveInstant(ObjectPosition destination)
		{
			return false;
		}
		public override bool Teleport(ObjectPosition destination, bool performCheck)
		{
			return false;
		}
		public void FightWith(Character character)
		{
			if (!(character.Map != base.Map))
			{
				base.Map.Leave(this);
				if (base.Map.GetBlueFightPlacement().Length < this.m_monsters.Count)
				{
					character.SendServerMessage("Cannot start fight : Not enough fight placements");
				}
				else
				{
                    Fights.Fight fight = Singleton<FightManager>.Instance.CreatePvMFight(base.Map);
					fight.RedTeam.AddFighter(character.CreateFighter(fight.RedTeam));
					foreach (MonsterFighter current in this.CreateFighters(fight.BlueTeam))
					{
						fight.BlueTeam.AddFighter(current);
					}
					this.Fight = fight;
					fight.StartPlacement();
					this.OnEnterFight(character);
				}
			}
		}
		private void OnEnterFight(Character character)
		{
			Action<MonsterGroup, Character> enterFight = this.EnterFight;
			if (enterFight != null)
			{
				this.EnterFight(this, character);
			}
		}
		public System.Collections.Generic.IEnumerable<MonsterFighter> CreateFighters(FightTeam team)
		{
			return 
				from monster in this.m_monsters
				select monster.CreateFighter(team);
		}
		public void AddMonster(Monster monster)
		{
			this.m_monsters.Add(monster);
			if (this.m_monsters.Count == 1)
			{
				this.Leader = monster;
			}
			base.Map.Refresh(this);
		}
		public void RemoveMonster(Monster monster)
		{
			this.m_monsters.Remove(monster);
			if (this.m_monsters.Count == 0)
			{
				this.Leader = null;
			}
			base.Map.Refresh(this);
		}
		public System.Collections.Generic.IEnumerable<Monster> GetMonsters()
		{
			return this.m_monsters;
		}
		public System.Collections.Generic.IEnumerable<Monster> GetMonstersWithoutLeader()
		{
			return 
				from entry in this.m_monsters
				where entry != this.Leader
				select entry;
		}
		public int Count()
		{
			return this.m_monsters.Count;
		}
		public override GameContextActorInformations GetGameContextActorInformations(Character character)
		{
			return new GameRolePlayGroupMonsterInformations(this.Id, this.Leader.Look.GetEntityLook(), this.GetEntityDispositionInformations(), false, false, false, this.GetGroupMonsterStaticInformations(), Convert.ToInt16((this.AgeBonus > 200) ? 200 : this.AgeBonus), 0, -1);
		}
		public GroupMonsterStaticInformations GetGroupMonsterStaticInformations()
		{
			return new GroupMonsterStaticInformations(this.Leader.GetMonsterInGroupLightInformations(), 
				from entry in this.GetMonstersWithoutLeader()
				select entry.GetMonsterInGroupInformations());
		}
		protected override void OnDisposed()
		{
			base.OnDisposed();
		}
		public override string ToString()
		{
			return string.Format("{0} monsters ({1})", this.m_monsters.Count, this.Id);
		}
	}
}
