using MongoDB.Bson;
using Stump.Core.Extensions;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Messages;
using Stump.Server.BaseServer.Logging;
using Stump.Server.BaseServer.Network;
using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Game;
using Stump.Server.WorldServer.Game.Actors;
using Stump.Server.WorldServer.Game.Actors.Interfaces;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Social;

namespace Stump.Server.WorldServer.Handlers.Chat
{
	public class ChatHandler : WorldHandlerContainer
	{
		[WorldHandler(890u)]
		public static void HandleChannelEnablingMessage(WorldClient client, ChannelEnablingMessage message)
		{
		}
		public static void SendEnabledChannelsMessage(IPacketReceiver client, sbyte[] allows, sbyte[] disallows)
		{
			client.Send(new EnabledChannelsMessage(allows, disallows));
		}
		[WorldHandler(851u)]
		public static void HandleChatClientPrivateMessage(WorldClient client, ChatClientPrivateMessage message)
		{
			if (!string.IsNullOrEmpty(message.content))
			{
				Character character = Singleton<World>.Instance.GetCharacter(message.receiver);
				if (character != null)
				{
					if (client.Character.IsMuted())
					{
						client.Character.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 123, new object[]
						{
							(int)client.Character.GetMuteRemainingTime().TotalSeconds
						});
					}
					else
					{
						if (client.Character != character)
						{
							if (!character.IsAway)
							{
								if (client.Character.IsAway)
								{
									client.Character.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_MESSAGE, 72, new object[0]);
								}
								ChatHandler.SendChatServerCopyMessage(client, character, character, ChatActivableChannelsEnum.PSEUDO_CHANNEL_PRIVATE, message.content);
								ChatHandler.SendChatServerMessage(character.Client, client.Character, ChatActivableChannelsEnum.PSEUDO_CHANNEL_PRIVATE, message.content);
								BsonDocument document = new BsonDocument
								{

									{
										"SenderId",
										client.Character.Id
									},

									{
										"ReceiverId",
										character.Id
									},

									{
										"Message",
										message.content
									},

									{
										"Date",
										System.DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture)
									}
								};
								Singleton<MongoLogger>.Instance.Insert("PrivateMSG", document);
							}
							else
							{
								client.Character.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 14, new object[]
								{
									character.Name
								});
							}
						}
						else
						{
							ChatHandler.SendChatErrorMessage(client, ChatErrorEnum.CHAT_ERROR_INTERIOR_MONOLOGUE);
						}
					}
				}
				else
				{
					ChatHandler.SendChatErrorMessage(client, ChatErrorEnum.CHAT_ERROR_RECEIVER_NOT_FOUND);
				}
			}
		}
		[WorldHandler(861u)]
		public static void HandleChatClientMultiMessage(WorldClient client, ChatClientMultiMessage message)
		{
			BsonDocument document = new BsonDocument
			{

				{
					"SenderId",
					client.Character.Id
				},

				{
					"Message",
					message.content
				},

				{
					"Date",
					System.DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture)
				}
			};
			Singleton<MongoLogger>.Instance.Insert("MultiMessage", document);
			Singleton<ChatManager>.Instance.HandleChat(client, (ChatActivableChannelsEnum)message.channel, message.content);
		}
		public static void SendChatServerMessage(IPacketReceiver client, string message)
		{
			ChatHandler.SendChatServerMessage(client, ChatActivableChannelsEnum.PSEUDO_CHANNEL_INFO, message, System.DateTime.Now.GetUnixTimeStamp(), "", 0, "", 0);
		}
		public static void SendChatServerMessage(IPacketReceiver client, INamedActor sender, ChatActivableChannelsEnum channel, string message)
		{
			ChatHandler.SendChatServerMessage(client, sender, channel, message, System.DateTime.Now.GetUnixTimeStamp(), "");
		}
		public static void SendChatServerMessage(IPacketReceiver client, INamedActor sender, ChatActivableChannelsEnum channel, string message, int timestamp, string fingerprint)
		{
			client.Send(new ChatServerMessage((sbyte)channel, message, timestamp, fingerprint, sender.Id, sender.Name, 0));
		}
		public static void SendChatServerMessage(IPacketReceiver client, Character sender, ChatActivableChannelsEnum channel, string message)
		{
			ChatHandler.SendChatServerMessage(client, sender, channel, message, System.DateTime.Now.GetUnixTimeStamp(), "");
		}
		public static void SendChatServerMessage(IPacketReceiver client, Character sender, ChatActivableChannelsEnum channel, string message, int timestamp, string fingerprint)
		{
			if (!string.IsNullOrEmpty(message))
			{
				if (sender.UserGroup.Role <= RoleEnum.Moderator)
				{
					message = message.HtmlEntities();
				}
				client.Send(new ChatServerMessage((sbyte)channel, message, timestamp, fingerprint, sender.Id, sender.Name, sender.Account.Id));
			}
		}
		public static void SendChatServerMessage(IPacketReceiver client, ChatActivableChannelsEnum channel, string message, int timestamp, string fingerprint, int senderId, string senderName, int accountId)
		{
			if (!string.IsNullOrEmpty(message))
			{
				client.Send(new ChatServerMessage((sbyte)channel, message, timestamp, fingerprint, senderId, senderName, accountId));
			}
		}
		public static void SendChatAdminServerMessage(IPacketReceiver client, Character sender, ChatActivableChannelsEnum channel, string message)
		{
			ChatHandler.SendChatAdminServerMessage(client, sender, channel, message, System.DateTime.Now.GetUnixTimeStamp(), "");
		}
		public static void SendChatAdminServerMessage(IPacketReceiver client, Character sender, ChatActivableChannelsEnum channel, string message, int timestamp, string fingerprint)
		{
			ChatHandler.SendChatAdminServerMessage(client, channel, message, timestamp, fingerprint, sender.Id, sender.Name, sender.Account.Id);
		}
		public static void SendChatAdminServerMessage(IPacketReceiver client, ChatActivableChannelsEnum channel, string message, int timestamp, string fingerprint, int senderId, string senderName, int accountId)
		{
			if (!string.IsNullOrEmpty(message))
			{
				client.Send(new ChatAdminServerMessage((sbyte)channel, message, timestamp, fingerprint, senderId, senderName, accountId));
			}
		}
		public static void SendChatServerCopyMessage(IPacketReceiver client, Character sender, Character receiver, ChatActivableChannelsEnum channel, string message)
		{
			ChatHandler.SendChatServerCopyMessage(client, sender, receiver, channel, message, System.DateTime.Now.GetUnixTimeStamp(), "");
		}
		public static void SendChatServerCopyMessage(IPacketReceiver client, Character sender, Character receiver, ChatActivableChannelsEnum channel, string message, int timestamp, string fingerprint)
		{
			if (sender.UserGroup.Role <= RoleEnum.Moderator)
			{
				message = message.HtmlEntities();
			}
            client.Send(new ChatServerCopyMessage((sbyte)channel, message, timestamp, fingerprint, (uint)receiver.Id, receiver.Name));
		}
		public static void SendChatErrorMessage(IPacketReceiver client, ChatErrorEnum error)
		{
			client.Send(new ChatErrorMessage((sbyte)error));
		}
		[WorldHandler(800u)]
		public static void HandleChatSmileyRequestMessage(WorldClient client, ChatSmileyRequestMessage message)
		{
			client.Character.DisplaySmiley(message.smileyId);
		}
		public static void SendChatSmileyMessage(IPacketReceiver client, Character character, sbyte smileyId)
		{
			client.Send(new ChatSmileyMessage(character.Id, smileyId, character.Account.Id));
		}
		public static void SendChatSmileyMessage(IPacketReceiver client, ContextActor entity, sbyte smileyId)
		{
			client.Send(new ChatSmileyMessage(entity.Id, smileyId, 0));
		}
	}
}
