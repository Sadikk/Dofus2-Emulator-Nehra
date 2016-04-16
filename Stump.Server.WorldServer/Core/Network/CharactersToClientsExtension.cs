using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using System.Linq;
namespace Stump.Server.WorldServer.Core.Network
{
	public static class CharactersToClientsExtension
	{
		public static WorldClientCollection ToClients(this System.Collections.Generic.IEnumerable<Character> characters)
		{
			return new WorldClientCollection(
				from x in characters
				select x.Client);
		}
	}
}
