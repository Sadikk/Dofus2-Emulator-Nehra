using System;

namespace Stump.Server.WorldServer.Game.Effects.Spells.TargetMask
{
    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = true)]
    public class TargetMaskHandlerAttribute : System.Attribute
    {
        public char Pattern
        {
            get;
            private set;
        }
        public TargetMaskHandlerAttribute(char pattern)
        {
            this.Pattern = pattern;
        }
    }
}

