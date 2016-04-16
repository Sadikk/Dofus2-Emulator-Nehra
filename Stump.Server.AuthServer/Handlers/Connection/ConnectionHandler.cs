using Stump.Core.Cryptography;
using Stump.Core.Extensions;
using Stump.Core.IO;
using Stump.Core.Reflection;
using Stump.Core.Threading;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Messages;
using Stump.Server.AuthServer.Database;
using Stump.Server.AuthServer.Managers;
using Stump.Server.AuthServer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Stump.Server.AuthServer.Handlers.Connection
{
    public class ConnectionHandler : AuthHandlerContainer
    {
        private ConnectionHandler() { }

        [AuthHandler(IdentificationMessage.Id)]
        public static void HandleIdentificationMessage(AuthClient client, IdentificationMessage message)
        {
            if (!AuthQueueManager.Instance.IsInQueue(client))
            {
                client.IdentificationMessage = message;
                AuthQueueManager.Instance.AddClient(client);
            }
        }
        [AuthHandler(NicknameChoiceRequestMessage.Id)]
        public static void HandleNicknameChoiceRequestMessage(AuthClient client, NicknameChoiceRequestMessage message)
        {
            string nickname = message.nickname;
            if (!ConnectionHandler.CheckNickName(nickname))
            {
                client.Send(new NicknameRefusedMessage((sbyte)NicknameErrorEnum.INVALID_NICK));
            }
            else
            {
                if (nickname == client.Account.Login)
                {
                    client.Send(new NicknameRefusedMessage((sbyte)NicknameErrorEnum.SAME_AS_LOGIN));
                }
                else
                {
                    if (client.Account.Login.Contains(nickname))
                    {
                        client.Send(new NicknameRefusedMessage((sbyte)NicknameErrorEnum.TOO_SIMILAR_TO_LOGIN));
                    }
                    else
                    {
                        if (Singleton<AccountManager>.Instance.NicknameExists(nickname))
                        {
                            client.Send(new NicknameRefusedMessage((sbyte)NicknameErrorEnum.ALREADY_USED));
                        }
                        else
                        {
                            client.Account.Nickname = nickname;
                            client.Save();
                            client.Send(new NicknameAcceptedMessage());
                            ConnectionHandler.SendIdentificationSuccessMessage(client, false);
                            ConnectionHandler.SendServersListMessage(client);
                        }
                    }
                }
            }
        }
        [AuthHandler(AcquaintanceSearchMessage.Id)]
        public static void HandleAcquaintanceSearchMessage(AuthClient client, AcquaintanceSearchMessage message)
        {
            Account account = Singleton<AccountManager>.Instance.FindAccountByNickname(message.nickname);
            if (account == null)
            {
                ConnectionHandler.SendAcquaintanceSearchErrorMessage(client, AcquaintanceErrorEnum.NO_RESULT);
            }
            else
            {
                ConnectionHandler.SendAcquaintanceSearchServerListMessage(client, (
                    from wcr in account.WorldCharacters
                    select (ushort)wcr.WorldId).Distinct());
            }
        }
        [AuthHandler(ServerSelectionMessage.Id)]
        public static void HandleServerSelectionMessage(AuthClient client, ServerSelectionMessage message)
        {
            WorldServer serverById = Singleton<WorldServerManager>.Instance.GetServerById((int)message.serverId);
            if (serverById == null)
            {
                ConnectionHandler.SendSelectServerRefusedMessage(client, serverById, ServerConnectionErrorEnum.SERVER_CONNECTION_ERROR_NO_REASON);
            }
            else
            {
                if (serverById.Status != ServerStatusEnum.ONLINE)
                {
                    ConnectionHandler.SendSelectServerRefusedMessage(client, serverById, ServerConnectionErrorEnum.SERVER_CONNECTION_ERROR_DUE_TO_STATUS);
                }
                else
                {
                    if (serverById.RequiredRole > client.Account.Role)
                    {
                        ConnectionHandler.SendSelectServerRefusedMessage(client, serverById, ServerConnectionErrorEnum.SERVER_CONNECTION_ERROR_ACCOUNT_RESTRICTED);
                    }
                    else
                    {
                        ConnectionHandler.SendSelectServerData(client, serverById);
                    }
                }
            }
        }

        public static void SendCredentialsAcknowledgementMessage(AuthClient client)
        {
            client.Send(new CredentialsAcknowledgementMessage());
        }
        public static void SendLoginQueueStatusMessage(AuthClient client, ushort position, ushort count)
        {
            client.Send(new LoginQueueStatusMessage(position, count));
        }
        public static void SendAcquaintanceSearchServerListMessage(AuthClient client, IEnumerable<ushort> serverIds)
        {
            client.Send(new AcquaintanceServerListMessage(serverIds.ToArray()));
        }
        public static void SendAcquaintanceSearchErrorMessage(AuthClient client, AcquaintanceErrorEnum reason)
        {
            client.Send(new AcquaintanceSearchErrorMessage((sbyte)reason));
        }
        public static void SendIdentificationSuccessMessage(AuthClient client, bool wasAlreadyConnected)
        {
            client.Send(new IdentificationSuccessMessage(client.Account.Role >= RoleEnum.Moderator, wasAlreadyConnected, client.Account.Login, client.Account.Nickname, client.Account.Id, 0, client.Account.SecretQuestion, (DateTime.Now - client.Account.CreationDate).TotalMilliseconds, 0, 0xFFFF));
        }
        public static void SendIdentificationFailedMessage(AuthClient client, IdentificationFailureReasonEnum reason)
        {
            client.Send(new IdentificationFailedMessage((sbyte)reason));
        }
        public static void SendIdentificationFailedForBadVersionMessage(AuthClient client, Stump.DofusProtocol.Types.Version version)
        {
            client.Send(new IdentificationFailedForBadVersionMessage(1, version));
        }
        public static void SendIdentificationFailedBannedMessage(AuthClient client, DateTime date)
        {
            client.Send(new IdentificationFailedBannedMessage(3, (double)date.GetUnixTimeStamp()));
        }
        public static void SendSelectServerData(AuthClient client, WorldServer world)
        {
            if (world != null)
            {
                client.LookingOfServers = false;
                client.Account.Ticket = new AsyncRandom().RandomString(32);
                Singleton<AccountManager>.Instance.CacheAccount(client.Account);
                client.Account.LastConnection = new DateTime?(DateTime.Now);
                client.Account.LastConnectedIp = client.IP;
                client.Account.LastConnectionWorld = new int?(world.Id);
                client.SaveNow();

                var writer = new BigEndianWriter();
                writer.WriteByte((byte)client.Account.Ticket.Length);
                writer.WriteUTFBytes(client.Account.Ticket);
                client.Send(new SelectedServerDataMessage((ushort)world.Id, world.Address, world.Port, true, Cryptography.EncryptAES(writer.Data, client.AesKey)));

                client.Disconnect();
            }
        }
        public static void SendSelectServerRefusedMessage(AuthClient client, WorldServer world, ServerConnectionErrorEnum reason)
        {
            client.Send(new SelectedServerRefusedMessage((ushort)world.Id, (sbyte)reason, (sbyte)world.Status));
        }
        public static void SendServersListMessage(AuthClient client)
        {
            client.Send(new ServersListMessage(WorldServerManager.Instance.GetServersInformationArray(client), 0, false));
        }
        public static void SendServerStatusUpdateMessage(AuthClient client, WorldServer world)
        {
            client.Send(new ServerStatusUpdateMessage(WorldServerManager.Instance.GetServerInformation(client, world)));
        }

        public static bool CheckNickName(string nickName)
        {
            return Regex.IsMatch(nickName, "^[a-zA-Z\\-]{3,29}$", RegexOptions.Compiled);
        }
    }
}
