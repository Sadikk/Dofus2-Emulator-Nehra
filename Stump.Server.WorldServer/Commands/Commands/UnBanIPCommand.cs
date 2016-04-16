using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.BaseServer.IPC;
using Stump.Server.BaseServer.IPC.Messages;
using Stump.Server.WorldServer.Core.IPC;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class UnBanIPCommand : CommandBase
	{
		public UnBanIPCommand()
		{
			base.Aliases = new string[]
			{
				"unbanip"
			};
			base.RequiredRole = RoleEnum.GameMaster;
			base.Description = "Unban an ip";
			base.AddParameter<string>("ip", "ip", "The ip to unban", null, false, null);
		}
		public override void Execute(TriggerBase trigger)
		{
			string ip = trigger.Get<string>("ip");
			if (!IPCAccessor.Instance.IsConnected)
			{
				trigger.ReplyError("IPC service not operational !");
			}
			else
			{
				IPCAccessor.Instance.SendRequest(new UnBanIPMessage(ip), delegate(CommonOKMessage ok)
				{
					trigger.Reply("IP {0} unbanned", new object[]
					{
						ip
					});
				}, delegate(IPCErrorMessage error)
				{
					trigger.ReplyError("IP {0} not unbanned : {1}", new object[]
					{
						ip,
						error.Message
					});
				});
			}
		}
	}
}
