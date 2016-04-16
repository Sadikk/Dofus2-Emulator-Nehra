using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Fights.Teams
{
	public class FightMonsterTeam : FightTeamWithLeader<MonsterFighter>
	{
		public override TeamTypeEnum TeamType
		{
			get
			{
				return TeamTypeEnum.TEAM_TYPE_MONSTER;
			}
		}
		public FightMonsterTeam(sbyte id, Cell[] placementCells) : base(id, placementCells)
		{
		}
		public FightMonsterTeam(sbyte id, Cell[] placementCells, AlignmentSideEnum alignmentSide) : base(id, placementCells, alignmentSide)
		{
		}
		public override FighterRefusedReasonEnum CanJoin(Character character)
		{
			FighterRefusedReasonEnum result;
			if (!character.IsGameMaster())
			{
				result = FighterRefusedReasonEnum.WRONG_ALIGNMENT;
			}
			else
			{
				result = base.CanJoin(character);
			}
			return result;
		}
	}
}
