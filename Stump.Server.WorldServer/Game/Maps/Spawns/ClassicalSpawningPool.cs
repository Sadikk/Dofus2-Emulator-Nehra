using NLog;
using Stump.Core.Extensions;
using Stump.Core.Threading;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Maps.Spawns
{
	public class ClassicalSpawningPool : SpawningPoolBase
	{
		protected enum GroupSize
		{
			Small,
			Medium,
			Big
		}
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();
		protected System.Collections.Generic.Dictionary<ClassicalSpawningPool.GroupSize, Tuple<int, int>> GroupSizes = new System.Collections.Generic.Dictionary<ClassicalSpawningPool.GroupSize, Tuple<int, int>>
		{

			{
				ClassicalSpawningPool.GroupSize.Small,
				Tuple.Create<int, int>(1, 2)
			},

			{
				ClassicalSpawningPool.GroupSize.Medium,
				Tuple.Create<int, int>(3, 5)
			},

			{
				ClassicalSpawningPool.GroupSize.Big,
				Tuple.Create<int, int>(6, 8)
			}
		};
		private readonly object m_locker = new object();
		private readonly MonsterGroup[] m_groupsBySize = new MonsterGroup[3];
		private readonly Queue<ClassicalSpawningPool.GroupSize> m_groupsToSpawn = new Queue<ClassicalSpawningPool.GroupSize>();
		public MonsterGroup SmallGroup
		{
			get
			{
				return this.m_groupsBySize[0];
			}
		}
		public MonsterGroup MediumGroup
		{
			get
			{
				return this.m_groupsBySize[1];
			}
		}
		public MonsterGroup BigGroup
		{
			get
			{
				return this.m_groupsBySize[2];
			}
		}
		public ClassicalSpawningPool(Map map) : base(map)
		{
			this.RandomQueue();
		}
		public ClassicalSpawningPool(Map map, int interval) : base(map, interval)
		{
			this.RandomQueue();
		}
		private void RandomQueue()
		{
			System.Array values = System.Enum.GetValues(typeof(ClassicalSpawningPool.GroupSize));
			foreach (ClassicalSpawningPool.GroupSize current in values.Cast<ClassicalSpawningPool.GroupSize>().Shuffle<ClassicalSpawningPool.GroupSize>())
			{
				this.m_groupsToSpawn.Enqueue(current);
			}
		}
		protected override bool IsLimitReached()
		{
			return this.m_groupsToSpawn.Count == 0;
		}
		protected override MonsterGroup DequeueNextGroupToSpawn()
		{
			MonsterGroup result;
			lock (this.m_locker)
			{
				if (this.m_groupsToSpawn.Count == 0)
				{
					result = null;
				}
				else
				{
					ClassicalSpawningPool.GroupSize groupSize = this.m_groupsToSpawn.Dequeue();
					result = (this.m_groupsBySize[(int)groupSize] = base.Map.GenerateRandomMonsterGroup(this.GroupSizes[groupSize].Item1, this.GroupSizes[groupSize].Item2));
				}
			}
			return result;
		}
		protected override int GetNextSpawnInterval()
		{
			AsyncRandom asyncRandom = new AsyncRandom();
			int result;
			if (asyncRandom.Next(0, 2) == 0)
			{
				result = (int)(((double)base.Interval - asyncRandom.NextDouble() * (double)base.Interval / 4.0) * 1000.0);
			}
			else
			{
				result = (int)(((double)base.Interval + asyncRandom.NextDouble() * (double)base.Interval / 4.0) * 1000.0);
			}
			return result;
		}
		protected override void OnGroupUnSpawned(MonsterGroup monster)
		{
			lock (this.m_locker)
			{
				if (monster == this.SmallGroup)
				{
					this.m_groupsBySize[0] = null;
					this.m_groupsToSpawn.Enqueue(ClassicalSpawningPool.GroupSize.Small);
				}
				if (monster == this.MediumGroup)
				{
					this.m_groupsBySize[1] = null;
					this.m_groupsToSpawn.Enqueue(ClassicalSpawningPool.GroupSize.Medium);
				}
				if (monster == this.BigGroup)
				{
					this.m_groupsBySize[2] = null;
					this.m_groupsToSpawn.Enqueue(ClassicalSpawningPool.GroupSize.Big);
				}
			}
			base.OnGroupUnSpawned(monster);
		}
	}
}
