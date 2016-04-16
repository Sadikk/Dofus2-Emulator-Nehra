using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.BaseServer.IPC;
using Stump.Server.BaseServer.IPC.Messages;
using Stump.Server.WorldServer.Commands.Commands.Patterns;
using Stump.Server.WorldServer.Core.IPC;
using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Game;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using System;

namespace Stump.Server.WorldServer.Commands.Commands
{
    public class BannedCommand : TargetCommand
    {
        public BannedCommand()
        {
            base.Aliases = new string[]
            {
                "ban"
            };
            base.RequiredRole = RoleEnum.GameMaster;
            base.Description = "Bannir un joueur";
            base.AddTargetParameter(false, "Defined target");
            base.AddParameter<string>("reason", "r", "Reason of ban", null, false, null);
            base.AddParameter<bool>("life", "l", "Specify a life ban", false, true, null);
            base.AddParameter<bool>("ip", "ip", "Also ban the ip", false, true, null);
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

                    else
                    {
                        if (!trigger.IsArgumentDefined("life") || trigger.UserRole == RoleEnum.GameMaster_Padawan)
                        {
                            trigger.ReplyError("Ajoute le temps du Ban");
                            break;
                        }
                        banAccountMessage.BanEndDate = null;
                    }
                    banAccountMessage.Jailed = false;
                    target.Client.Disconnect();
                    IPCAccessor.Instance.SendRequest(banAccountMessage, delegate (CommonOKMessage ok)
                    {
                        World.Instance.SendAnnounce(String.Format("Le joueur {0} a été banni.", target.Name), System.Drawing.Color.Red);
                    }, delegate (IPCErrorMessage error)
                    {
                        trigger.ReplyError("Account {0} not banned : {1}", new object[]
                          {
                           target.Account.Login,
                           error.Message
                          });
                    });
                    if (!trigger.IsArgumentDefined("ip"))
                    {
                        break;
                    }
                    BanIPMessage message = new BanIPMessage
                    {
                        IPRange = target.Client.IP,
                        BanReason = reason,
                        BannerAccountId = banAccountMessage.BannerAccountId
                    };
                    IPCAccessor.Instance.SendRequest(message, delegate (CommonOKMessage ok)
                    {
                        trigger.Reply("IP {0} banned", new object[]
                          {
                           target.Client.IP
                          });
                    }, delegate (IPCErrorMessage error)
                    {
                        trigger.ReplyError("IP {0} not banned : {1}", new object[]
                          {
                           target.Client.IP,
                           error.Message
                          });
                    });
                }
            }
        }
    }
}