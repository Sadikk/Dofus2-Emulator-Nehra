using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer;
using Stump.Server.BaseServer.Database;
using Stump.Server.BaseServer.IPC;
using Stump.Server.BaseServer.IPC.Messages;
using Stump.Server.BaseServer.IPC.Objects;
using Stump.Server.WorldServer.Core.IPC;
using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Database.Accounts;
using System;
using System.Linq;

namespace Stump.Server.WorldServer.Game.Accounts
{
	public class AccountManager : DataManager<AccountManager>
	{
		public static UserGroup DefaultUserGroup = new UserGroup(new UserGroupData
		{
			Id = 0,
			IsGameMaster = false,
			Name = "Default",
			Role = RoleEnum.Player
		}, new UserGroupCommand[0]);

		private System.Collections.Generic.Dictionary<int, UserGroup> m_userGroups;

		public override void Initialize()
		{
			IPCAccessor.Instance.Granted += delegate(IPCAccessor accessor)
			{
				if (this.m_userGroups == null)
				{
					ServerBase<WorldServer>.Instance.IOTaskPool.ExecuteInContext(new Action(this.LoadUserGroups));
				}
			};
			base.Initialize();
		}
		public void LoadUserGroups()
		{
			if (!IPCAccessor.Instance.IsConnected)
			{
				throw new System.Exception("IPC not connected");
			}
			System.Threading.ManualResetEvent ev = new System.Threading.ManualResetEvent(false);
			System.Collections.Generic.IList<UserGroupData> groups = null;
			IPCErrorMessage errorMsg = null;
			IPCAccessor.Instance.SendRequest<GroupsListMessage>(new GroupsRequestMessage(), delegate(GroupsListMessage reply)
			{
				groups = reply.Groups;
				ev.Set();
			}, delegate(IPCErrorMessage error)
			{
				errorMsg = error;
				ev.Set();
			});
			ev.WaitOne();
			if (groups == null)
			{
				throw new System.Exception(string.Format("Cannot load groups : {0}", errorMsg.Message));
			}
			this.m_userGroups = (
				from x in groups
				select new UserGroup(x, base.Database.Query<UserGroupCommand>(string.Format(UserGroupCommandRelator.FetchById, x.Id), new object[0]).ToArray<UserGroupCommand>())).ToDictionary((UserGroup x) => x.Id, (UserGroup x) => x);
		}
		public UserGroup GetGroupOrDefault(int id)
		{
			UserGroup userGroup;
			return this.m_userGroups.TryGetValue(id, out userGroup) ? userGroup : AccountManager.DefaultUserGroup;
		}
		public WorldAccount CreateWorldAccount(WorldClient client)
		{
			WorldAccount worldAccount = new WorldAccount
			{
				Id = client.Account.Id,
				Nickname = client.Account.Nickname
			};
			base.Database.Insert(worldAccount);
			return worldAccount;
		}
		public WorldAccount FindById(int id)
		{
			return base.Database.FirstOrDefault<WorldAccount>(string.Format(WorldAccountRelator.FetchById, id), new object[0]);
		}
		public WorldAccount FindByNickname(string nickname)
		{
			return base.Database.FirstOrDefault<WorldAccount>(WorldAccountRelator.FetchByNickname, new object[]
			{
				nickname
			});
		}
		public bool DoesExist(int id)
		{
			return base.Database.ExecuteScalar<bool>(string.Format("SELECT EXISTS(SELECT 1 FROM accounts WHERE Id={0})", id), new object[0]);
		}
	}
}
