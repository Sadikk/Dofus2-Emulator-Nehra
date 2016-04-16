using Stump.Server.AuthServer.Network;
using Stump.Server.BaseServer.Handler;
using Stump.Server.BaseServer.Network;
using System;
using System.Collections.Generic;
namespace Stump.Server.AuthServer.Handlers
{
	public abstract class AuthHandlerContainer : IHandlerContainer
	{
		private static readonly Dictionary<uint, Predicate<AuthClient>> Predicates = new Dictionary<uint, Predicate<AuthClient>>();
		protected void Predicate(uint messageId, Predicate<AuthClient> predicate)
		{
			AuthHandlerContainer.Predicates.Add(messageId, predicate);
		}
		public bool CanHandleMessage(BaseClient client, uint messageId)
		{
			return !AuthHandlerContainer.Predicates.ContainsKey(messageId) || (client is AuthClient && AuthHandlerContainer.Predicates[messageId](client as AuthClient));
		}
	}
}
