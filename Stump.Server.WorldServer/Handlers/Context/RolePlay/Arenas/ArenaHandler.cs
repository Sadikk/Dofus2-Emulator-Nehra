using Stump.Core.Reflection;
using Stump.DofusProtocol.Messages;
using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Game.Arenas;

namespace Stump.Server.WorldServer.Handlers.Context.RolePlay.Arenas
{
    public class ArenaHandler : WorldHandlerContainer
    {
        private ArenaHandler() { }

        [WorldHandler(GameRolePlayArenaRegisterMessage.Id)]
        public static void HandleGameRolePlayArenaRegisterMessage(WorldClient client, GameRolePlayArenaRegisterMessage message)
        {
            var manager = Singleton<ArenaManager>.Instance;

        }
    }
}
