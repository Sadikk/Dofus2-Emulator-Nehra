using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.BaseServer.IPC;
using Stump.Server.BaseServer.IPC.Messages;
using Stump.Server.WorldServer.Commands.Commands.Patterns;
using Stump.Server.WorldServer.Core.IPC;
using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class JailCommand : TargetCommand
	{
		public JailCommand()
		{
			base.Aliases = new string[]
			{
				"jail"
			};
			base.RequiredRole = RoleEnum.Moderator;
			base.Description = "Jail a character";
			base.AddTargetParameter(false, "Defined target");
			base.AddParameter<int>("time", "time", "Ban duration (in minutes)", 30, false, null);
			base.AddParameter<string>("reason", "r", "Reason of ban", "No reason", false, null);
		}
		public override void Execute(TriggerBase trigger)
		{
			string reason = trigger.Get<string>("reason");
			if (!IPCAccessor.Instance.IsConnected)
			{
				trigger.ReplyError("IPC service not operational !");
			}
			else
			{
				Character[] targets = base.GetTargets(trigger);
				for (int i = 0; i < targets.Length; i++)
				{
					Character target = targets[i];
					BanAccountMessage banAccountMessage = new BanAccountMessage
					{
						AccountId = new int?(target.Account.Id),
						BanReason = reason
					};
					WorldClient worldClient = trigger.GetSource() as WorldClient;
					if (worldClient != null)
					{
						banAccountMessage.BannerAccountId = new int?(worldClient.Account.Id);
					}
					if (trigger.IsArgumentDefined("time"))
					{
						int num = trigger.Get<int>("time");
						if (num > 1440 && trigger.UserRole == RoleEnum.GameMaster_Padawan)
						{
							num = 1440;
						}
						banAccountMessage.BanEndDate = new System.DateTime?(System.DateTime.Now + System.TimeSpan.FromMinutes((double)num));
					}
					else
					{
						if (!trigger.IsArgumentDefined("life") || trigger.UserRole == RoleEnum.GameMaster_Padawan)
						{
							trigger.ReplyError("No ban duration given");
							break;
						}
						banAccountMessage.BanEndDate = null;
					}
					target.TeleportToJail();
					target.Account.IsJailed = true;
					banAccountMessage.Jailed = true;
					IPCAccessor.Instance.SendRequest(banAccountMessage, delegate(CommonOKMessage ok)
					{
						trigger.Reply("Account {0} jailed for {1} minutes. Reason : {2}", new object[]
						{
							target.Account.Login,
							trigger.Get<int>("time"),
							reason
						});
					}, delegate(IPCErrorMessage error)
					{
						trigger.ReplyError("Account {0} not jailed : {1}", new object[]
						{
							target.Account.Login,
							error.Message
						});
					});
				}
			}
		}
	}
}
