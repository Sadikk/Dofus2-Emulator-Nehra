using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Stump.Core.Reflection
{
    internal class DynamicSwitchExecutor
    {
        private class SwitchCase
        {
            public int Value
            {
                get;
                private set;
            }
            public object Instance
            {
                get;
                private set;
            }
            public MethodInfo Method
            {
                get;
                private set;
            }

            public SwitchCase(int value, object instance, MethodInfo method)
            {
                this.Value = value;
                this.Instance = instance;
                this.Method = method;
            }
        }

        private readonly Type m_type;
        private readonly List<DynamicSwitchExecutor.SwitchCase> m_cases = new List<DynamicSwitchExecutor.SwitchCase>();

        public void Add(int value, object instance, MethodInfo method)
        {
            this.m_cases.Add(new DynamicSwitchExecutor.SwitchCase(value, instance, method));
        }

        public Tuple<Dictionary<int, int>, Delegate> Build(params Type[] delegParams)
        {
            Dictionary<int, int> dictionary = new Dictionary<int, int>();
            DynamicMethod dynamicMethod = new DynamicMethod(string.Empty, null, delegParams);
            ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
            Label label = iLGenerator.DefineLabel();
            Label[] array = new Label[this.m_cases.Count];
            for (int i = 0; i < this.m_cases.Count; i++)
            {
                array[i] = iLGenerator.DefineLabel();
                dictionary.Add(this.m_cases[i].Value, i);
            }
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Switch, array);
            for (int i = 0; i < this.m_cases.Count; i++)
            {
                iLGenerator.MarkLabel(array[i]);
                Type[] array2 = (
                    from p in this.m_cases[i].Method.GetParameters()
                    select p.ParameterType).ToArray<Type>();
                if (array2.Length != delegParams.Length - 1)
                {
                    throw new Exception("Le nombre d'arguments passé ne correspond pas au nombre d'arguments de la methode");
                }
                for (int j = 1; j < delegParams.Length; j++)
                {
                    iLGenerator.Emit(OpCodes.Ldarg, j);
                    if (delegParams[j] != array2[j - 1])
                    {
                        if (!array2[j - 1].IsSubclassOf(delegParams[j]))
                        {
                            throw new Exception("Impossible de réaliser un cast vers un object de ce type");
                        }
                        iLGenerator.Emit(array2[j - 1].IsClass ? OpCodes.Castclass : OpCodes.Unbox, array2[j - 1]);
                    }
                }
                iLGenerator.Emit(OpCodes.Call, this.m_cases[i].Method);
                iLGenerator.Emit(OpCodes.Br_S, label);
            }
            iLGenerator.MarkLabel(label);
            iLGenerator.Emit(OpCodes.Ret);
            return new Tuple<Dictionary<int, int>, Delegate>(dictionary, dynamicMethod.CreateDelegate(Expression.GetActionType(delegParams)));
        }
    }
}