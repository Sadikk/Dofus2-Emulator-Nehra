using Stump.Core.IO;
using Stump.DofusProtocol.Enums;
using Stump.Server.AuthServer.IO;
using Stump.Server.BaseServer;
using Stump.Server.BaseServer.Commands;
using Stump.Server.BaseServer.Network;
using System;

namespace Stump.Server.AuthServer.Commands.Trigger
{
	public class AuthConsoleTrigger : TriggerBase
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
				return AuthServer.Instance.ConsoleInterface as AuthConsole;
			}
		}
		public AuthConsoleTrigger(StringStream args) : base(args, RoleEnum.Administrator)
		{
		}
		public AuthConsoleTrigger(string args) : base(args, RoleEnum.Administrator)
		{
		}
		public override void Reply(string text)
		{
			Console.WriteLine(" " + text);
		}
		public override BaseClient GetSource()
		{
			return null;
		}
	}
}
