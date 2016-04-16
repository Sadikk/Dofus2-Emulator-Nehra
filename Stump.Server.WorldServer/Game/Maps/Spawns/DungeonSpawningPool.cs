using NLog;
using Stump.Core.Attributes;
using Stump.Server.WorldServer.Database.Monsters;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Monsters;
using Stump.Server.WorldServer.Game.Fights;
using Stump.Server.WorldServer.Game.Fights.Teams;
using Stump.Server.WorldServer.Game.Maps.Cells;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Maps.Spawns
{
	public class DungeonSpawningPool : SpawningPoolBase
	{
		[Variable(true)]
		public static int DungeonSpawnsInterval = 30;
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();
		private readonly object m_locker = new object();
		private readonly System.Collections.Generic.List<MonsterDungeonSpawn> m_spawns = new System.Collections.Generic.List<MonsterDungeonSpawn>();
		private Queue<MonsterDungeonSpawn> m_spawnsQueue = new Queue<MonsterDungeonSpawn>();
		private readonly System.Collections.Generic.Dictionary<MonsterGroup, MonsterDungeonSpawn> m_groupsSpawn = new System.Collections.Generic.Dictionary<MonsterGroup, MonsterDungeonSpawn>();

		public DungeonSpawningPool(Map map) : this(map, DungeonSpawningPool.DungeonSpawnsInterval)
		{
		}
		public DungeonSpawningPool(Map map, int interval) : base(map, interval)
		{
		}

		public void AddSpawn(MonsterDungeonSpawn spawn)
		{
			lock (this.m_locker)
			{
				this.m_spawns.Add(spawn);
				this.m_spawnsQueue.Enqueue(spawn);
			}
		}
		public void RemoveSpawn(MonsterDungeonSpawn spawn)
		{
			lock (this.m_locker)
			{
				this.m_spawns.Remove(spawn);
				System.Collections.Generic.List<MonsterDungeonSpawn> list = this.m_spawnsQueue.ToList<MonsterDungeonSpawn>();
				if (list.Remove(spawn))
				{
					this.m_spawnsQueue = new Queue<MonsterDungeonSpawn>(list);
				}
			}
		}
		protected override bool IsLimitReached()
		{
			return this.m_spawnsQueue.Count == 0;
		}
		protected override MonsterGroup DequeueNextGroupToSpawn()
		{
			MonsterGroup result;
			if (!base.Map.CanSpawnMonsters())
			{
				base.StopAutoSpawn();
				result = null;
			}
			else
			{
				lock (this.m_locker)
				{
                    if (this.m_spawnsQueue.Count == 0)
                    {
                        DungeonSpawningPool.logger.Error("SpawningPool Map = {0} try to spawn a monser group but m_groupsToSpawn is empty", base.Map.Id);
                        result = null;
                    }
                    else
                    {
                        MonsterDungeonSpawn monsterDungeonSpawn = this.m_spawnsQueue.Dequeue();
                        MonsterGroup monsterGroup = new MonsterGroup(base.Map.GetNextContextualId(), new ObjectPosition(base.Map, base.Map.GetRandomFreeCell(false), base.Map.GetRandomDirection()));
                        foreach (MonsterDungeonSpawnEntity current in monsterDungeonSpawn.GroupMonsters)
                        {
                            var monster = current.GenerateMonster(ref monsterGroup);
                            if (monster != null)
                            {
                                monsterGroup.AddMonster(monster);
                            }
                        }
                        if (monsterGroup.GetMonsters().Count() == 0)
                        {
                            DungeonSpawningPool.logger.Error("SpawningPool Map = {0} try to spawn a monser group but MonsterGroup is empty", base.Map.Id);
                            result = null;
                        }
                        else
                        {
                            this.m_groupsSpawn.Add(monsterGroup, monsterDungeonSpawn);
                            result = monsterGroup;
                        }
                    }
				}
			}
			return result;
		}
		protected override int GetNextSpawnInterval()
		{
			return base.Interval * 1000;
		}
		protected override void OnGroupSpawned(MonsterGroup group)
		{
			group.EnterFight += new Action<MonsterGroup, Character>(this.OnGroupEnterFight);
			base.OnGroupSpawned(group);
		}
		private void OnGroupEnterFight(MonsterGroup group, Character character)
		{
			group.EnterFight -= new Action<MonsterGroup, Character>(this.OnGroupEnterFight);
			group.Fight.WinnersDetermined += new Fight.FightWinnersDelegate(this.OnWinnersDetermined);
		}
		private void OnWinnersDetermined(Fight fight, FightTeam winners, FightTeam losers, bool draw)
		{
			fight.WinnersDetermined -= new Fight.FightWinnersDelegate(this.OnWinnersDetermined);
			if (!draw && (winners is FightPlayerTeam && losers is FightMonsterTeam))
			{
				MonsterGroup group = ((MonsterFighter)losers.Leader).Monster.Group;
				if (!this.m_groupsSpawn.ContainsKey(group))
				{
					DungeonSpawningPool.logger.Error<int, int>("Group {0} (Map {1}) has ended his fight but is not register in the pool", group.Id, base.Map.Id);
				}
				else
				{
					MonsterDungeonSpawn monsterDungeonSpawn = this.m_groupsSpawn[group];
					if (monsterDungeonSpawn.TeleportEvent)
					{
						ObjectPosition teleportPosition = monsterDungeonSpawn.GetTeleportPosition();
						foreach (CharacterFighter current in winners.GetAllFighters<CharacterFighter>())
						{
							current.Character.NextMap = teleportPosition.Map;
							current.Character.Cell = teleportPosition.Cell;
							current.Character.Direction = teleportPosition.Direction;
							this.m_groupsSpawn.Remove(group);
						}
					}
				}
			}
		}
		protected override void OnGroupUnSpawned(MonsterGroup monster)
		{
			lock (this.m_locker)
			{
				if (!this.m_groupsSpawn.ContainsKey(monster))
				{
					DungeonSpawningPool.logger.Error<int, int>("Group {0} (Map {1}) was not bind to a dungeon spawn", monster.Id, base.Map.Id);
				}
				else
				{
					MonsterDungeonSpawn item = this.m_groupsSpawn[monster];
					if (this.m_spawns.Contains(item))
					{
						this.m_spawnsQueue.Enqueue(item);
					}
				}
			}
			base.OnGroupUnSpawned(monster);
		}
	}
}
