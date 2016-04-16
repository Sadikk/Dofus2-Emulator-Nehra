using Stump.Core.IO;
using Stump.Server.BaseServer;
using Stump.Server.BaseServer.Commands;
using Stump.Server.WorldServer.Commands.Trigger;
using System;

namespace Stump.Server.WorldServer.Core.IO
{
	public class WorldVirtualConsole : ConsoleBase, ICommandsUser
	{
		private readonly System.Collections.Generic.List<string> m_commands = new System.Collections.Generic.List<string>();
		private readonly System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, System.Exception>> m_commandsError = new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, System.Exception>>();
		public System.Collections.Generic.List<string> Commands
		{
			get
			{
				return this.m_commands;
			}
		}
		public System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, System.Exception>> CommandsErrors
		{
			get
			{
				return this.m_commandsError;
			}
		}
		public void EnterCommand(string Cmd, Action<bool, string> callback)
		{
			if (ServerBase<WorldServer>.Instance.Running && !(Cmd == ""))
			{
				ServerBase<WorldServer>.Instance.CommandManager.HandleCommand(new WorldVirtualConsoleTrigger(new StringStream(Cmd), callback));
			}
		}
	}
}
