using Stump.DofusProtocol.Messages;
using Stump.Server.BaseServer.Network;
using Stump.Server.WorldServer.Core.Network;

namespace Stump.Server.WorldServer.Handlers.Web
{
    public class WebHandler : WorldHandlerContainer
    {
        private WebHandler() { }

        [WorldHandler(KrosmasterAuthTokenRequestMessage.Id)]
        public static void HandleKrosmasterAuthTokenRequestMessage(WorldClient client, KrosmasterAuthTokenRequestMessage message)
        {
            WebHandler.SendKrosmasterAuthTokenMessage(client);
        }

        public static void SendKrosmasterAuthTokenMessage(IPacketReceiver client)
        {
            client.Send(new KrosmasterAuthTokenMessage("azertyuiopqsdfghjklmwxcvbnazerty"));
        }
    }
}
