using System;

namespace Stump.Core.Threading
{
    public class Message3<T1, T2, T3> : IMessage
    {
        public Action<T1, T2, T3> Callback
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

        public Message3()
        {
        }

        public Message3(Action<T1, T2, T3> callback)
        {
            this.Callback = callback;
        }

        public Message3(T1 param1, T2 param2, T3 param3, Action<T1, T2, T3> callback)
        {
            this.Callback = callback;
            this.Parameter1 = param1;
            this.Parameter2 = param2;
            this.Parameter3 = param3;
        }

        public virtual void Execute()
        {
            Action<T1, T2, T3> callback = this.Callback;
            if (callback != null)
            {
                callback(this.Parameter1, this.Parameter2, this.Parameter3);
            }
        }

        public static explicit operator Message3<T1, T2, T3>(Action<T1, T2, T3> dele)
        {
            return new Message3<T1, T2, T3>(dele);
        }
    }
}