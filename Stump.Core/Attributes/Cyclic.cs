using System;

namespace Stump.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class Cyclic : Attribute
    {
        public int Time
        {
            get;
            set;
        }

        public Cyclic(int time)
        {
            this.Time = time;
        }
    }
}