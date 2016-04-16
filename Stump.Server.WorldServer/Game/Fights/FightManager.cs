using Stump.Core.Pool;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Game.Fights.Teams;
using Stump.Server.WorldServer.Game.Maps;

namespace Stump.Server.WorldServer.Game.Fights
{
	public class FightManager : EntityManager<FightManager, Fight>
	{
		private readonly UniqueIdProvider m_idProvider = new UniqueIdProvider();
		public Fight CreateDuel(Map map)
		{
			FightPlayerTeam redTeam = new FightPlayerTeam(0, map.GetRedFightPlacement());
			FightPlayerTeam blueTeam = new FightPlayerTeam(1, map.GetBlueFightPlacement());
			FightDuel fightDuel = new FightDuel(this.m_idProvider.Pop(), map, blueTeam, redTeam);
			base.AddEntity(fightDuel.Id, fightDuel);
			return fightDuel;
		}
		public Fight CreatePvMFight(Map map)
		{
			FightPlayerTeam redTeam = new FightPlayerTeam(0, map.GetRedFightPlacement());
			FightMonsterTeam blueTeam = new FightMonsterTeam(1, map.GetBlueFightPlacement());
			FightPvM fightPvM = new FightPvM(this.m_idProvider.Pop(), map, blueTeam, redTeam);
			base.AddEntity(fightPvM.Id, fightPvM);
			return fightPvM;
		}
		public Fight CreateAgressionFight(Map map, AlignmentSideEnum redAlignment, AlignmentSideEnum blueAlignment)
		{
			FightPlayerTeam redTeam = new FightPlayerTeam(0, map.GetRedFightPlacement(), redAlignment);
			FightPlayerTeam blueTeam = new FightPlayerTeam(1, map.GetBlueFightPlacement(), blueAlignment);
			FightAgression fightAgression = new FightAgression(this.m_idProvider.Pop(), map, blueTeam, redTeam);
			base.AddEntity(fightAgression.Id, fightAgression);
			return fightAgression;
		}
		public FightPvT CreatePvTFight(Map map)
		{
			FightTaxCollectorAttackersTeam redTeam = new FightTaxCollectorAttackersTeam(0, map.GetRedFightPlacement());
			FightTaxCollectorDefenderTeam blueTeam = new FightTaxCollectorDefenderTeam(1, map.GetBlueFightPlacement());
			FightPvT fightPvT = new FightPvT(this.m_idProvider.Pop(), map, blueTeam, redTeam);
			base.AddEntity(fightPvT.Id, fightPvT);
			return fightPvT;
		}
		public void Remove(Fight fight)
		{
			base.RemoveEntity(fight.Id);
			this.m_idProvider.Push(fight.Id);
		}
		public Fight GetFight(int id)
		{
			return base.GetEntityOrDefault(id);
		}
	}
}
