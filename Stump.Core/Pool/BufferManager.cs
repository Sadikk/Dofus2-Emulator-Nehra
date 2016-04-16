using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Stump.Core.Pool
{
    public sealed class BufferManager : IDisposable
    {
        private readonly int m_bufferSize;
        private readonly int m_numBytes;
        private byte[] m_buffer;
        private int m_currentIndex;
        private Stack<int> m_freeIndexPool;

        public BufferManager(int totalBytes, int bufferSize)
        {
            this.m_numBytes = totalBytes;
            this.m_currentIndex = 0;
            this.m_bufferSize = bufferSize;
            this.m_freeIndexPool = new Stack<int>();
        }

        public void Dispose()
        {
            this.m_buffer = null;
            this.m_freeIndexPool = null;
        }

        public void InitializeBuffer()
        {
            this.m_buffer = new byte[this.m_numBytes];
        }

        public bool SetBuffer(SocketAsyncEventArgs args)
        {
            bool result;
            if (this.m_freeIndexPool.Count > 0)
            {
                args.SetBuffer(this.m_buffer, this.m_freeIndexPool.Pop(), this.m_bufferSize);
            }
            else
            {
                if (this.m_numBytes - this.m_bufferSize < this.m_currentIndex)
                {
                    result = false;
                    return result;
                }
                args.SetBuffer(this.m_buffer, this.m_currentIndex, this.m_bufferSize);
                this.m_currentIndex += this.m_bufferSize;
            }
            result = true;
            return result;
        }

        public void FreeBuffer(SocketAsyncEventArgs args)
        {
            this.m_freeIndexPool.Push(args.Offset);
            args.SetBuffer(null, 0, 0);
        }
    }
}