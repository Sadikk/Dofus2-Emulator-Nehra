using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.BaseServer.IPC;
using Stump.Server.BaseServer.IPC.Messages;
using Stump.Server.BaseServer.Network;
using Stump.Server.WorldServer.Core.IPC;
using Stump.Server.WorldServer.Core.Network;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class BanIpCommand : CommandBase
	{
		public BanIpCommand()
		{
			base.Aliases = new string[]
			{
				"banip"
			};
			base.RequiredRole = RoleEnum.GameMaster;
			base.Description = "Ban an ip";
			base.AddParameter<string>("ip", "ip", "The ip to ban", null, false, null);
			base.AddParameter<int>("time", "time", "Ban duration (in minutes)", 0, true, null);
			base.AddParameter<string>("reason", "r", "Reason of ban", "No reason", false, null);
			base.AddParameter<bool>("life", "l", "Specify a life ban", false, true, null);
		}
		public override void Execute(TriggerBase trigger)
		{
			string ip = trigger.Get<string>("ip");
			string banReason = trigger.Get<string>("reason");
			if (!IPCAccessor.Instance.IsConnected)
			{
				trigger.ReplyError("IPC service not operational !");
			}
			else
			{
				try
				{
					IPAddressRange.Parse(ip);
				}
				catch
				{
					trigger.ReplyError("IP format '{0}' incorrect", new object[]
					{
						ip
					});
					return;
				}
				BanIPMessage banIPMessage = new BanIPMessage
				{
					IPRange = ip,
					BanReason = banReason
				};
				WorldClient worldClient = trigger.GetSource() as WorldClient;
				if (worldClient != null)
				{
					banIPMessage.BannerAccountId = new int?(worldClient.Account.Id);
				}
				if (trigger.IsArgumentDefined("time"))
				{
					banIPMessage.BanEndDate = new System.DateTime?(System.DateTime.Now + System.TimeSpan.FromMinutes((double)trigger.Get<int>("time")));
				}
				else
				{
					if (!trigger.IsArgumentDefined("life"))
					{
						trigger.ReplyError("No ban duration given");
						return;
					}
					banIPMessage.BanEndDate = null;
				}
				IPCAccessor.Instance.SendRequest(banIPMessage, delegate(CommonOKMessage ok)
				{
					trigger.Reply("IP {0} banned", new object[]
					{
						ip
					});
				}, delegate(IPCErrorMessage error)
				{
					trigger.ReplyError("IP {0} not banned : {1}", new object[]
					{
						ip,
						error.Message
					});
				});
			}
		}
	}
}
