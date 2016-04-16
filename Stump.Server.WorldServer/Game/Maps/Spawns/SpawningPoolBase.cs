using Stump.Core.Timers;
using Stump.Server.WorldServer.Game.Actors.RolePlay;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Monsters;
using System;

namespace Stump.Server.WorldServer.Game.Maps.Spawns
{
	public abstract class SpawningPoolBase
	{
		public event Action<SpawningPoolBase, MonsterGroup> Spawned;
		public Map Map
		{
			get;
			protected set;
		}
		public int Interval
		{
			get;
			protected set;
		}
		public int RemainingTime
		{
			get
			{
				return (this.State != SpawningPoolState.Running) ? 0 : ((int)(this.SpawnTimer.NextTick - System.DateTime.Now).TotalMilliseconds);
			}
		}
		protected System.Collections.Generic.List<MonsterGroup> Spawns
		{
			get;
			set;
		}
		public int SpawnsCount
		{
			get
			{
				return this.Spawns.Count;
			}
		}
		protected MonsterGroup NextGroup
		{
			get;
			set;
		}
		protected TimedTimerEntry SpawnTimer
		{
			get;
			set;
		}
		public SpawningPoolState State
		{
			get;
			private set;
		}
		public bool AutoSpawnEnabled
		{
			get
			{
				return this.State != SpawningPoolState.Stoped;
			}
		}
		protected SpawningPoolBase(Map map)
		{
			this.Map = map;
			this.Map.ActorLeave += new Action<Map, RolePlayActor>(this.OnMapActorLeave);
			this.Spawns = new System.Collections.Generic.List<MonsterGroup>();
		}
		protected SpawningPoolBase(Map map, int interval) : this(map)
		{
			this.Interval = interval;
		}
		public void StartAutoSpawn()
		{
			bool flag = false;
			try
			{
				System.Threading.Monitor.Enter(this, ref flag);
				if (this.Map.CanSpawnMonsters())
				{
					if (!this.AutoSpawnEnabled)
					{
						this.ResetTimer();
						this.State = SpawningPoolState.Running;
						this.OnAutoSpawnEnabled();
					}
				}
			}
			finally
			{
				if (flag)
				{
					System.Threading.Monitor.Exit(this);
				}
			}
		}
		protected virtual void OnAutoSpawnEnabled()
		{
			this.SpawnNextGroup();
		}
		public void StopAutoSpawn()
		{
			bool flag = false;
			try
			{
				System.Threading.Monitor.Enter(this, ref flag);
				if (this.AutoSpawnEnabled)
				{
					if (this.SpawnTimer != null)
					{
						this.SpawnTimer.Dispose();
					}
					this.State = SpawningPoolState.Stoped;
					this.OnAutoSpawnDisabled();
				}
			}
			finally
			{
				if (flag)
				{
					System.Threading.Monitor.Exit(this);
				}
			}
		}
		protected virtual void OnAutoSpawnDisabled()
		{
		}
		protected void PauseAutoSpawn()
		{
			bool flag = false;
			try
			{
				System.Threading.Monitor.Enter(this, ref flag);
				if (this.State == SpawningPoolState.Running)
				{
					if (this.SpawnTimer != null)
					{
						this.SpawnTimer.Dispose();
					}
					this.State = SpawningPoolState.Paused;
				}
			}
			finally
			{
				if (flag)
				{
					System.Threading.Monitor.Exit(this);
				}
			}
		}
		protected void ResumeAutoSpawn()
		{
			bool flag = false;
			try
			{
				System.Threading.Monitor.Enter(this, ref flag);
				if (this.State == SpawningPoolState.Paused)
				{
					this.ResetTimer();
					this.State = SpawningPoolState.Running;
					this.OnAutoSpawnEnabled();
				}
			}
			finally
			{
				if (flag)
				{
					System.Threading.Monitor.Exit(this);
				}
			}
		}
		private void TimerCallBack()
		{
			if (this.IsLimitReached())
			{
				this.PauseAutoSpawn();
			}
			else
			{
				bool flag = false;
				try
				{
					System.Threading.Monitor.Enter(this, ref flag);
					this.SpawnNextGroup();
				}
				finally
				{
					if (flag)
					{
						System.Threading.Monitor.Exit(this);
					}
				}
				if (this.IsLimitReached())
				{
					this.PauseAutoSpawn();
				}
				else
				{
					this.ResetTimer();
				}
			}
		}
		private void ResetTimer()
		{
			this.SpawnTimer = this.Map.Area.CallDelayed(this.GetNextSpawnInterval(), new Action(this.TimerCallBack));
		}
		public bool SpawnNextGroup()
		{
			MonsterGroup monsterGroup = this.DequeueNextGroupToSpawn();
			bool result;
			if (monsterGroup == null)
			{
				result = false;
			}
			else
			{
				this.Map.Enter(monsterGroup);
				this.OnGroupSpawned(monsterGroup);
				result = true;
			}
			return result;
		}
		public void SetTimer(int time)
		{
			bool flag = false;
			try
			{
				System.Threading.Monitor.Enter(this, ref flag);
				this.Interval = time;
				this.ResetTimer();
			}
			finally
			{
				if (flag)
				{
					System.Threading.Monitor.Exit(this);
				}
			}
		}
		protected abstract bool IsLimitReached();
		protected abstract int GetNextSpawnInterval();
		protected virtual MonsterGroup DequeueNextGroupToSpawn()
		{
			MonsterGroup result;
			if (this.NextGroup != null)
			{
				result = this.NextGroup;
			}
			else
			{
				result = null;
			}
			return result;
		}
		public virtual void SetNextGroupToSpawn(System.Collections.Generic.IEnumerable<Monster> monsters)
		{
			this.NextGroup = new MonsterGroup(this.Map.GetNextContextualId(), this.Map.GetRandomFreePosition(false));
			foreach (Monster current in monsters)
			{
				this.NextGroup.AddMonster(current);
			}
		}
		private void OnMapActorLeave(Map map, RolePlayActor actor)
		{
			if (actor is MonsterGroup && this.Spawns.Contains(actor as MonsterGroup))
			{
				this.OnGroupUnSpawned(actor as MonsterGroup);
			}
		}
		protected virtual void OnGroupSpawned(MonsterGroup group)
		{
			lock (this.Spawns)
			{
				this.Spawns.Add(group);
			}
			this.NextGroup = null;
			Action<SpawningPoolBase, MonsterGroup> spawned = this.Spawned;
			if (spawned != null)
			{
				spawned(this, group);
			}
		}
		protected virtual void OnGroupUnSpawned(MonsterGroup monster)
		{
			lock (this.Spawns)
			{
				this.Spawns.Remove(monster);
			}
			if (!this.IsLimitReached() && this.State == SpawningPoolState.Paused)
			{
				this.ResumeAutoSpawn();
			}
		}
	}
}
