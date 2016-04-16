using NLog;
using Stump.Core.Attributes;
using Stump.Core.Collections;
using Stump.Core.Threading;
using Stump.Core.Timers;
using Stump.Server.WorldServer.Database.Monsters;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace Stump.Server.WorldServer.Game.Maps
{
	public class Area : IContextHandler
	{
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();
		[Variable]
		public static readonly int DefaultUpdateDelay = 50;
		public static int[] UpdatePriorityMillis = new int[]
		{
			10000,
			3000,
			1000,
			6000,
			300,
			0
		};
		private readonly System.Collections.Generic.List<Character> m_characters = new System.Collections.Generic.List<Character>();
		private readonly System.Collections.Generic.List<Map> m_maps = new System.Collections.Generic.List<Map>();
		private readonly LockFreeQueue<IMessage> m_messageQueue = new LockFreeQueue<IMessage>();
		private readonly System.Collections.Generic.List<MonsterSpawn> m_monsterSpawns = new System.Collections.Generic.List<MonsterSpawn>();
		private readonly System.Collections.Generic.List<WorldObject> m_objects = new System.Collections.Generic.List<WorldObject>();
		private readonly System.Collections.Generic.List<SubArea> m_subAreas = new System.Collections.Generic.List<SubArea>();
		private readonly System.Collections.Generic.Dictionary<Point, System.Collections.Generic.List<Map>> m_mapsByPoint = new System.Collections.Generic.Dictionary<Point, System.Collections.Generic.List<Map>>();
		private readonly PriorityQueueB<TimedTimerEntry> m_timers = new PriorityQueueB<TimedTimerEntry>(new TimedTimerComparer());
		private readonly System.Collections.Generic.List<TimedTimerEntry> m_pausedTimers = new System.Collections.Generic.List<TimedTimerEntry>();
		private readonly System.Threading.ManualResetEvent m_stoppedAsync = new System.Threading.ManualResetEvent(false);
		protected internal AreaRecord Record;
		private int m_currentThreadId;
		private bool m_isUpdating;
		private System.DateTime m_lastUpdateTime;
		private bool m_running;
		private int m_updateDelay;
		public event System.Action<Area> Started;
		public event System.Action<Area> Stopped;
		public int Id
		{
			get
			{
				return this.Record.Id;
			}
		}
		public string Name
		{
			get
			{
				return this.Record.Name;
			}
		}
		public System.Collections.Generic.IEnumerable<SubArea> SubAreas
		{
			get
			{
				return this.m_subAreas;
			}
		}
		public System.Collections.Generic.IEnumerable<Map> Maps
		{
			get
			{
				return this.m_maps;
			}
		}
		public System.Collections.Generic.Dictionary<Point, System.Collections.Generic.List<Map>> MapsByPosition
		{
			get
			{
				return this.m_mapsByPoint;
			}
		}
		public SuperArea SuperArea
		{
			get;
			internal set;
		}
		public int ObjectCount
		{
			get
			{
				return this.m_objects.Count;
			}
		}
		public int TimersCount
		{
			get
			{
				return this.m_timers.Count;
			}
		}
		public System.Collections.Generic.List<Character> Characters
		{
			get
			{
				this.EnsureContext();
				return this.m_characters;
			}
		}
		public int CharacterCount
		{
			get
			{
				return this.m_characters.Count;
			}
		}
		public bool IsRunning
		{
			get
			{
				return this.m_running;
			}
			set
			{
				if (this.m_running != value)
				{
					if (value)
					{
						this.Start();
					}
					else
					{
						this.Stop(false);
					}
				}
			}
		}
		public int TickCount
		{
			get;
			private set;
		}
		public int UpdateDelay
		{
			get
			{
				return this.m_updateDelay;
			}
			set
			{
				System.Threading.Interlocked.Exchange(ref this.m_updateDelay, value);
			}
		}
		public System.DateTime LastUpdateTime
		{
			get
			{
				return this.m_lastUpdateTime;
			}
		}
		public bool IsUpdating
		{
			get
			{
				return this.m_isUpdating;
			}
		}
		public float AverageUpdateTime
		{
			get;
			private set;
		}
		public bool IsDisposed
		{
			get;
			private set;
		}
		public int CurrentThreadId
		{
			get
			{
				return this.m_currentThreadId;
			}
		}
		public bool IsInContext
		{
			get
			{
				return System.Threading.Thread.CurrentThread.ManagedThreadId == this.m_currentThreadId;
			}
		}
		public System.Collections.ObjectModel.ReadOnlyCollection<MonsterSpawn> MonsterSpawns
		{
			get
			{
				return this.m_monsterSpawns.AsReadOnly();
			}
		}
		public Area(AreaRecord record)
		{
			this.Record = record;
			this.m_updateDelay = Area.DefaultUpdateDelay;
		}
		public void AddMessage(Action action)
		{
			this.AddMessage(new Message(action));
		}
		public void AddMessage(IMessage msg)
		{
			this.Start();
			this.m_messageQueue.Enqueue(msg);
		}
		public bool ExecuteInContext(Action action)
		{
			bool result;
			if (!this.IsInContext)
			{
				this.AddMessage(new Message(action));
				result = false;
			}
			else
			{
				action();
				result = true;
			}
			return result;
		}
		public void EnsureContext()
		{
			if (System.Threading.Thread.CurrentThread.ManagedThreadId != this.m_currentThreadId && this.IsRunning)
			{
				this.Stop(false);
				throw new System.InvalidOperationException(string.Format("Context needed in Area '{0}'", this));
			}
		}
		private void OnStarted()
		{
			System.Action<Area> started = this.Started;
			if (started != null)
			{
				started(this);
			}
		}
		private void OnStopped()
		{
			System.Action<Area> stopped = this.Stopped;
			if (stopped != null)
			{
				stopped(this);
			}
		}
		public void Start()
		{
			if (!this.m_running)
			{
				lock (this.m_objects)
				{
					if (!this.m_running)
					{
						this.m_running = true;
						Area.logger.Info<Area>("Area '{0}' started", this);
						Task.Factory.StartNewDelayed(this.m_updateDelay, new System.Action<object>(this.UpdateCallback), this);
						this.SpawnMapsLater();
						this.m_lastUpdateTime = System.DateTime.Now;
						this.OnStarted();
					}
				}
			}
		}
		public void Stop(bool wait = false)
		{
			if (this.m_running)
			{
				lock (this.m_objects)
				{
					if (this.m_running)
					{
						this.m_running = false;
						if (wait && this.m_currentThreadId != 0)
						{
							this.m_stoppedAsync.WaitOne();
						}
						Area.logger.Info<Area>("Area '{0}' stopped", this);
					}
				}
			}
		}
		public void RegisterTimer(TimedTimerEntry timer)
		{
			this.EnsureContext();
			this.m_timers.Push(timer);
		}
		public void UnregisterTimer(TimedTimerEntry timer)
		{
			this.EnsureContext();
			this.m_timers.Remove(timer);
		}
		public void RegisterTimerLater(TimedTimerEntry timer)
		{
			this.m_messageQueue.Enqueue(new Message(delegate
			{
				this.RegisterTimer(timer);
			}));
		}
		public void UnregisterTimerLater(TimedTimerEntry timer)
		{
			this.m_messageQueue.Enqueue(new Message(delegate
			{
				this.UnregisterTimer(timer);
			}));
		}
		public TimedTimerEntry CallDelayed(int delay, Action action)
		{
			TimedTimerEntry timedTimerEntry = new TimedTimerEntry
			{
				Interval = -1,
				Delay = delay,
				Action = action
			};
			timedTimerEntry.Start();
			this.RegisterTimerLater(timedTimerEntry);
			return timedTimerEntry;
		}
		public TimedTimerEntry CallPeriodically(int interval, Action action)
		{
			TimedTimerEntry timedTimerEntry = new TimedTimerEntry
			{
				Interval = interval,
				Action = action
			};
			timedTimerEntry.Start();
			this.RegisterTimerLater(timedTimerEntry);
			return timedTimerEntry;
		}
        private void UpdateCallback(object state)
        {
            if ((this.IsDisposed || !this.IsRunning ? 0 : (Interlocked.CompareExchange(ref this.m_currentThreadId, Thread.CurrentThread.ManagedThreadId, 0) == 0 ? 1 : 0)) == 0)
                return;
            DateTime now = DateTime.Now;
            int num1 = (int)(now - this.m_lastUpdateTime).TotalMilliseconds;
            long num2 = 0L;
            long num3 = 0L;
            int num4 = 0;
            try
            {
                Stopwatch stopwatch1 = Stopwatch.StartNew();
                IMessage message;
                while (this.m_messageQueue.TryDequeue(out message))
                {
                    try
                    {
                        message.Execute();
                    }
                    catch (Exception ex)
                    {
                        Area.logger.Error<Area, Exception>("Exception raised when processing Message in {0} : {1}.", this, ex);
                    }
                }
                stopwatch1.Stop();
                num2 = stopwatch1.ElapsedMilliseconds;
                this.m_isUpdating = true;
                foreach (TimedTimerEntry timedTimerEntry in Enumerable.Where<TimedTimerEntry>((IEnumerable<TimedTimerEntry>)this.m_pausedTimers, (Func<TimedTimerEntry, bool>)(timer => timer.Enabled)))
                    this.m_timers.Push(timedTimerEntry);
                Stopwatch stopwatch2 = Stopwatch.StartNew();
                TimedTimerEntry timedTimerEntry1;
                while (((timedTimerEntry1 = this.m_timers.Peek()) == null ? 0 : (timedTimerEntry1.NextTick <= DateTime.Now ? 1 : 0)) != 0)
                {
                    TimedTimerEntry timedTimerEntry2 = this.m_timers.Pop();
                    if (!timedTimerEntry2.Enabled)
                    {
                        if (!timedTimerEntry2.IsDisposed)
                            this.m_pausedTimers.Add(timedTimerEntry2);
                    }
                    else
                    {
                        try
                        {
                            timedTimerEntry2.Trigger();
                            if (timedTimerEntry2.Enabled)
                                this.m_timers.Push(timedTimerEntry2);
                            ++num4;
                        }
                        catch (Exception ex)
                        {
                            Area.logger.Error<Area, Exception>("Exception raised when processing TimerEntry in {0} : {1}.", this, ex);
                        }
                    }
                }
                stopwatch2.Stop();
                num3 = stopwatch2.ElapsedMilliseconds;
            }
            finally
            {
                try
                {
                    this.m_lastUpdateTime = now;
                    ++this.TickCount;
                    this.m_isUpdating = false;
                    TimeSpan timeSpan = DateTime.Now - now;
                    this.AverageUpdateTime = (float)(((double)this.AverageUpdateTime * 9.0 + timeSpan.TotalMilliseconds) / 10.0);
                    Interlocked.Exchange(ref this.m_currentThreadId, 0);
                    int millisecondsDelay = (int)((double)this.m_updateDelay - timeSpan.TotalMilliseconds);
                    if (millisecondsDelay < 0)
                    {
                        millisecondsDelay = 0;
                        Area.logger.Debug("Area '{0}' update lagged ({1}ms) (msg:{2}ms, timers:{3}ms, timerProc:{4}/{5})", (object)this, (object)(int)timeSpan.TotalMilliseconds, (object)num2, (object)num3, (object)num4, (object)this.m_timers.Count);
                    }
                    if (!this.IsRunning)
                        this.m_stoppedAsync.Set();
                    TaskFactoryExtensions.StartNewDelayed(Task.Factory, millisecondsDelay, new Action<object>(this.UpdateCallback), (object)this);
                }
                catch (Exception ex)
                {
                    Area.logger.Error<Area, Exception>("Area {0}. Could not recall callback !! Exception {1}", this, ex);
                }
            }
        }
		public void Dispose()
		{
			this.IsDisposed = true;
			if (this.IsRunning)
			{
				this.Stop(false);
			}
		}
		public void Enter(WorldObject obj)
		{
			this.m_objects.Add(obj);
			if (obj is Character)
			{
				this.m_characters.Add((Character)obj);
				if (!this.IsRunning)
				{
					this.Start();
				}
			}
		}
		public void Leave(WorldObject obj)
		{
			this.m_objects.Remove(obj);
			if (obj is Character)
			{
				this.m_characters.Remove((Character)obj);
				if (this.m_characters.Count <= 0 && this.IsRunning)
				{
					this.Stop(false);
				}
			}
		}
		public void SpawnMapsLater()
		{
			this.AddMessage(new Action(this.SpawnMaps));
		}
		public void SpawnMaps()
		{
			this.EnsureContext();
			foreach (Map current in 
				from map in this.Maps
                where !map.SpawnEnabled && !map.IsDungeonSpawn && map.MonsterSpawnsCount > 0
				select map)
			{
				current.EnableClassicalMonsterSpawns();
			}
		}
		public void CallOnAllCharacters(System.Action<Character> action)
		{
			this.ExecuteInContext(delegate
			{
				foreach (Character current in this.m_characters)
				{
					action(current);
				}
			});
		}
		internal void AddSubArea(SubArea subArea)
		{
			this.m_subAreas.Add(subArea);
			this.m_maps.AddRange(subArea.Maps);
			foreach (Map current in subArea.Maps)
			{
				if (!this.m_mapsByPoint.ContainsKey(current.Position))
				{
					this.m_mapsByPoint.Add(current.Position, new System.Collections.Generic.List<Map>());
				}
				this.m_mapsByPoint[current.Position].Add(current);
			}
			subArea.Area = this;
		}
		internal void RemoveSubArea(SubArea subArea)
		{
			this.m_subAreas.Remove(subArea);
			this.m_maps.RemoveAll(delegate(Map entry)
			{
				bool result;
				if (subArea.Maps.Contains(entry))
				{
					if (this.m_mapsByPoint.ContainsKey(entry.Position))
					{
						System.Collections.Generic.List<Map> list = this.m_mapsByPoint[entry.Position];
						list.Remove(entry);
						if (list.Count <= 0)
						{
							this.m_mapsByPoint.Remove(entry.Position);
						}
					}
					result = true;
				}
				else
				{
					result = false;
				}
				return result;
			});
			subArea.Area = null;
		}
		public Map[] GetMaps(int x, int y)
		{
			return this.GetMaps(new Point(x, y));
		}
		public Map[] GetMaps(int x, int y, bool outdoor)
		{
			return this.GetMaps(new Point(x, y), outdoor);
		}
		public Map[] GetMaps(Point position)
		{
			return (!this.m_mapsByPoint.ContainsKey(position)) ? new Map[0] : this.m_mapsByPoint[position].ToArray();
		}
		public Map[] GetMaps(Point position, bool outdoor)
		{
			return (!this.m_mapsByPoint.ContainsKey(position)) ? new Map[0] : (
				from entry in this.m_mapsByPoint[position]
				where entry.Outdoor == outdoor
				select entry).ToArray<Map>();
		}
		public void AddMonsterSpawn(MonsterSpawn spawn)
		{
			this.m_monsterSpawns.Add(spawn);
			foreach (SubArea current in this.SubAreas)
			{
				current.AddMonsterSpawn(spawn);
			}
		}
		public void RemoveMonsterSpawn(MonsterSpawn spawn)
		{
			this.m_monsterSpawns.Remove(spawn);
			foreach (SubArea current in this.SubAreas)
			{
				current.RemoveMonsterSpawn(spawn);
			}
		}
		public void EnsureNoContext()
		{
			if (System.Threading.Thread.CurrentThread.ManagedThreadId == this.m_currentThreadId)
			{
				this.Stop(false);
				throw new System.InvalidOperationException(string.Format("Context prohibitted in Area '{0}'", this));
			}
		}
		public void EnsureNotUpdating()
		{
			if (this.m_isUpdating)
			{
				this.Stop(false);
				throw new System.InvalidOperationException(string.Format("Area '{0}' is updating", this));
			}
		}
		public override string ToString()
		{
			return string.Format("{0} ({1})", this.Name, this.Id);
		}
	}
}
