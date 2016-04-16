using Stump.Core.Attributes;
using Stump.Core.Reflection;
using Stump.Server.BaseServer;
using Stump.Server.BaseServer.Commands;
using Stump.Server.WorldServer.Commands.Trigger;
using System.Diagnostics;
namespace Stump.Server.WorldServer.Core.IO
{
	public class WorldConsole : ConsoleBase, ICommandsUser
	{
		[Variable]
		public static string CommandPreffix = "";
		private System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, System.Exception>> m_commandsError = new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, System.Exception>>();
		public System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, System.Exception>> CommandsErrors
		{
			get
			{
				return this.m_commandsError;
			}
		}
		public WorldConsole()
		{
			this.m_conditionWaiter.Success += new System.EventHandler(this.OnConsoleKeyPressed);
		}
		protected override void Process()
		{
			this.m_conditionWaiter.Start();
		}
		private void OnConsoleKeyPressed(object sender, System.EventArgs e)
		{
			base.EnteringCommand = true;
			if (!ServerBase<WorldServer>.Instance.Running)
			{
				base.EnteringCommand = false;
			}
			else
			{
				try
				{
					this.Cmd = System.Console.ReadLine();
				}
				catch (System.Exception)
				{
					base.EnteringCommand = false;
					return;
				}
				if (this.Cmd == null || !ServerBase<WorldServer>.Instance.Running)
				{
					base.EnteringCommand = false;
				}
				else
				{
					base.EnteringCommand = false;
					lock (System.Console.Out)
					{
						try
						{
							if (this.Cmd.StartsWith(WorldConsole.CommandPreffix))
							{
								this.Cmd = this.Cmd.Substring(WorldConsole.CommandPreffix.Length);
								Stopwatch stopwatch = new Stopwatch();
								stopwatch.Start();
								Singleton<CommandManager>.Instance.HandleCommand(new WorldConsoleTrigger(this.Cmd));
								stopwatch.Stop();
								Debug.WriteLine(stopwatch.ElapsedMilliseconds + " ms");
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
