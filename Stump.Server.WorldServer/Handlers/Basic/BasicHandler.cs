using Stump.Core.Extensions;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Messages;
using Stump.DofusProtocol.Types;
using Stump.Server.BaseServer.Network;
using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Game;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using System.Linq;

namespace Stump.Server.WorldServer.Handlers.Basic
{
	public class BasicHandler : WorldHandlerContainer
	{
        private BasicHandler() { }

        [WorldHandler(BasicWhoAmIRequestMessage.Id)]
		public static void HandleBasicWhoAmIRequestMessage(WorldClient client, BasicWhoAmIRequestMessage message)
		{
			Character character = client.Character;
            client.Send(new BasicWhoIsMessage(true, true, (sbyte)character.UserGroup.Role, character.Client.WorldAccount.Nickname, character.Client.WorldAccount.Id, character.Name, (uint)character.Id, (short)character.Map.SubArea.Id, Enumerable.Empty<AbstractSocialGroupInfos>(), 0));
		}
        [WorldHandler(BasicWhoIsRequestMessage.Id)]
		public static void HandleBasicWhoIsRequestMessage(WorldClient client, BasicWhoIsRequestMessage message)
		{
			Character character = Singleton<World>.Instance.GetCharacter(message.search);
			if (character == null)
			{
				client.Send(new BasicWhoIsNoMatchMessage(message.search));
			}
			else
			{
                client.Send(new BasicWhoIsMessage(true, true, (sbyte)character.UserGroup.Role, character.Client.WorldAccount.Nickname, character.Client.WorldAccount.Id, character.Name, (uint)character.Id, (short)character.Map.SubArea.Id, Enumerable.Empty<AbstractSocialGroupInfos>(), 0));
			}
		}

		public static void SendTextInformationMessage(IPacketReceiver client, TextInformationTypeEnum msgType, short msgId, params string[] arguments)
		{
			client.Send(new TextInformationMessage((sbyte)msgType, (ushort)msgId, arguments));
		}
		public static void SendTextInformationMessage(IPacketReceiver client, TextInformationTypeEnum msgType, short msgId, params object[] arguments)
		{
            client.Send(new TextInformationMessage((sbyte)msgType, (ushort)msgId, 
				from entry in arguments
				select entry.ToString()));
		}
		public static void SendTextInformationMessage(IPacketReceiver client, TextInformationTypeEnum msgType, short msgId)
		{
            client.Send(new TextInformationMessage((sbyte)msgType, (ushort)msgId, new string[0]));
		}
		public static void SendSystemMessageDisplayMessage(IPacketReceiver client, bool hangUp, short msgId, System.Collections.Generic.IEnumerable<string> arguments)
		{
            client.Send(new SystemMessageDisplayMessage(hangUp, (ushort)msgId, arguments));
		}
		public static void SendSystemMessageDisplayMessage(IPacketReceiver client, bool hangUp, short msgId, params object[] arguments)
		{
            client.Send(new SystemMessageDisplayMessage(hangUp, (ushort)msgId, 
				from entry in arguments
				select entry.ToString()));
		}
		public static void SendBasicTimeMessage(IPacketReceiver client)
		{
            client.Send(new BasicTimeMessage(System.DateTime.Now.GetUnixTimeStampLong(), Settings.TimeZoneOffset));
		}
		public static void SendBasicNoOperationMessage(IPacketReceiver client)
		{
			client.Send(new BasicNoOperationMessage());
		}
	}
}
