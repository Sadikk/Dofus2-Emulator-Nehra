using Stump.Core.Reflection;
using Stump.DofusProtocol.Messages;
using Stump.Server.BaseServer.Network;
using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Game;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Interactives;
using Stump.Server.WorldServer.Game.Interactives.Skills;
using Stump.Server.WorldServer.Game.Maps;

namespace Stump.Server.WorldServer.Handlers.Interactives
{
	public class InteractiveHandler : WorldHandlerContainer
	{
		[WorldHandler(5001u)]
		public static void HandleInteractiveUseRequestMessage(WorldClient client, InteractiveUseRequestMessage message)
		{
            client.Character.Map.UseInteractiveObject(client.Character, (int)message.elemId, (int)message.skillInstanceUid);
		}
		[WorldHandler(6112u)]
		public static void HandleInteractiveUseEndedMessage(WorldClient client, InteractiveUseEndedMessage message)
		{
		}
		[WorldHandler(5961u)]
		public static void HandleTeleportRequestMessage(WorldClient client, TeleportRequestMessage message)
		{
			if (client.Character.IsInZaapDialog())
			{
				Map map = Singleton<World>.Instance.GetMap(message.mapId);
				if (!(map == null))
				{
					client.Character.ZaapDialog.Teleport(map);
				}
			}
		}
		public static void SendInteractiveUsedMessage(IPacketReceiver client, Character user, InteractiveObject interactiveObject, Skill skill)
		{
            client.Send(new InteractiveUsedMessage((uint)user.Id, (uint)interactiveObject.Id, (ushort)skill.Id, (ushort)skill.GetDuration(user)));
		}
	}
}
