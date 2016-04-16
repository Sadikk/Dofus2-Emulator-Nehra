using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Messages;
using Stump.Server.BaseServer;
using Stump.Server.BaseServer.Network;
using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Database.Accounts;
using Stump.Server.WorldServer.Game;
using Stump.Server.WorldServer.Game.Accounts;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Social;
using System.Linq;
namespace Stump.Server.WorldServer.Handlers.Friends
{
	public class FriendHandler : WorldHandlerContainer
	{
		[WorldHandler(4001u)]
		public static void HandleFriendsGetListMessage(WorldClient client, FriendsGetListMessage message)
		{
			FriendHandler.SendFriendsListMessage(client, client.Character.FriendsBook.Friends);
		}
		[WorldHandler(5676u)]
		public static void HandleIgnoredGetListMessage(WorldClient client, IgnoredGetListMessage message)
		{
			FriendHandler.SendIgnoredListMessage(client, client.Character.FriendsBook.Ignoreds);
		}
		[WorldHandler(4004u)]
		public static void HandleFriendAddRequestMessage(WorldClient client, FriendAddRequestMessage message)
		{
			Character character = Singleton<World>.Instance.GetCharacter(message.name);
			if (character != null)
			{
				if (character.UserGroup.Role == RoleEnum.Player)
				{
					client.Character.FriendsBook.AddFriend(character.Client.WorldAccount);
				}
				else
				{
					FriendHandler.SendFriendAddFailureMessage(client, ListAddFailureEnum.LIST_ADD_FAILURE_NOT_FOUND);
				}
			}
			else
			{
				ServerBase<WorldServer>.Instance.IOTaskPool.AddMessage(delegate
				{
					WorldAccount record = Singleton<AccountManager>.Instance.FindByNickname(message.name);
					if (record != null && client.Character.Context != null)
					{
						client.Character.Context.ExecuteInContext(delegate
						{
							client.Character.FriendsBook.AddFriend(record);
						});
					}
					else
					{
						FriendHandler.SendFriendAddFailureMessage(client, ListAddFailureEnum.LIST_ADD_FAILURE_NOT_FOUND);
					}
				});
			}
		}
		[WorldHandler(5603u)]
		public static void HandleFriendDeleteRequestMessage(WorldClient client, FriendDeleteRequestMessage message)
		{
			Friend friend = client.Character.FriendsBook.Friends.FirstOrDefault((Friend entry) => entry.Account.Id == message.accountId);
			if (friend == null)
			{
                FriendHandler.SendFriendDeleteResultMessage(client, false, friend.Account.Nickname);
			}
			else
			{
				client.Character.FriendsBook.RemoveFriend(friend);
			}
		}
		[WorldHandler(5673u)]
		public static void HandleIgnoredAddRequestMessage(WorldClient client, IgnoredAddRequestMessage message)
		{
			Character character = Singleton<World>.Instance.GetCharacter(message.name);
			if (character != null)
			{
				if (character.UserGroup.Role == RoleEnum.Player)
				{
					client.Character.FriendsBook.AddIgnored(character.Client.WorldAccount, message.session);
				}
				else
				{
					FriendHandler.SendFriendAddFailureMessage(client, ListAddFailureEnum.LIST_ADD_FAILURE_NOT_FOUND);
				}
			}
			else
			{
				ServerBase<WorldServer>.Instance.IOTaskPool.AddMessage(delegate
				{
					WorldAccount record = Singleton<AccountManager>.Instance.FindByNickname(message.name);
					if (record != null && client.Character.Context != null)
					{
						client.Character.Context.ExecuteInContext(delegate
						{
							client.Character.FriendsBook.AddIgnored(record, message.session);
						});
					}
					else
					{
						FriendHandler.SendIgnoredAddFailureMessage(client, ListAddFailureEnum.LIST_ADD_FAILURE_NOT_FOUND);
					}
				});
			}
		}
		[WorldHandler(5680u)]
		public static void HandleIgnoredDeleteRequestMessage(WorldClient client, IgnoredDeleteRequestMessage message)
		{
			Ignored ignored = client.Character.FriendsBook.Ignoreds.FirstOrDefault((Ignored entry) => entry.Account.Id == message.accountId);
			if (ignored == null)
			{
                FriendHandler.SendIgnoredDeleteResultMessage(client, false, false, ignored.Account.Nickname);
			}
			else
			{
				client.Character.FriendsBook.RemoveIgnored(ignored);
			}
		}
		[WorldHandler(5602u)]
		public static void HandleFriendSetWarnOnConnectionMessage(WorldClient client, FriendSetWarnOnConnectionMessage message)
		{
			client.Character.FriendsBook.WarnOnConnection = message.enable;
		}
		[WorldHandler(6078u)]
		public static void HandleFriendWarnOnLevelGainStateMessage(WorldClient client, FriendWarnOnLevelGainStateMessage message)
		{
			client.Character.FriendsBook.WarnOnLevel = message.enable;
		}
		public static void SendFriendWarnOnConnectionStateMessage(IPacketReceiver client, bool state)
		{
			client.Send(new FriendWarnOnConnectionStateMessage(state));
		}
		public static void SendFriendWarnOnLevelGainStateMessage(IPacketReceiver client, bool state)
		{
			client.Send(new FriendWarnOnLevelGainStateMessage(state));
		}
		public static void SendFriendAddFailureMessage(IPacketReceiver client, ListAddFailureEnum reason)
		{
			client.Send(new FriendAddFailureMessage((sbyte)reason));
		}
		public static void SendFriendDeleteResultMessage(IPacketReceiver client, bool success, string name)
		{
			client.Send(new FriendDeleteResultMessage(success, name));
		}
		public static void SendFriendUpdateMessage(IPacketReceiver client, Friend friend)
		{
			client.Send(new FriendUpdateMessage(friend.GetFriendInformations()));
		}
		public static void SendFriendsListMessage(IPacketReceiver client, System.Collections.Generic.IEnumerable<Friend> friends)
		{
			client.Send(new FriendsListMessage(
				from entry in friends
				select entry.GetFriendInformations()));
		}
		public static void SendIgnoredAddFailureMessage(IPacketReceiver client, ListAddFailureEnum reason)
		{
			client.Send(new IgnoredAddFailureMessage((sbyte)reason));
		}
		public static void SendIgnoredDeleteResultMessage(IPacketReceiver client, bool success, bool session, string name)
		{
			client.Send(new IgnoredDeleteResultMessage(success, session, name));
		}
		public static void SendIgnoredListMessage(IPacketReceiver client, System.Collections.Generic.IEnumerable<Ignored> ignoreds)
		{
			client.Send(new IgnoredListMessage(
				from entry in ignoreds
				select entry.GetIgnoredInformations()));
		}
	}
}
