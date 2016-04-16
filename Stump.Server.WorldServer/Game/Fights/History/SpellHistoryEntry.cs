using Stump.Server.WorldServer.Database.Spells;
using Stump.Server.WorldServer.Game.Actors.Fight;

namespace Stump.Server.WorldServer.Game.Fights.History
{
	public class SpellHistoryEntry
	{
		public SpellHistory History
		{
			get;
			private set;
		}
		public SpellLevelTemplate Spell
		{
			get;
			private set;
		}
		public FightActor Caster
		{
			get;
			private set;
		}
		public FightActor Target
		{
			get;
			private set;
		}
		public int CastRound
		{
			get;
			private set;
		}
		public SpellHistoryEntry(SpellHistory history, SpellLevelTemplate spell, FightActor caster, FightActor target, int castRound)
		{
			this.History = history;
			this.Spell = spell;
			this.Caster = caster;
			this.Target = target;
			this.CastRound = castRound;
		}
		public int GetElapsedRounds(int currentRound)
		{
			return currentRound - this.CastRound;
		}
		public bool IsGlobalCooldownActive(int currentRound)
		{
			return (long)this.GetElapsedRounds(currentRound) < (long)((ulong)this.Spell.MinCastInterval);
		}
	}
}
