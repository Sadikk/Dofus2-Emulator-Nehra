using System;

namespace Stump.Core.Threading
{
    public class Message1<T1> : IMessage
    {
        public Action<T1> Callback
        {
            get;
            set;
        }
        public T1 Parameter1
        {
            get;
            set;
        }

        public Message1()
        {
        }

        public Message1(Action<T1> callback)
        {
            this.Callback = callback;
        }

        public Message1(T1 param1, Action<T1> callback)
        {
            this.Callback = callback;
            this.Parameter1 = param1;
        }

        public virtual void Execute()
        {
            Action<T1> callback = this.Callback;
            if (callback != null)
            {
                callback(this.Parameter1);
            }
        }

        public static explicit operator Message1<T1>(Action<T1> dele)
        {
            return new Message1<T1>(dele);
        }
    }
}