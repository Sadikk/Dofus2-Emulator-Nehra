using Stump.Core.Extensions;
using Stump.Server.BaseServer.Initialization;
using System;
using System.Reflection;
namespace Stump.Server.BaseServer.Database
{
	public static class DataManagerAllocator
	{
		public static Assembly Assembly;
		[Initialization(InitializationPass.First, "Initialize 'DataManagers'")]
		public static void Initialize()
		{
			Type[] types = DataManagerAllocator.Assembly.GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				Type type = types[i];
				if (!type.IsAbstract && type.IsSubclassOfGeneric(typeof(DataManager<>)) && !(type == typeof(DataManager<>)))
				{
					MethodInfo method = type.GetMethod("Initialize", BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
					if (method.GetCustomAttribute<InitializationAttribute>(true) == null)
					{
						object value = type.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy).GetValue(null, new object[0]);
						method.Invoke(value, new object[0]);
					}
				}
			}
		}
	}
}
