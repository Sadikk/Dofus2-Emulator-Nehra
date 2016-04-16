using System;

namespace Stump.Core.Threading
{
    public interface IContextHandler
    {
        bool IsInContext
        {
            get;
        }

        void AddMessage(IMessage message);

        void AddMessage(Action action);

        bool ExecuteInContext(Action action);

        void EnsureContext();
    }
}