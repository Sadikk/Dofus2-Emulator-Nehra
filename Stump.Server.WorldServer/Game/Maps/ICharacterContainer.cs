using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Maps
{
	public interface ICharacterContainer
	{
		WorldClientCollection Clients
		{
			get;
		}
		System.Collections.Generic.IEnumerable<Character> GetAllCharacters();
		void ForEach(System.Action<Character> action);
	}
}
