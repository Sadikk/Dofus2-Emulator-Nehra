using Stump.Core.IO;
using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.BaseServer.Network;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Handlers.Authorized;

namespace Stump.Server.WorldServer.Commands.Trigger
{
	public class TriggerConsole : GameTrigger
	{
		public override ICommandsUser User
		{
			get
			{
				return base.Character;
			}
		}
		public TriggerConsole(StringStream args, Character character) : base(args, character)
		{
		}
		public TriggerConsole(string args, Character character) : base(args, character)
		{
		}
		public override void Reply(string text)
		{
			AuthorizedHandler.SendConsoleMessage(base.Character.Client, text);
		}
		public override void ReplyError(string message)
		{
			AuthorizedHandler.SendConsoleMessage(base.Character.Client, ConsoleMessageTypeEnum.CONSOLE_ERR_MESSAGE, message);
		}
		public override BaseClient GetSource()
		{
			return (base.Character != null) ? base.Character.Client : null;
		}
	}
}
