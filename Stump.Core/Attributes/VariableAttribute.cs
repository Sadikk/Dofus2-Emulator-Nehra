using System;

namespace Stump.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class VariableAttribute : Attribute
    {
        public bool DefinableRunning
        {
            get;
            set;
        }
        public int Priority
        {
            get;
            set;
        }

        public VariableAttribute()
        {
            this.Priority = 1;
        }

        public VariableAttribute(bool definableByConfig = false)
        {
            this.DefinableRunning = definableByConfig;
            this.Priority = 1;
        }
    }
}