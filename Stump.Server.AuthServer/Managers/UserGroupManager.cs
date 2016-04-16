using NLog;
using Stump.Server.AuthServer.Database;
using Stump.Server.BaseServer.Database;
using Stump.Server.BaseServer.IPC.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stump.Server.AuthServer.Managers
{
    public class UserGroupManager : DataManager<UserGroupManager>
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private List<UserGroup> _groups;

        public override void Initialize()
        {
            base.Initialize();
            _groups = Database.Fetch<UserGroup>(UserGroupRelator.FetchQuery);
            if (!_groups.Any())
            {
                logger.Info("Create default groups");
                Database.Insert(new UserGroup() { Id = 1, IsGameMaster = false, Name = "Player", Role = DofusProtocol.Enums.RoleEnum.Player });
                Database.Insert(new UserGroup() { Id = 2, IsGameMaster = true, Name = "Moderator", Role = DofusProtocol.Enums.RoleEnum.Moderator });
                Database.Insert(new UserGroup() { Id = 3, IsGameMaster = true, Name = "GameMaster", Role = DofusProtocol.Enums.RoleEnum.GameMaster });
                Database.Insert(new UserGroup() { Id = 3, IsGameMaster = true, Name = "Administrator", Role = DofusProtocol.Enums.RoleEnum.Administrator });
                _groups = Database.Fetch<UserGroup>(UserGroupRelator.FetchQuery);
            }
        }

        public List<UserGroupData> Users
        {
            get
            {
                lock(_groups)
                {
                    return _groups.Select(entry => entry.Data).ToList();
                }
            }
        }
    }
}
