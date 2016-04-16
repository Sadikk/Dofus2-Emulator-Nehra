using Stump.DofusProtocol.Messages;
using Stump.Server.BaseServer.Network;
using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Game.Maps;

namespace Stump.Server.WorldServer.Handlers.PvP
{
	public class PvPHandler : WorldHandlerContainer
	{
        private PvPHandler() { }

        [WorldHandler(SetEnablePVPRequestMessage.Id)]
		public static void HandleSetEnablePVPRequestMessage(WorldClient client, SetEnablePVPRequestMessage message)
		{
			client.Character.TogglePvPMode(message.enable);
		}

		public static void SendAlignmentRankUpdateMessage(IPacketReceiver client)
		{
			client.Send(new AlignmentRankUpdateMessage(1, false));
		}
        public static void SendUpdateMapPlayersAgressableStatusMessage(IPacketReceiver client, Map map)
        {
            client.Send(new UpdateMapPlayersAgressableStatusMessage(new uint[0], new sbyte[0]));
        }
	}
}
