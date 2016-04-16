using NLog;
using Stump.Core.IO;
using Stump.Core.Reflection;
using Stump.Core.Threading;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Messages;
using Stump.Server.BaseServer;
using Stump.Server.BaseServer.Commands;
using Stump.Server.BaseServer.Initialization;
using Stump.Server.BaseServer.IPC;
using Stump.Server.BaseServer.IPC.Messages;
using Stump.Server.BaseServer.IPC.Objects;
using Stump.Server.BaseServer.Network;
using Stump.Server.WorldServer.Core.IPC;
using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Database.Accounts;
using Stump.Server.WorldServer.Game;
using Stump.Server.WorldServer.Game.Accounts;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Breeds;
using Stump.Server.WorldServer.Handlers.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Stump.Server.WorldServer.Handlers.Approach
{
	public class ApproachHandler : WorldHandlerContainer
	{
        public static SynchronizedCollection<WorldClient> ConnectionQueue = new SynchronizedCollection<WorldClient>();

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static Task m_queueRefresherTask;

        private ApproachHandler() { }

		[Initialization(InitializationPass.First)]
		private static void Initialize()
		{
			ApproachHandler.m_queueRefresherTask = Task.Factory.StartNewDelayed(3000, new Action(ApproachHandler.RefreshQueue));
		}

        [WorldHandler(AuthenticationTicketMessage.Id, ShouldBeLogged = false, IsGamePacket = false)]
        public static void HandleAuthenticationTicketMessage(WorldClient client, AuthenticationTicketMessage message)
        {
            if (!IPCAccessor.Instance.IsConnected)
            {
                client.Send(new AuthenticationTicketRefusedMessage());
                client.DisconnectLater(1000);
            }
            else
            {
                var reader = new BigEndianReader(Encoding.ASCII.GetBytes(message.ticket));
                var count = reader.ReadByte();
                var ticket = reader.ReadUTFBytes(count);

                ApproachHandler.logger.Debug("Client request ticket {0}", ticket);
                IPCAccessor.Instance.SendRequest<AccountAnswerMessage>(new AccountRequestMessage
                {
                    Ticket = ticket
                }, delegate(AccountAnswerMessage msg)
                {
                    ServerBase<WorldServer>.Instance.IOTaskPool.AddMessage(delegate
                    {
                        ApproachHandler.OnAccountReceived(msg, client);
                    });
                }, delegate(IPCErrorMessage error)
                {
                    client.Disconnect();
                });
            }
        }
        [WorldHandler(ReloginTokenRequestMessage.Id, ShouldBeLogged = false, IsGamePacket = false)]
        public static void HandleReloginTokenRequestMessage(WorldClient client, ReloginTokenRequestMessage message)
        {

        }

		private static void RefreshQueue()
		{
			try
			{
				System.Collections.Generic.List<WorldClient> list = new System.Collections.Generic.List<WorldClient>();
				int num = 0;
				lock (ApproachHandler.ConnectionQueue.SyncRoot)
				{
					foreach (WorldClient current in ApproachHandler.ConnectionQueue)
					{
						num++;
						if (!current.Connected)
						{
							list.Add(current);
						}
						if (!(System.DateTime.Now - current.InQueueUntil <= System.TimeSpan.FromSeconds(3.0)))
						{
							ApproachHandler.SendQueueStatusMessage(current, (ushort)num, (ushort)ApproachHandler.ConnectionQueue.Count);
							current.QueueShowed = true;
						}
					}
					foreach (WorldClient current in list)
					{
						ApproachHandler.ConnectionQueue.Remove(current);
					}
				}
			}
			finally
			{
				ApproachHandler.m_queueRefresherTask = Task.Factory.StartNewDelayed(3000, new Action(ApproachHandler.RefreshQueue));
			}
		}

        public static void SendServerSettingsMessage(WorldClient client)
        {//offi send 0 for gametype, so 0
            client.Send(new ServerSettingsMessage("fr", 0, 0));
        }
		public static void SendStartupActionsListMessage(IPacketReceiver client)
		{
			client.Send(new StartupActionsListMessage());
		}
		public static void SendServerOptionalFeaturesMessage(IPacketReceiver client, System.Collections.Generic.IEnumerable<sbyte> features)
		{
			client.Send(new ServerOptionalFeaturesMessage(features));
		}
		public static void SendAccountCapabilitiesMessage(WorldClient client)
        {
            client.Send(new AccountCapabilitiesMessage(true, true, client.Account.Id, Singleton<BreedManager>.Instance.AvailableBreedsFlags, client.Account.BreedFlags, (sbyte)client.UserGroup.Role));
		}
		public static void SendConsoleCommandsListMessage(IPacketReceiver client)
		{
			client.Send(new ConsoleCommandsListMessage(ServerBase<WorldServer>.Instance.CommandManager.AvailableCommands.SelectMany((CommandBase c) => c.Aliases), new string[0], new string[0]));
		}
		public static void SendQueueStatusMessage(IPacketReceiver client, ushort position, ushort total)
		{
			client.Send(new QueueStatusMessage(position, total));
		}

        private static void OnAccountReceived(AccountAnswerMessage message, WorldClient client)
        {
            lock (ApproachHandler.ConnectionQueue.SyncRoot)
            {
                ApproachHandler.ConnectionQueue.Remove(client);
            }
            if (client.QueueShowed)
            {
                ApproachHandler.SendQueueStatusMessage(client, 0, 0);
            }
            AccountData account = message.Account;
            if (account == null)
            {
                client.Send(new AuthenticationTicketRefusedMessage());
                client.DisconnectLater(1000);
            }
            else
            {
                WorldAccount worldAccount = Singleton<AccountManager>.Instance.FindById(account.Id);
                if (worldAccount != null)
                {
                    client.WorldAccount = worldAccount;
                    if (client.WorldAccount.ConnectedCharacter.HasValue)
                    {
                        Character character = Singleton<World>.Instance.GetCharacter(client.WorldAccount.ConnectedCharacter.Value);
                        if (character != null)
                        {
                            character.LogOut();
                        }
                    }
                }

                client.SetCurrentAccount(account);
                client.Send(new AuthenticationTicketAcceptedMessage());

                BasicHandler.SendBasicTimeMessage(client);
                ApproachHandler.SendServerOptionalFeaturesMessage(client, new sbyte[1] { 7 });
                ApproachHandler.SendAccountCapabilitiesMessage(client);
                client.Send(new TrustStatusMessage(true, false));
                if (client.UserGroup.Role >= RoleEnum.Moderator)
                {
                    ApproachHandler.SendConsoleCommandsListMessage(client);
                }
            }
        }
	}
}
