using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Fights.Teams
{
	public class FightPlayerTeam : FightTeamWithLeader<CharacterFighter>
	{
		public override TeamTypeEnum TeamType
		{
			get
			{
				return TeamTypeEnum.TEAM_TYPE_PLAYER;
			}
		}
		public FightPlayerTeam(sbyte id, Cell[] placementCells) : base(id, placementCells)
		{
		}
		public FightPlayerTeam(sbyte id, Cell[] placementCells, AlignmentSideEnum alignmentSide) : base(id, placementCells, alignmentSide)
		{
		}
		public override FighterRefusedReasonEnum CanJoin(Character character)
		{
			FighterRefusedReasonEnum result;
			if (base.IsRestrictedToParty && (!base.Leader.Character.IsInParty() || !base.Leader.Character.Party.IsInGroup(character)))
			{
				result = FighterRefusedReasonEnum.TEAM_LIMITED_BY_MAINCHARACTER;
			}
			else
			{
				result = base.CanJoin(character);
			}
			return result;
		}
	}
}
