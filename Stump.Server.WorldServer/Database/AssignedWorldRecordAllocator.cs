using Stump.Core.Extensions;
using Stump.Server.BaseServer.Initialization;

namespace Stump.Server.WorldServer.Database
{
	internal static class AssignedWorldRecordAllocator
	{
		[Initialization(InitializationPass.First, "Register id providers")]
		public static void InitializeProviders()
		{
			System.Type[] types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				System.Type type = types[i];
				if (!type.IsAbstract && type.IsSubclassOfGeneric(typeof(AutoAssignedRecord<>)) && type != typeof(AutoAssignedRecord<>))
				{
					System.Type baseType = type.BaseType;
					while (baseType != null && baseType.GetGenericTypeDefinition() != typeof(AutoAssignedRecord<>))
					{
						baseType = baseType.BaseType;
					}
					if (!(baseType == null))
					{
						baseType.TypeInitializer.Invoke(null, null);
					}
				}
			}
		}
	}
}
