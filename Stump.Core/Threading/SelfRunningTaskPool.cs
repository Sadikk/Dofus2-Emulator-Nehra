using NLog;
using Stump.Core.Collections;
using Stump.Core.Timers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Stump.Core.Threading
{
    public class SelfRunningTaskPool : IContextHandler
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly LockFreeQueue<IMessage> m_messageQueue;
        private readonly Stopwatch m_queueTimer;
        private readonly List<SimpleTimerEntry> m_simpleTimers;
        private readonly ManualResetEvent m_stoppedAsync = new ManualResetEvent(false);
        private readonly List<TimerEntry> m_timers;
        private int m_currentThreadId;
        private int m_lastUpdate;
        private Task m_updateTask;
        public string Name
        {
            get;
            set;
        }
        public int UpdateInterval
        {
            get;
            set;
        }
        public long LastUpdateTime
        {
            get
            {
                return (long)this.m_lastUpdate;
            }
        }
        public bool IsRunning
        {
            get;
            protected set;
        }
        public ReadOnlyCollection<TimerEntry> Timers
        {
            get
            {
                return this.m_timers.AsReadOnly();
            }
        }
        public ReadOnlyCollection<SimpleTimerEntry> SimpleTimers
        {
            get
            {
                return this.m_simpleTimers.AsReadOnly();
            }
        }
        public bool IsInContext
        {
            get
            {
                return Thread.CurrentThread.ManagedThreadId == this.m_currentThreadId;
            }
        }

        public SelfRunningTaskPool(int interval, string name)
        {
            this.m_messageQueue = new LockFreeQueue<IMessage>();
            this.m_queueTimer = Stopwatch.StartNew();
            this.m_simpleTimers = new List<SimpleTimerEntry>();
            this.m_timers = new List<TimerEntry>();
            this.UpdateInterval = interval;
            this.Name = name;
            this.Start();
        }

        public void AddMessage(IMessage message)
        {
            this.m_messageQueue.Enqueue(message);
        }

        public void AddMessage(Action action)
        {
            this.m_messageQueue.Enqueue(new Message(action));
        }

        public bool ExecuteInContext(Action action)
        {
            bool result;
            if (this.IsInContext)
            {
                action();
                result = true;
            }
            else
            {
                this.AddMessage(action);
                result = false;
            }
            return result;
        }

        public void EnsureContext()
        {
            if (!this.IsInContext)
            {
                throw new InvalidOperationException("Not in context");
            }
        }

        public void Start()
        {
            this.IsRunning = true;
            this.m_updateTask = Task.Factory.StartNewDelayed(this.UpdateInterval, new Action<object>(this.ProcessCallback), this);
        }

        public void Stop(bool async = false)
        {
            this.IsRunning = false;
            if (async && this.m_currentThreadId != 0)
            {
                this.m_stoppedAsync.WaitOne();
            }
        }

        public void EnsureNotContext()
        {
            if (this.IsInContext)
            {
                throw new InvalidOperationException("Forbidden context");
            }
        }

        public void AddTimer(TimerEntry timer)
        {
            this.AddMessage(delegate
            {
                this.m_timers.Add(timer);
            });
        }

        public void RemoveTimer(TimerEntry timer)
        {
            this.AddMessage(delegate
            {
                this.m_timers.Remove(timer);
            });
        }

        public void CancelSimpleTimer(SimpleTimerEntry timer)
        {
            this.m_simpleTimers.Remove(timer);
        }

        public SimpleTimerEntry CallPeriodically(int delayMillis, Action callback)
        {
            SimpleTimerEntry simpleTimerEntry = new SimpleTimerEntry(delayMillis, callback, this.LastUpdateTime, false);
            this.m_simpleTimers.Add(simpleTimerEntry);
            return simpleTimerEntry;
        }

        public SimpleTimerEntry CallDelayed(int delayMillis, Action callback)
        {
            SimpleTimerEntry simpleTimerEntry = new SimpleTimerEntry(delayMillis, callback, this.LastUpdateTime, true);
            this.m_simpleTimers.Add(simpleTimerEntry);
            return simpleTimerEntry;
        }

        internal int GetDelayUntilNextExecution(SimpleTimerEntry timer)
        {
            return timer.Delay - (int)(this.LastUpdateTime - timer.LastCallTime);
        }

        protected void ProcessCallback(object state)
        {
            if (this.IsRunning && Interlocked.CompareExchange(ref this.m_currentThreadId, Thread.CurrentThread.ManagedThreadId, 0) == 0)
            {
                long num = 0L;
                try
                {
                    num = this.m_queueTimer.ElapsedMilliseconds;
                    int dtMillis = (int)(num - (long)this.m_lastUpdate);
                    this.m_lastUpdate = (int)num;
                    foreach (TimerEntry current in this.m_timers)
                    {
                        try
                        {
                            current.Update(dtMillis);
                        }
                        catch (Exception argument)
                        {
                            SelfRunningTaskPool.logger.Error<TimerEntry, Exception>("Failed to update {0} : {1}", current, argument);
                        }
                        if (!this.IsRunning)
                        {
                            return;
                        }
                    }
                    int count = this.m_simpleTimers.Count;
                    int num2 = count - 1;
                    while (num2 >= 0)
                    {
                        SimpleTimerEntry simpleTimerEntry = this.m_simpleTimers[num2];
                        if (this.GetDelayUntilNextExecution(simpleTimerEntry) > 0)
                        {
                            goto IL_107;
                        }
                        try
                        {
                            simpleTimerEntry.Execute(this);
                            goto IL_107;
                        }
                        catch (Exception argument)
                        {
                            SelfRunningTaskPool.logger.Error<SimpleTimerEntry, Exception>("Failed to execute timer {0} : {1}", simpleTimerEntry, argument);
                            goto IL_107;
                        }
                    IL_FF:
                        num2--;
                        continue;
                    IL_107:
                        if (!this.IsRunning)
                        {
                            return;
                        }
                        goto IL_FF;
                    }
                    IMessage message;
                    while (this.m_messageQueue.TryDequeue(out message))
                    {
                        try
                        {
                            message.Execute();
                        }
                        catch (Exception argument)
                        {
                            SelfRunningTaskPool.logger.Error<IMessage, Exception>("Failed to execute message {0} : {1}", message, argument);
                        }
                        if (!this.IsRunning)
                        {
                            break;
                        }
                    }
                }
                catch (Exception argument)
                {
                    SelfRunningTaskPool.logger.Error<string, Exception>("Failed to run TaskQueue callback for \"{0}\" : {1}", this.Name, argument);
                }
                finally
                {
                    long elapsedMilliseconds = this.m_queueTimer.ElapsedMilliseconds;
                    bool flag;
                    long num3 = (flag = (elapsedMilliseconds - num > (long)this.UpdateInterval)) ? 0L : (num + (long)this.UpdateInterval - elapsedMilliseconds);
                    Interlocked.Exchange(ref this.m_currentThreadId, 0);
                    if (flag)
                    {
                        SelfRunningTaskPool.logger.Debug<string, long>("TaskPool '{0}' update lagged ({1}ms)", this.Name, elapsedMilliseconds - num);
                    }
                    if (this.IsRunning)
                    {
                        this.m_updateTask = Task.Factory.StartNewDelayed((int)num3, new Action<object>(this.ProcessCallback), this);
                    }
                    else
                    {
                        this.m_stoppedAsync.Set();
                    }
                }
            }
        }
    }
}