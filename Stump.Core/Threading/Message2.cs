using System;

namespace Stump.Core.Threading
{
    public class Message2<T1, T2> : IMessage
    {
        public Action<T1, T2> Callback
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

        public Message2()
        {
        }

        public Message2(Action<T1, T2> callback)
        {
            this.Callback = callback;
        }

        public Message2(T1 param1, T2 param2, Action<T1, T2> callback)
        {
            this.Callback = callback;
            this.Parameter1 = param1;
            this.Parameter2 = param2;
        }

        public Message2(T1 param1, T2 param2)
        {
            this.Parameter1 = param1;
            this.Parameter2 = param2;
        }

        public virtual void Execute()
        {
            Action<T1, T2> callback = this.Callback;
            if (callback != null)
            {
                callback(this.Parameter1, this.Parameter2);
            }
        }

        public static explicit operator Message2<T1, T2>(Action<T1, T2> dele)
        {
            return new Message2<T1, T2>(dele);
        }
    }
}