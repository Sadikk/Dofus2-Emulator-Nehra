using Stump.Core.IO;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Messages;
using Stump.Server.BaseServer;
using Stump.Server.BaseServer.Network;
using Stump.Server.WorldServer.Commands.Trigger;
using Stump.Server.WorldServer.Core.Network;

namespace Stump.Server.WorldServer.Handlers.Authorized
{
	public class AuthorizedHandler : WorldHandlerContainer
	{
		[WorldHandler(5662u)]
		public static void HandleAdminQuietCommandMessage(WorldClient client, AdminQuietCommandMessage message)
		{
			if (client.UserGroup.Role >= RoleEnum.GameMaster_Padawan)
			{
				string[] array = message.content.Split(new char[]
				{
					' '
				});
				string text = array[0];
				string text2 = text;
				if (text2 != null)
				{
					if (!(text2 == "look"))
					{
						if (text2 == "moveto")
						{
							string arg = array[1];
							ServerBase<WorldServer>.Instance.CommandManager.HandleCommand(new TriggerConsole(string.Format("go {0}", arg), client.Character));
						}
					}
					else
					{
						ServerBase<WorldServer>.Instance.CommandManager.HandleCommand(new TriggerConsole("look " + array[2], client.Character));
					}
				}
			}
		}
		[WorldHandler(76u)]
		public static void HandleAdminCommandMessage(WorldClient client, AdminCommandMessage message)
		{
			if (client.UserGroup.Role >= RoleEnum.GameMaster_Padawan && client.Character != null)
			{
				ServerBase<WorldServer>.Instance.CommandManager.HandleCommand(new TriggerConsole(new StringStream(message.content), client.Character));
			}
		}
		public static void SendConsoleMessage(IPacketReceiver client, string text)
		{
			AuthorizedHandler.SendConsoleMessage(client, ConsoleMessageTypeEnum.CONSOLE_TEXT_MESSAGE, text);
		}
		public static void SendConsoleMessage(IPacketReceiver client, ConsoleMessageTypeEnum type, string text)
		{
			client.Send(new ConsoleMessage((sbyte)type, text));
		}
	}
}
