using System;
using System.Collections.Generic;
namespace Stump.Server.BaseServer.Network
{
	public static class ClientExtensions
	{
		public static void ForEach<T>(this IEnumerable<T> clients, Action<T> action) where T : BaseClient
		{
			foreach (T current in clients)
			{
				action(current);
			}
		}
	}
}
