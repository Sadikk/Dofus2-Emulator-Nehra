using System;
using System.Collections;
using System.Collections.Generic;

namespace Stump.Core.Memory
{
    public interface IWeakCollection<T> : ICollection<T>, IEnumerable<T>, IEnumerable, IDisposable where T : class
    {
        IEnumerable<T> LiveList
        {
            get;
        }
        IEnumerable<T> CompleteList
        {
            get;
        }
        IEnumerable<T> LiveListWithoutPurge
        {
            get;
        }
        int CompleteCount
        {
            get;
        }
        int DeadCount
        {
            get;
        }
        int LiveCount
        {
            get;
        }
        int LiveCountWithoutPurge
        {
            get;
        }

        void Purge();
    }
}