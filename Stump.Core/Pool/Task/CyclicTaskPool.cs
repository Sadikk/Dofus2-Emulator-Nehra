using Stump.Core.Attributes;
using Stump.Core.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Stump.Core.Pool.Task
{
    public class CyclicTaskPool
    {
        private readonly List<CyclicTask> m_cyclicTasks = new List<CyclicTask>();
        private readonly object m_sync = new object();

        public void Initialize(IEnumerable<Assembly> assemblies)
        {
            foreach (Assembly current in assemblies)
            {
                Type[] types = current.GetTypes();
                for (int i = 0; i < types.Length; i++)
                {
                    Type type = types[i];
                    MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    for (int j = 0; j < methods.Length; j++)
                    {
                        MethodInfo methodInfo = methods[j];
                        Cyclic cyclic = methodInfo.GetCustomAttributes(typeof(Cyclic), false).SingleOrDefault<object>() as Cyclic;
                        if (cyclic != null)
                        {
                            this.RegisterCyclicTask(Delegate.CreateDelegate(methodInfo.GetActionType(), methodInfo) as Action, cyclic.Time);
                        }
                    }
                }
            }
        }

        public void Clear()
        {
            CyclicTask[] array = this.m_cyclicTasks.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                CyclicTask cyclicTask = array[i];
                this.UnregisterCyclicTask(cyclicTask);
            }
        }

        public void RegisterCyclicTask(Action method, int interval)
        {
            this.RegisterCyclicTask(new CyclicTask(method, TimeSpan.FromSeconds((double)interval)));
        }

        public void RegisterCyclicTask(Action method, int interval, int waitDelay = 0, int? callsLimit = null, CyclicTask.Predicate predicate = null)
        {
            this.RegisterCyclicTask(new CyclicTask(method, TimeSpan.FromSeconds((double)interval)));
        }

        public void RegisterCyclicTask(CyclicTask cyclicTask)
        {
            lock (this.m_sync)
            {
                this.m_cyclicTasks.Add(cyclicTask);
                cyclicTask.TaskEnded += new Action<CyclicTask>(this.UnregisterCyclicTask);
                cyclicTask.Start();
            }
        }

        public void UnregisterCyclicTask(Action method)
        {
            CyclicTask[] array = (
                from entry in this.m_cyclicTasks
                where entry.Action == method
                select entry).ToArray<CyclicTask>();
            for (int i = 0; i < array.Length; i++)
            {
                CyclicTask cyclicTask = array[i];
                this.UnregisterCyclicTask(cyclicTask);
            }
        }

        public void UnregisterCyclicTask(CyclicTask cyclicTask)
        {
            lock (this.m_sync)
            {
                cyclicTask.Stop();
                this.m_cyclicTasks.Remove(cyclicTask);
            }
        }
    }
}