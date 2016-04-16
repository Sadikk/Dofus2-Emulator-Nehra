using System.Collections.Generic;
using System.Threading;

namespace Stump.Core.Collections
{
    public sealed class BlockingQueue<T>
    {
        private readonly object m_lockObj = new object();
        private readonly Queue<T> m_queue;
        public bool IsWaiting
        {
            get;
            private set;
        }
        public int Count
        {
            get
            {
                return this.m_queue.Count;
            }
        }

        public BlockingQueue(int quantity)
        {
            this.m_queue = new Queue<T>();
        }

        public BlockingQueue()
        {
            this.m_queue = new Queue<T>();
        }

        public void Enqueue(T element)
        {
            lock (this.m_lockObj)
            {
                this.m_queue.Enqueue(element);
                if (this.m_queue.Count == 1)
                {
                    Monitor.Pulse(this.m_lockObj);
                }
            }
        }

        public T Dequeue()
        {
            T result;
            lock (this.m_lockObj)
            {
                while (this.m_queue.Count == 0)
                {
                    this.IsWaiting = true;
                    Monitor.Wait(this.m_lockObj);
                }
                this.IsWaiting = false;
                result = this.m_queue.Dequeue();
            }
            return result;
        }
    }
}