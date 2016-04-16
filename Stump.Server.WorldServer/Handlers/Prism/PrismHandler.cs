using Stump.DofusProtocol.Messages;
using Stump.DofusProtocol.Types;
using Stump.Server.BaseServer.Network;
using Stump.Server.WorldServer.Core.Network;

namespace Stump.Server.WorldServer.Handlers.Prism
{
    public class PrismHandler : WorldHandlerContainer
    {
        private PrismHandler() { }

        [WorldHandler(PrismsListRegisterMessage.Id)]
        public static void HandlePrismsListRegisterMessage(WorldClient client, PrismsListRegisterMessage message)
        {
            PrismHandler.SendPrismsListMessage(client);
        }

        public static void SendPrismsListMessage(IPacketReceiver client)
        {
            client.Send(new PrismsListMessage(new PrismSubareaEmptyInfo[0]));
        }
    }
}
