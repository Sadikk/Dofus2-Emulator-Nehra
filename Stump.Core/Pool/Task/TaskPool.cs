using NLog;
using Stump.Core.Collections;
using System;

namespace Stump.Core.Pool.Task
{
    public class TaskPool
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private BlockingQueue<Action> m_tasks = new BlockingQueue<Action>();

        public void Clear()
        {
            this.m_tasks = new BlockingQueue<Action>();
        }

        public void EnqueueTask(Action action)
        {
            this.m_tasks.Enqueue(action);
        }

        public void ProcessUpdate()
        {
            do
            {
                Action action = this.m_tasks.Dequeue();
                try
                {
                    action();
                }
                catch (Exception argument)
                {
                    this.logger.Error<Action, Exception>("Exception occurs in the task Pool for action {0} : {1}", action, argument);
                }
            }
            while (this.m_tasks.Count > 0);
        }
    }
}