using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;

namespace Commands.Commands
{
    public class AllianceCommand : SubCommandContainer
    {
        public AllianceCommand()
        {
            base.Aliases = new string[]
            {
                "alliance"
            };

            base.RequiredRole = RoleEnum.GameMaster;
            base.Description = "Provides many commands to manage alliances";
        }
    }
}
