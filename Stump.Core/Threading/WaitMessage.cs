using System.Threading;

namespace Stump.Core.Threading
{
    public class WaitMessage : Message
    {
        private bool m_executed;

        public override void Execute()
        {
            try
            {
                base.Execute();
            }
            finally
            {
                bool flag = false;
                try
                {
                    Monitor.Enter(this, ref flag);
                    this.m_executed = true;
                    Monitor.PulseAll(this);
                }
                finally
                {
                    if (flag)
                    {
                        Monitor.Exit(this);
                    }
                }
            }
        }

        public void Wait()
        {
            if (!this.m_executed)
            {
                bool flag = false;
                try
                {
                    Monitor.Enter(this, ref flag);
                    Monitor.Wait(this);
                }
                finally
                {
                    if (flag)
                    {
                        Monitor.Exit(this);
                    }
                }
            }
        }
    }
}