using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.BaseServer.IPC;
using Stump.Server.BaseServer.IPC.Messages;
using Stump.Server.WorldServer.Core.IPC;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class UnBanAccountCommand : CommandBase
	{
		public UnBanAccountCommand()
		{
			base.Aliases = new string[]
			{
				"unbanacc"
			};
			base.RequiredRole = RoleEnum.GameMaster;
			base.Description = "Unban an account";
			base.AddParameter<string>("character", "account", "Account login", null, false, null);
		}
		public override void Execute(TriggerBase trigger)
		{
			string accountName = trigger.Get<string>("account");
			if (!IPCAccessor.Instance.IsConnected)
			{
				trigger.ReplyError("IPC service not operational !");
			}
			else
			{
				IPCAccessor.Instance.SendRequest(new UnBanAccountMessage(accountName), delegate(CommonOKMessage ok)
				{
					trigger.Reply("Account {0} unbanned", new object[]
					{
						accountName
					});
				}, delegate(IPCErrorMessage error)
				{
					trigger.ReplyError("Account {0} not unbanned : {1}", new object[]
					{
						accountName,
						error.Message
					});
				});
			}
		}
	}
}
