using System.Collections.Generic;

namespace Stump.Core.Timers
{
    public class TimedTimerComparer : IComparer<TimedTimerEntry>
    {
        public int Compare(TimedTimerEntry x, TimedTimerEntry y)
        {
            return x.NextTick.CompareTo(y.NextTick);
        }
    }
}