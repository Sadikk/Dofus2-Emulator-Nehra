using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.BaseServer.IPC.Objects;
using System.Linq;
namespace Stump.Server.WorldServer.Database.Accounts
{
	public class UserGroup
	{
		private readonly UserGroupData m_data;
		public int Id
		{
			get
			{
				return this.m_data.Id;
			}
		}
		public UserGroupCommand[] AvailableCommands
		{
			get;
			private set;
		}
		public string Name
		{
			get
			{
				return this.m_data.Name;
			}
		}
		public RoleEnum Role
		{
			get
			{
				return this.m_data.Role;
			}
		}
		public bool IsGameMaster
		{
			get
			{
				return this.m_data.IsGameMaster;
			}
		}
		public UserGroup(UserGroupData data, UserGroupCommand[] commands)
		{
			this.m_data = data;
			this.AvailableCommands = commands;
		}
		public bool IsCommandAvailable(CommandBase command)
		{
			return this.Role >= command.RequiredRole || this.AvailableCommands.Any((UserGroupCommand x) => command.Aliases.Contains(x.CommandAlias));
		}
	}
}
