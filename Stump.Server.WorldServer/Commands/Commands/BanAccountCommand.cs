using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.BaseServer.IPC;
using Stump.Server.BaseServer.IPC.Messages;
using Stump.Server.WorldServer.Core.IPC;
using Stump.Server.WorldServer.Core.Network;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class BanAccountCommand : CommandBase
	{
		public BanAccountCommand()
		{
			base.Aliases = new string[]
			{
				"banacc"
			};
			base.RequiredRole = RoleEnum.GameMaster;
			base.Description = "Ban an account";
			base.AddParameter<string>("account", "account", "Account login", null, false, null);
			base.AddParameter<int>("time", "time", "Ban duration (in minutes)", 0, true, null);
			base.AddParameter<string>("reason", "r", "Reason of ban", "No reason", false, null);
			base.AddParameter<bool>("life", "l", "Specify a life ban", false, true, null);
		}
		public override void Execute(TriggerBase trigger)
		{
			string accountName = trigger.Get<string>("account");
			string banReason = trigger.Get<string>("reason");
			if (!IPCAccessor.Instance.IsConnected)
			{
				trigger.ReplyError("IPC service not operational !");
			}
			else
			{
				BanAccountMessage banAccountMessage = new BanAccountMessage
				{
					AccountName = accountName,
					BanReason = banReason
				};
				WorldClient worldClient = trigger.GetSource() as WorldClient;
				if (worldClient != null)
				{
					banAccountMessage.BannerAccountId = new int?(worldClient.Account.Id);
				}
				if (trigger.IsArgumentDefined("time"))
				{
					banAccountMessage.BanEndDate = new System.DateTime?(System.DateTime.Now + System.TimeSpan.FromMinutes((double)trigger.Get<int>("time")));
				}
				else
				{
					if (!trigger.IsArgumentDefined("life"))
					{
						trigger.ReplyError("No ban duration given");
						return;
					}
					banAccountMessage.BanEndDate = null;
				}
				IPCAccessor.Instance.SendRequest(banAccountMessage, delegate(CommonOKMessage ok)
				{
					trigger.Reply("Account {0} banned", new object[]
					{
						accountName
					});
				}, delegate(IPCErrorMessage error)
				{
					trigger.ReplyError("Account {0} not banned : {1}", new object[]
					{
						accountName,
						error.Message
					});
				});
			}
		}
	}
}
