using System;

namespace Stump.Core.Threading
{
    public class Message4<T1, T2, T3, T4> : IMessage
    {
        public Action<T1, T2, T3, T4> Callback
        {
            get;
            set;
        }
        public T1 Parameter1
        {
            get;
            set;
        }
        public T2 Parameter2
        {
            get;
            set;
        }
        public T3 Parameter3
        {
            get;
            set;
        }
        public T4 Parameter4
        {
            get;
            set;
        }

        public Message4()
        {
        }

        public Message4(Action<T1, T2, T3, T4> callback)
        {
            this.Callback = callback;
        }

        public Message4(Action<T1, T2, T3, T4> callback, T1 param1, T2 param2, T3 param3, T4 param4)
        {
            this.Callback = callback;
            this.Parameter1 = param1;
            this.Parameter2 = param2;
            this.Parameter3 = param3;
            this.Parameter4 = param4;
        }

        public virtual void Execute()
        {
            Action<T1, T2, T3, T4> callback = this.Callback;
            if (callback != null)
            {
                callback(this.Parameter1, this.Parameter2, this.Parameter3, this.Parameter4);
            }
        }

        public static explicit operator Message4<T1, T2, T3, T4>(Action<T1, T2, T3, T4> callback)
        {
            return new Message4<T1, T2, T3, T4>(callback);
        }
    }
}