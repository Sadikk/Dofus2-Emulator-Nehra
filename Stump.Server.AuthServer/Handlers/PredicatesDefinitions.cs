using Stump.Server.AuthServer.Network;
using System;
namespace Stump.Server.AuthServer.Handlers
{
	public static class PredicatesDefinitions
	{
		public static readonly Predicate<AuthClient> HasChoosenAccount = (AuthClient entry) => entry.Account != null;
		public static readonly Predicate<AuthClient> IsLookingOfServers = (AuthClient entry) => entry.LookingOfServers;
	}
}
