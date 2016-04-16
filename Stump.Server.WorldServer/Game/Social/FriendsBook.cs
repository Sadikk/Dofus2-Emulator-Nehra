using NLog;
using Stump.Core.Attributes;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer;
using Stump.Server.WorldServer.Database.Accounts;
using Stump.Server.WorldServer.Game.Accounts;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Handlers.Basic;
using Stump.Server.WorldServer.Handlers.Characters;
using Stump.Server.WorldServer.Handlers.Friends;
using System.Collections.Concurrent;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Social
{
	public class FriendsBook : System.IDisposable
	{
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();
		[Variable(true)]
		public static int MaxFriendsNumber = 30;
		private readonly ConcurrentDictionary<int, Friend> m_friends = new ConcurrentDictionary<int, Friend>();
		private readonly ConcurrentDictionary<int, Ignored> m_ignoreds = new ConcurrentDictionary<int, Ignored>();
		private readonly ConcurrentStack<AccountRelation> m_relationsToRemove = new ConcurrentStack<AccountRelation>();
		private object m_lock = new object();
		private ConcurrentDictionary<int, AccountRelation> m_relations;
		public Character Owner
		{
			get;
			set;
		}
		public System.Collections.Generic.IEnumerable<Friend> Friends
		{
			get
			{
				return this.m_friends.Values;
			}
		}
		public System.Collections.Generic.IEnumerable<Ignored> Ignoreds
		{
			get
			{
				return this.m_ignoreds.Values;
			}
		}
		public bool WarnOnConnection
		{
			get
			{
				return this.Owner.Record.WarnOnConnection;
			}
			set
			{
				this.Owner.Record.WarnOnConnection = value;
				FriendHandler.SendFriendWarnOnConnectionStateMessage(this.Owner.Client, value);
			}
		}
		public bool WarnOnLevel
		{
			get
			{
				return this.Owner.Record.WarnOnLevel;
			}
			set
			{
				this.Owner.Record.WarnOnLevel = value;
				FriendHandler.SendFriendWarnOnLevelGainStateMessage(this.Owner.Client, value);
			}
		}
		public FriendsBook(Character owner)
		{
			this.Owner = owner;
		}
		public void Dispose()
		{
			Singleton<World>.Instance.CharacterJoined -= new System.Action<Character>(this.OnCharacterLogIn);
		}
		public ListAddFailureEnum? CanAddFriend(WorldAccount friendAccount)
		{
			ListAddFailureEnum? result;
			if (friendAccount.Id == this.Owner.Client.WorldAccount.Id)
			{
				result = new ListAddFailureEnum?(ListAddFailureEnum.LIST_ADD_FAILURE_EGOCENTRIC);
			}
			else
			{
				if (this.m_friends.ContainsKey(friendAccount.Id))
				{
					result = new ListAddFailureEnum?(ListAddFailureEnum.LIST_ADD_FAILURE_IS_DOUBLE);
				}
				else
				{
					if (this.m_friends.Count >= FriendsBook.MaxFriendsNumber)
					{
						result = new ListAddFailureEnum?(ListAddFailureEnum.LIST_ADD_FAILURE_OVER_QUOTA);
					}
					else
					{
						result = null;
					}
				}
			}
			return result;
		}
		public bool AddFriend(WorldAccount friendAccount)
		{
			ListAddFailureEnum? listAddFailureEnum = this.CanAddFriend(friendAccount);
			bool result;
			if (listAddFailureEnum.HasValue)
			{
				FriendHandler.SendFriendAddFailureMessage(this.Owner.Client, listAddFailureEnum.Value);
				result = false;
			}
			else
			{
				AccountRelation accountRelation = new AccountRelation
				{
					AccountId = this.Owner.Client.Account.Id,
					TargetId = friendAccount.Id,
					Type = AccountRelationType.Friend
				};
				this.m_relations.AddOrUpdate(accountRelation.TargetId, accountRelation, delegate(int key, AccountRelation value)
				{
					value.Type = AccountRelationType.Friend;
					return value;
				});
				bool flag;
				if (friendAccount.ConnectedCharacter.HasValue)
				{
					Character character = Singleton<World>.Instance.GetCharacter(friendAccount.ConnectedCharacter.Value);
					Friend friend = new Friend(accountRelation, friendAccount, character);
					if (flag = this.m_friends.TryAdd(friendAccount.Id, friend))
					{
						this.OnFriendOnline(friend);
					}
				}
				else
				{
					flag = this.m_friends.TryAdd(friendAccount.Id, new Friend(accountRelation, friendAccount));
				}
				FriendHandler.SendFriendsListMessage(this.Owner.Client, this.Friends);
				result = flag;
			}
			return result;
		}
		public bool RemoveFriend(Friend friend)
		{
			if (friend.IsOnline())
			{
				this.OnCharacterLogout(friend.Character);
			}
			Friend friend2;
			bool result;
			if (this.m_friends.TryRemove(friend.Account.Id, out friend2))
			{
				this.m_relationsToRemove.Push(friend.Relation);
				FriendHandler.SendFriendDeleteResultMessage(this.Owner.Client, true, friend.Account.Nickname);
				result = true;
			}
			else
			{
				FriendHandler.SendFriendDeleteResultMessage(this.Owner.Client, false, friend.Account.Nickname);
				result = false;
			}
			return result;
		}
		public ListAddFailureEnum? CanAddIgnored(WorldAccount ignoredAccount)
		{
			ListAddFailureEnum? result;
			if (ignoredAccount.Id == this.Owner.Client.WorldAccount.Id)
			{
				result = new ListAddFailureEnum?(ListAddFailureEnum.LIST_ADD_FAILURE_EGOCENTRIC);
			}
			else
			{
				if (this.m_ignoreds.ContainsKey(ignoredAccount.Id))
				{
					result = new ListAddFailureEnum?(ListAddFailureEnum.LIST_ADD_FAILURE_IS_DOUBLE);
				}
				else
				{
					if (this.m_ignoreds.Count >= FriendsBook.MaxFriendsNumber)
					{
						result = new ListAddFailureEnum?(ListAddFailureEnum.LIST_ADD_FAILURE_OVER_QUOTA);
					}
					else
					{
						result = null;
					}
				}
			}
			return result;
		}
		public bool AddIgnored(WorldAccount ignoredAccount, bool session = false)
		{
			ListAddFailureEnum? listAddFailureEnum = this.CanAddIgnored(ignoredAccount);
			bool result;
			if (listAddFailureEnum.HasValue)
			{
				FriendHandler.SendIgnoredAddFailureMessage(this.Owner.Client, listAddFailureEnum.Value);
				result = false;
			}
			else
			{
				AccountRelation accountRelation = new AccountRelation
				{
					AccountId = this.Owner.Client.Account.Id,
					TargetId = ignoredAccount.Id,
					Type = AccountRelationType.Ignored
				};
				if (!session)
				{
					this.m_relations.AddOrUpdate(accountRelation.TargetId, accountRelation, delegate(int key, AccountRelation value)
					{
						value.Type = AccountRelationType.Ignored;
						return value;
					});
				}
				bool flag;
				if (ignoredAccount.ConnectedCharacter.HasValue)
				{
					Character character = Singleton<World>.Instance.GetCharacter(ignoredAccount.ConnectedCharacter.Value);
					flag = this.m_ignoreds.TryAdd(ignoredAccount.Id, new Ignored(accountRelation, ignoredAccount, session, character));
				}
				else
				{
					flag = this.m_ignoreds.TryAdd(ignoredAccount.Id, new Ignored(accountRelation, ignoredAccount, session));
				}
				FriendHandler.SendIgnoredListMessage(this.Owner.Client, this.Ignoreds);
				result = flag;
			}
			return result;
		}
		public bool RemoveIgnored(Ignored ignored)
		{
			Ignored ignored2;
			bool result;
			if (this.m_ignoreds.TryRemove(ignored.Account.Id, out ignored2))
			{
				this.m_relationsToRemove.Push(ignored.Relation);
				FriendHandler.SendIgnoredDeleteResultMessage(this.Owner.Client, true, ignored.Session, ignored.Account.Nickname);
				result = true;
			}
			else
			{
				FriendHandler.SendIgnoredDeleteResultMessage(this.Owner.Client, false, ignored.Session, ignored.Account.Nickname);
				result = false;
			}
			return result;
		}
		private void OnCharacterLogIn(Character character)
		{
			Friend friend;
			if (this.m_friends.TryGetValue(character.Client.WorldAccount.Id, out friend))
			{
				friend.SetOnline(character);
				this.OnFriendOnline(friend);
				if (this.WarnOnConnection)
				{
					BasicHandler.SendTextInformationMessage(this.Owner.Client, TextInformationTypeEnum.TEXT_INFORMATION_MESSAGE, 143, new string[]
					{
						character.Client.WorldAccount.Nickname,
						character.Name
					});
				}
			}
			Ignored ignored;
			if (this.m_ignoreds.TryGetValue(character.Client.WorldAccount.Id, out ignored))
			{
				ignored.SetOnline(character);
			}
		}
		private void OnFriendOnline(Friend friend)
		{
			friend.Character.LoggedOut += new System.Action<Character>(this.OnCharacterLogout);
			friend.Character.LevelChanged += new Character.LevelChangedHandler(this.OnLevelChanged);
			friend.Character.ContextChanged += new Character.CharacterContextChangedHandler(this.OnContextChanged);
		}
		private void OnContextChanged(Character character, bool infight)
		{
			Friend friend = this.TryGetFriend(character);
			if (friend == null)
			{
				FriendsBook.logger.Error<Character>("Sad, friend bound with character {0} is not found :(", character);
			}
			else
			{
				FriendHandler.SendFriendUpdateMessage(this.Owner.Client, friend);
			}
		}
		private void OnLevelChanged(Character character, byte currentlevel, int difference)
		{
			Friend friend = this.TryGetFriend(character);
			if (friend == null)
			{
				FriendsBook.logger.Error<Character>("Sad, friend bound with character {0} is not found :(", character);
			}
			else
			{
				FriendHandler.SendFriendUpdateMessage(this.Owner.Client, friend);
				if (this.WarnOnLevel && character.Map != this.Owner.Map)
				{
					CharacterHandler.SendCharacterLevelUpInformationMessage(this.Owner.Client, character, character.Level);
				}
			}
		}
		private void OnCharacterLogout(Character character)
		{
			Friend friend = this.TryGetFriend(character);
			if (friend == null)
			{
				FriendsBook.logger.Error<Character>("Sad, friend bound with character {0} is not found :(", character);
			}
			else
			{
				friend.SetOffline();
			}
			character.LoggedOut -= new System.Action<Character>(this.OnCharacterLogout);
			character.LevelChanged -= new Character.LevelChangedHandler(this.OnLevelChanged);
			character.ContextChanged -= new Character.CharacterContextChangedHandler(this.OnContextChanged);
		}
		public void Load()
		{
			this.m_relations = new ConcurrentDictionary<int, AccountRelation>(ServerBase<WorldServer>.Instance.DBAccessor.Database.Query<AccountRelation>(string.Format(AccountRelationRelator.FetchByAccount, this.Owner.Account.Id), new object[0]).ToDictionary((AccountRelation x) => x.AccountId, (AccountRelation x) => x));
			foreach (AccountRelation current in this.m_relations.Values)
			{
				WorldAccount worldAccount = Singleton<World>.Instance.GetConnectedAccount(current.TargetId) ?? Singleton<AccountManager>.Instance.FindById(current.TargetId);
				if (worldAccount == null)
				{
					ServerBase<WorldServer>.Instance.DBAccessor.Database.Delete(current);
				}
				else
				{
					switch (current.Type)
					{
					case AccountRelationType.Friend:
						if (worldAccount.ConnectedCharacter.HasValue)
						{
							Character character = Singleton<World>.Instance.GetCharacter(worldAccount.ConnectedCharacter.Value);
							this.m_friends.TryAdd(worldAccount.Id, new Friend(current, worldAccount, character));
						}
						else
						{
							this.m_friends.TryAdd(worldAccount.Id, new Friend(current, worldAccount));
						}
						break;
					case AccountRelationType.Ignored:
						if (worldAccount.ConnectedCharacter.HasValue)
						{
							Character character = Singleton<World>.Instance.GetCharacter(worldAccount.ConnectedCharacter.Value);
							this.m_ignoreds.TryAdd(worldAccount.Id, new Ignored(current, worldAccount, false, character));
						}
						else
						{
							this.m_ignoreds.TryAdd(worldAccount.Id, new Ignored(current, worldAccount, false));
						}
						break;
					}
				}
			}
			Singleton<World>.Instance.CharacterJoined += new System.Action<Character>(this.OnCharacterLogIn);
		}
		public void Save()
		{
            Stump.ORM.Database database = ServerBase<WorldServer>.Instance.DBAccessor.Database;
			using (System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int, AccountRelation>> enumerator = this.m_relations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					System.Collections.Generic.KeyValuePair<int, AccountRelation> current = enumerator.Current;
					database.Save(current);
				}
				goto IL_63;
			}
			IL_49:
			AccountRelation poco;
			if (this.m_relationsToRemove.TryPop(out poco))
			{
				database.Delete(poco);
			}
			IL_63:
			if (this.m_relationsToRemove.Count == 0)
			{
				return;
			}
			goto IL_49;
		}
		public Friend TryGetFriend(Character character)
		{
			Friend friend;
			return this.m_friends.TryGetValue(character.Account.Id, out friend) ? friend : null;
		}
	}
}
