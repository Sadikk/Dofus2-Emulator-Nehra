using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Fights.Teams
{
	public class FightTaxCollectorAttackersTeam : FightTeamWithLeader<CharacterFighter>
	{
		public override TeamTypeEnum TeamType
		{
			get
			{
				return TeamTypeEnum.TEAM_TYPE_PLAYER;
			}
		}
		public FightTaxCollectorAttackersTeam(sbyte id, Cell[] placementCells) : base(id, placementCells)
		{
		}
		public FightTaxCollectorAttackersTeam(sbyte id, Cell[] placementCells, AlignmentSideEnum alignmentSide) : base(id, placementCells, alignmentSide)
		{
		}
		public override FighterRefusedReasonEnum CanJoin(Character character)
		{
			FighterRefusedReasonEnum result;
			if (!character.IsGameMaster() && base.Fight is FightPvT && character.Guild == (base.Fight as FightPvT).TaxCollector.TaxCollectorNpc.Guild)
			{
				result = FighterRefusedReasonEnum.WRONG_GUILD;
			}
			else
			{
				result = base.CanJoin(character);
			}
			return result;
		}
	}
}
