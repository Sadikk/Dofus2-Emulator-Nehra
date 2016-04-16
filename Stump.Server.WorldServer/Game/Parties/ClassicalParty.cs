using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Parties
{
    public sealed class ClassicalParty : Party
    {
        // FIELDS
        
        // PROPERTIES
        public override PartyTypeEnum Type
        {
            get
            {
                return PartyTypeEnum.PARTY_TYPE_CLASSICAL;
            }
        }
        public override short MaxMemberCount
        {
            get
            {
                return 8;
            }
        }

        // CONSTRUCTORS
        public ClassicalParty(int id, Character leader)
            : base(id, leader)
        {
        }

        // METHODS
    }
}
