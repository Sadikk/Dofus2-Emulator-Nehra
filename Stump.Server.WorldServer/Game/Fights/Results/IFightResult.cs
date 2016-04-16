using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Game.Fights.Teams;

namespace Stump.Server.WorldServer.Game.Fights.Results
{
	public interface IFightResult
	{
		bool Alive
		{
			get;
		}
		int Id
		{
			get;
		}
		int Prospecting
		{
			get;
		}
		int Wisdom
		{
			get;
		}
		int Level
		{
			get;
		}
		FightLoot Loot
		{
			get;
		}
		FightOutcomeEnum Outcome
		{
			get;
		}
		Fight Fight
		{
			get;
		}
		bool CanLoot(FightTeam looters);
		FightResultListEntry GetFightResultListEntry();
		void Apply();
	}
}
