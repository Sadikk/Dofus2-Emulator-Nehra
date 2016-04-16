using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using System.Linq;

namespace Stump.Server.WorldServer.Game.Parties
{
    public sealed class ArenaParty : Party
    {
        // FIELDS

        // PROPERTIES
        public override PartyTypeEnum Type
        {
            get
            {
                return PartyTypeEnum.PARTY_TYPE_ARENA;
            }
        }
        public override short MaxMemberCount
        {
            get
            {
                return 3;
            }
        }

        // CONSTRUCTORS
        public ArenaParty(int id, Character leader)
            : base(id, leader)
        {
        }

        // METHODS
        public short GetHiddenRankSum()
        {
            return (short)this.Members.Sum(entry => entry.Record.HiddenArenaRank);
        }
        public short GetMinHiddenRank()
        {
            return this.Members.Min(entry => entry.Record.HiddenArenaRank);
        }
        public short GetMaxHiddenRank()
        {
            return this.Members.Min(entry => entry.Record.HiddenArenaRank);
        }

        public bool IsTag()
        {
            return this.Members.Any(entry => entry.ArenaInvitation != null);
        }
    }
}
