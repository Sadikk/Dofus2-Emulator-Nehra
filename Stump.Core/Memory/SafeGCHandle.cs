using System;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace Stump.Core.Memory
{
    internal sealed class SafeGCHandle : SafeHandle
    {
        public GCHandle Handle
        {
            get
            {
                return GCHandle.FromIntPtr(this.handle);
            }
        }
        public override bool IsInvalid
        {
            [PrePrepareMethod, ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            get
            {
                return this.handle == IntPtr.Zero;
            }
        }

        public SafeGCHandle(object target, GCHandleType type)
            : base(IntPtr.Zero, true)
        {
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
            }
            finally
            {
                base.SetHandle(GCHandle.ToIntPtr(GCHandle.Alloc(target, type)));
            }
        }

        [PrePrepareMethod, ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected override bool ReleaseHandle()
        {
            GCHandle.FromIntPtr(this.handle).Free();
            return true;
        }
    }
}