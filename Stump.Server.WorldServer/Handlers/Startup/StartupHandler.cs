using Stump.DofusProtocol.Messages;
using Stump.Server.BaseServer.Network;
using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Game.Accounts.Startup;
using System.Linq;
namespace Stump.Server.WorldServer.Handlers.Startup
{
	public class StartupHandler : WorldHandlerContainer
	{
		[WorldHandler(1303u, ShouldBeLogged = false, IsGamePacket = false)]
		public static void HandleStartupActionsObjetAttributionMessage(WorldClient client, StartupActionsObjetAttributionMessage message)
		{
		}
		public static void SendStartupActionsListMessage(IPacketReceiver client, System.Collections.Generic.IEnumerable<StartupAction> actions)
		{
			client.Send(new StartupActionsListMessage(
				from entry in actions
				select entry.GetStartupActionAddObject()));
		}
		public static void SendStartupActionFinishedMessage(IPacketReceiver client, StartupAction action, bool success)
		{
			client.Send(new StartupActionFinishedMessage(success, true, action.Id));
		}
	}
}
