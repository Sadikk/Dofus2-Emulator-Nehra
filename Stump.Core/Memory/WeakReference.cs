using System;
using System.Runtime.InteropServices;

namespace Stump.Core.Memory
{
    public sealed class WeakReference<T> : IDisposable where T : class
    {
        private SafeGCHandle safeHandle;
        public T Target
        {
            get
            {
                return this.safeHandle.Handle.Target as T;
            }
        }
        public bool IsAlive
        {
            get
            {
                return this.safeHandle.Handle.Target != null;
            }
        }

        public WeakReference(T target)
        {
            this.safeHandle = new SafeGCHandle(target, GCHandleType.Weak);
        }

        public void Dispose()
        {
            this.safeHandle.Dispose();
        }
    }
}