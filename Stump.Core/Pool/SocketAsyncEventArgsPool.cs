using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Stump.Core.Pool
{
    public sealed class SocketAsyncEventArgsPool : IDisposable
    {
        private Stack<SocketAsyncEventArgs> m_pool;
        private bool m_disposed;
        public int Count
        {
            get
            {
                return this.m_pool.Count;
            }
        }

        public SocketAsyncEventArgsPool(int capacity)
        {
            this.m_pool = new Stack<SocketAsyncEventArgs>(capacity);
        }

        public void Dispose()
        {
            this.m_pool.Clear();
            this.m_disposed = true;
        }

        public void Push(SocketAsyncEventArgs item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item", "Impossible d'ajouter un élement nul au pool");
            }
            lock (this.m_pool)
            {
                this.m_pool.Push(item);
            }
        }

        public SocketAsyncEventArgs Pop()
        {
            if (this.m_disposed)
            {
                throw new ObjectDisposedException("SocketAsyncEventArgsPool");
            }
            SocketAsyncEventArgs result;
            lock (this.m_pool)
            {
                result = this.m_pool.Pop();
            }
            return result;
        }
    }
}