using Stump.Core.IO;
using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer;
using Stump.Server.BaseServer.Commands;
using Stump.Server.BaseServer.Network;
using Stump.Server.WorldServer.Core.IO;

namespace Stump.Server.WorldServer.Commands.Trigger
{
	public class WorldConsoleTrigger : TriggerBase
	{
		public override bool CanFormat
		{
			get
			{
				return false;
			}
		}
		public override ICommandsUser User
		{
			get
			{
				return ServerBase<WorldServer>.Instance.ConsoleInterface as WorldConsole;
			}
		}
		public WorldConsoleTrigger(StringStream args) : base(args, RoleEnum.Administrator)
		{
		}
		public WorldConsoleTrigger(string args) : base(args, RoleEnum.Administrator)
		{
		}
		public override void Reply(string text)
		{
			System.Console.WriteLine(" " + text);
		}
		public override BaseClient GetSource()
		{
			return null;
		}
	}
}
