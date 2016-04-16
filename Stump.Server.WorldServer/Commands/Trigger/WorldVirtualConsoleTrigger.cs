using Stump.Core.IO;
using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer;
using Stump.Server.BaseServer.Commands;
using Stump.Server.BaseServer.Network;
using System;
namespace Stump.Server.WorldServer.Commands.Trigger
{
	internal class WorldVirtualConsoleTrigger : TriggerBase
	{
		public Action<bool, string> Callback
		{
			get;
			private set;
		}
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
				return ServerBase<WorldServer>.Instance.VirtualConsoleInterface;
			}
		}
		public WorldVirtualConsoleTrigger(StringStream args) : base(args, RoleEnum.Administrator)
		{
		}
		public WorldVirtualConsoleTrigger(string args) : base(args, RoleEnum.Administrator)
		{
		}
		public WorldVirtualConsoleTrigger(StringStream args, Action<bool, string> callback) : base(args, RoleEnum.Administrator)
		{
			this.Callback = callback;
		}
		public override void Reply(string text)
		{
			if (this.Callback != null)
			{
				this.Callback(true, text);
			}
		}
		public override void ReplyError(string message)
		{
			if (this.Callback != null)
			{
				this.Callback(false, "(Error) " + message);
			}
			else
			{
				this.Reply("(Error) " + message);
			}
		}
		public override BaseClient GetSource()
		{
			throw new System.NotImplementedException();
		}
	}
}
