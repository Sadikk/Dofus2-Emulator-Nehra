using System;

namespace Stump.Core.Timers
{
    public class TimedTimerEntry
    {
        private int m_interval;
        private bool m_firstCalled;
        private int m_delay;

        public bool Enabled
        {
            get;
            private set;
        }
        public bool IsDisposed
        {
            get;
            private set;
        }
        public Action Action
        {
            get;
            set;
        }
        public DateTime NextTick
        {
            get;
            private set;
        }
        public int Delay
        {
            get
            {
                return this.m_delay;
            }
            set
            {
                if (!this.m_firstCalled && this.Enabled && value != -1)
                {
                    this.NextTick = this.NextTick - TimeSpan.FromMilliseconds((double)this.m_delay) + TimeSpan.FromMilliseconds((double)value);
                }
                this.m_delay = value;
            }
        }
        public int Interval
        {
            get
            {
                return this.m_interval;
            }
            set
            {
                if (value != -1)
                {
                    this.NextTick = this.NextTick - TimeSpan.FromMilliseconds((double)this.m_interval) + TimeSpan.FromMilliseconds((double)value);
                }
                this.m_interval = value;
            }
        }

        public TimedTimerEntry() { }
        public TimedTimerEntry(int interval, Action action) 
            : this(interval, interval, action) { }
        public TimedTimerEntry(int delay, int interval, Action action)
        {
            this.m_delay = delay;
            this.Interval = interval;
            this.Action = action;
        }

        public void Start()
        {
            this.NextTick = DateTime.Now + TimeSpan.FromMilliseconds((double)this.m_delay);
            this.Enabled = true;
        }

        public void Stop()
        {
            this.Enabled = false;
        }

        public void Dispose()
        {
            this.Enabled = false;
            this.IsDisposed = true;
        }

        public bool ShouldTrigger()
        {
            return this.Enabled && DateTime.Now >= this.NextTick;
        }

        public void Trigger()
        {
            this.Action();
            if (this.Interval < 0)
            {
                this.Enabled = false;
            }
            else
            {
                this.NextTick = DateTime.Now + TimeSpan.FromMilliseconds((double)this.Interval);
            }
            this.m_firstCalled = true;
        }

        public uint GetRemainingSeconds()
        {
            var now = DateTime.Now;
            if (this.NextTick > now)
            {
                return (uint)(this.NextTick - now).TotalSeconds;
            }

            return 0;
        }
    }
}