using Stump.Core.Attributes;
using Stump.Core.IO;
using Stump.Server.AuthServer.Commands.Trigger;
using Stump.Server.BaseServer;
using Stump.Server.BaseServer.Commands;
using System;
using System.Collections.Generic;
namespace Stump.Server.AuthServer.IO
{
	public class AuthConsole : ConsoleBase, ICommandsUser
	{
		[Variable]
		public static string CommandPreffix = "";
		private List<KeyValuePair<string, Exception>> m_commandsError = new List<KeyValuePair<string, Exception>>();
		public List<KeyValuePair<string, Exception>> CommandsErrors
		{
			get
			{
				return this.m_commandsError;
			}
		}
		public AuthConsole()
		{
			this.m_conditionWaiter.Success += new EventHandler(this.OnConsoleKeyPressed);
		}
		protected override void Process()
		{
			this.m_conditionWaiter.Start();
		}
		private void OnConsoleKeyPressed(object sender, EventArgs e)
		{
			base.EnteringCommand = true;
			if (!AuthServer.Instance.Running)
			{
				base.EnteringCommand = false;
			}
			else
			{
				try
				{
					this.Cmd = Console.ReadLine();
				}
				catch (Exception)
				{
					base.EnteringCommand = false;
					return;
				}
				if (this.Cmd == null || !AuthServer.Instance.Running)
				{
					base.EnteringCommand = false;
				}
				else
				{
					base.EnteringCommand = false;
					lock (Console.Out)
					{
						try
						{
							if (this.Cmd.StartsWith(AuthConsole.CommandPreffix))
							{
								this.Cmd = this.Cmd.Substring(AuthConsole.CommandPreffix.Length);
								AuthServer.Instance.CommandManager.HandleCommand(new AuthConsoleTrigger(new StringStream(this.Cmd)));
							}
						}
						finally
						{
							this.m_conditionWaiter.Start();
						}
					}
				}
			}
		}
	}
}
