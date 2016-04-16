using Stump.DofusProtocol.Messages;
using Stump.DofusProtocol.Types;
using Stump.Server.BaseServer.Network;
using Stump.Server.WorldServer.Core.Network;

namespace Stump.Server.WorldServer.Handlers.Initialization
{
	public class InitializationHandler : WorldHandlerContainer
	{
		public static void SendOnConnectionEventMessage(IPacketReceiver client, sbyte eventType)
		{
			client.Send(new OnConnectionEventMessage(eventType));
		}
		public static void SendSetCharacterRestrictionsMessage(WorldClient client)
		{
			client.Send(new SetCharacterRestrictionsMessage(client.Character.Id, new ActorRestrictionsInformations(false, false, false, false, false, false, false, false, true, false, false, false, false, true, true, true, false, false, false, false, false)));
		}
	}
}
