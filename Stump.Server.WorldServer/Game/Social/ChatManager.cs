using Stump.Core.Attributes;
using Stump.Core.IO;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer;
using Stump.Server.BaseServer.Initialization;
using Stump.Server.BaseServer.Network;
using Stump.Server.WorldServer.Commands.Trigger;
using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Actors.Interfaces;
using Stump.Server.WorldServer.Game.Actors.RolePlay;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Handlers.Chat;
using System.Web;
namespace Stump.Server.WorldServer.Game.Social
{
	public class ChatManager : Singleton<ChatManager>
	{
		public delegate void ChatParserDelegate(WorldClient client, string msg);
		[Variable]
		public static readonly string CommandPrefix = ".";
		[Variable]
		public static readonly RoleEnum AdministratorChatMinAccess = RoleEnum.Moderator;
		[Variable]
		public static int AntiFloodTimeBetweenTwoMessages = 500;
		[Variable]
		public static int AntiFloodTimeBetweenTwoGlobalMessages = 60;
		[Variable]
		public static int AntiFloodAllowedMessages = 4;
		[Variable]
		public static int AntiFloodAllowedMessagesResetTime = 10;
		[Variable]
		public static int AntiFloodMuteTime = 10;
		public readonly System.Collections.Generic.Dictionary<ChatActivableChannelsEnum, ChatManager.ChatParserDelegate> ChatHandlers = new System.Collections.Generic.Dictionary<ChatActivableChannelsEnum, ChatManager.ChatParserDelegate>();
		[Initialization(InitializationPass.First)]
		public void Initialize()
		{
			this.ChatHandlers.Add(ChatActivableChannelsEnum.CHANNEL_GLOBAL, new ChatManager.ChatParserDelegate(this.SayGlobal));
			this.ChatHandlers.Add(ChatActivableChannelsEnum.CHANNEL_GUILD, new ChatManager.ChatParserDelegate(this.SayGuild));
			this.ChatHandlers.Add(ChatActivableChannelsEnum.CHANNEL_PARTY, new ChatManager.ChatParserDelegate(this.SayParty));
			this.ChatHandlers.Add(ChatActivableChannelsEnum.CHANNEL_SALES, new ChatManager.ChatParserDelegate(this.SaySales));
			this.ChatHandlers.Add(ChatActivableChannelsEnum.CHANNEL_SEEK, new ChatManager.ChatParserDelegate(this.SaySeek));
			this.ChatHandlers.Add(ChatActivableChannelsEnum.CHANNEL_ADMIN, new ChatManager.ChatParserDelegate(this.SayAdministrators));
			this.ChatHandlers.Add(ChatActivableChannelsEnum.CHANNEL_TEAM, new ChatManager.ChatParserDelegate(this.SayTeam));
		}
		public bool CanUseChannel(Character character, ChatActivableChannelsEnum channel)
		{
			bool result;
			switch (channel)
			{
			case ChatActivableChannelsEnum.CHANNEL_GLOBAL:
				result = (!character.Map.IsMuted || character.UserGroup.Role >= ChatManager.AdministratorChatMinAccess);
				break;
			case ChatActivableChannelsEnum.CHANNEL_TEAM:
				result = character.IsFighting();
				break;
			case ChatActivableChannelsEnum.CHANNEL_GUILD:
				result = (character.Guild != null);
				break;
			case ChatActivableChannelsEnum.CHANNEL_PARTY:
				result = character.IsInParty();
				break;
			case ChatActivableChannelsEnum.CHANNEL_SALES:
				result = !character.IsMuted();
				break;
			case ChatActivableChannelsEnum.CHANNEL_SEEK:
				result = !character.IsMuted();
				break;
			case ChatActivableChannelsEnum.CHANNEL_NOOB:
				result = true;
				break;
			case ChatActivableChannelsEnum.CHANNEL_ADMIN:
				result = (character.UserGroup.Role >= ChatManager.AdministratorChatMinAccess);
				break;
			case ChatActivableChannelsEnum.PSEUDO_CHANNEL_PRIVATE:
				result = !character.IsMuted();
				break;
			case ChatActivableChannelsEnum.PSEUDO_CHANNEL_INFO:
				result = false;
				break;
			case ChatActivableChannelsEnum.PSEUDO_CHANNEL_FIGHT_LOG:
				result = false;
				break;
			case ChatActivableChannelsEnum.CHANNEL_ADS:
				result = !character.IsMuted();
				break;
			default:
				result = false;
				break;
			}
			return result;
		}
		public void HandleChat(WorldClient client, ChatActivableChannelsEnum channel, string message)
		{
			if (this.CanUseChannel(client.Character, channel) && this.ChatHandlers.ContainsKey(channel))
			{
				if (message.StartsWith(ChatManager.CommandPrefix) && (message.Length < ChatManager.CommandPrefix.Length * 2 || message.Substring(ChatManager.CommandPrefix.Length, ChatManager.CommandPrefix.Length) != ChatManager.CommandPrefix))
				{
					message = message.Remove(0, ChatManager.CommandPrefix.Length);
					ServerBase<WorldServer>.Instance.CommandManager.HandleCommand(new TriggerChat(new StringStream(ChatManager.UnescapeChatCommand(message)), client.Character));
				}
				else
				{
					if (client.Character.IsMuted())
					{
						client.Character.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 124, new object[]
						{
							(int)client.Character.GetMuteRemainingTime().TotalSeconds
						});
					}
					else
					{
						if (client.Character.ChatHistory.RegisterAndCheckFlood(new ChatEntry(message, channel, System.DateTime.Now)))
						{
							this.ChatHandlers[channel](client, message);
						}
					}
				}
			}
		}
		private static string UnescapeChatCommand(string command)
		{
			return HttpUtility.HtmlDecode(command);
		}
		private static void SendChatServerMessage(IPacketReceiver client, Character sender, ChatActivableChannelsEnum channel, string message)
		{
			if (sender.AdminMessagesEnabled)
			{
				ChatHandler.SendChatAdminServerMessage(client, sender, channel, message);
			}
			else
			{
				ChatHandler.SendChatServerMessage(client, sender, channel, message);
			}
		}
		private static void SendChatServerMessage(IPacketReceiver client, INamedActor sender, ChatActivableChannelsEnum channel, string message)
		{
			ChatHandler.SendChatServerMessage(client, sender, channel, message);
		}
		public void SayGlobal(WorldClient client, string msg)
		{
			if (this.CanUseChannel(client.Character, ChatActivableChannelsEnum.CHANNEL_GLOBAL))
			{
				if (client.Character.IsFighting())
				{
					ChatManager.SendChatServerMessage(client.Character.Fight.Clients, client.Character, ChatActivableChannelsEnum.CHANNEL_GLOBAL, msg);
				}
				else
				{
					if (client.Character.IsSpectator())
					{
						ChatManager.SendChatServerMessage(client.Character.Fight.SpectatorClients, client.Character, ChatActivableChannelsEnum.CHANNEL_GLOBAL, msg);
					}
					else
					{
						ChatManager.SendChatServerMessage(client.Character.Map.Clients, client.Character, ChatActivableChannelsEnum.CHANNEL_GLOBAL, msg);
					}
				}
			}
		}
		public void SayGlobal(NamedActor actor, string msg)
		{
			ChatManager.SendChatServerMessage(actor.CharacterContainer.Clients, actor, ChatActivableChannelsEnum.CHANNEL_GLOBAL, msg);
		}
		public void SayAdministrators(WorldClient client, string msg)
		{
			if (client.UserGroup.Role >= ChatManager.AdministratorChatMinAccess)
			{
				Singleton<World>.Instance.ForEachCharacter(delegate(Character entry)
				{
					if (this.CanUseChannel(entry, ChatActivableChannelsEnum.CHANNEL_ADMIN))
					{
						ChatHandler.SendChatServerMessage(entry.Client, client.Character, ChatActivableChannelsEnum.CHANNEL_ADMIN, msg);
					}
				});
			}
		}
		public void SayParty(WorldClient client, string msg)
		{
			if (!this.CanUseChannel(client.Character, ChatActivableChannelsEnum.CHANNEL_PARTY))
			{
				ChatHandler.SendChatErrorMessage(client, ChatErrorEnum.CHAT_ERROR_NO_PARTY);
			}
			else
			{
				client.Character.Party.ForEach(delegate(Character entry)
				{
					ChatManager.SendChatServerMessage(entry.Client, client.Character, ChatActivableChannelsEnum.CHANNEL_PARTY, msg);
				});
			}
		}
		public void SayGuild(WorldClient client, string msg)
		{
			if (!this.CanUseChannel(client.Character, ChatActivableChannelsEnum.CHANNEL_GUILD))
			{
				ChatHandler.SendChatErrorMessage(client, ChatErrorEnum.CHAT_ERROR_NO_GUILD);
			}
			else
			{
				client.Character.Guild.Clients.ForEach(delegate(WorldClient entry)
				{
					ChatManager.SendChatServerMessage(entry, client.Character, ChatActivableChannelsEnum.CHANNEL_GUILD, msg);
				});
			}
		}
		public void SayTeam(WorldClient client, string msg)
		{
			if (!this.CanUseChannel(client.Character, ChatActivableChannelsEnum.CHANNEL_TEAM))
			{
				ChatHandler.SendChatErrorMessage(client, ChatErrorEnum.CHAT_ERROR_NO_TEAM);
			}
			else
			{
				foreach (CharacterFighter current in client.Character.Fighter.Team.GetAllFighters<CharacterFighter>())
				{
					ChatManager.SendChatServerMessage(current.Character.Client, client.Character, ChatActivableChannelsEnum.CHANNEL_TEAM, msg);
				}
			}
		}
		public void SaySeek(WorldClient client, string msg)
		{
			if (this.CanUseChannel(client.Character, ChatActivableChannelsEnum.CHANNEL_SEEK))
			{
				Singleton<World>.Instance.ForEachCharacter(delegate(Character entry)
				{
					ChatManager.SendChatServerMessage(entry.Client, client.Character, ChatActivableChannelsEnum.CHANNEL_SEEK, msg);
				});
			}
		}
		public void SaySales(WorldClient client, string msg)
		{
			if (this.CanUseChannel(client.Character, ChatActivableChannelsEnum.CHANNEL_SALES))
			{
				Singleton<World>.Instance.ForEachCharacter(delegate(Character entry)
				{
					ChatManager.SendChatServerMessage(entry.Client, client.Character, ChatActivableChannelsEnum.CHANNEL_SALES, msg);
				});
			}
		}
		public static bool IsGlobalChannel(ChatActivableChannelsEnum channel)
		{
			return channel == ChatActivableChannelsEnum.CHANNEL_SALES || channel == ChatActivableChannelsEnum.CHANNEL_SEEK;
		}
	}
}
