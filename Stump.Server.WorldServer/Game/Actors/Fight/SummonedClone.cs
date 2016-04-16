using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Stats;
using Stump.Server.WorldServer.Game.Maps.Cells;
using Stump.Server.WorldServer.Game.Spells;

namespace Stump.Server.WorldServer.Game.Actors.Fight
{
	public class SummonedClone : SummonedFighter
	{
		private readonly StatsFields m_stats;
		public FightActor Caster
		{
			get;
			private set;
		}
		public override ObjectPosition MapPosition
		{
			get
			{
				return this.Position;
			}
		}
		public override short Level
		{
			get
			{
				return this.Caster.Level;
			}
		}
		public override string Name
		{
			get
			{
				return (this.Caster is NamedFighter) ? ((NamedFighter)this.Caster).Name : "(no name)";
			}
		}
		public override StatsFields Stats
		{
			get
			{
				return this.m_stats;
			}
		}
		public SummonedClone(int id, FightActor caster, Cell cell) : base(id, caster.Team, new System.Collections.Generic.List<Spell>(), caster, cell)
		{
			this.Caster = caster;
			this.Look = caster.Look.Clone();
			this.m_stats = caster.Stats.CloneAndChangeOwner(this);
		}
		public override string GetMapRunningFighterName()
		{
			return this.Name;
		}
		public override GameFightFighterInformations GetGameFightFighterInformations()
		{
			GameFightFighterInformations gameFightFighterInformations = this.Caster.GetGameFightFighterInformations();
			gameFightFighterInformations.contextualId = this.Id;
			return gameFightFighterInformations;
		}
		public override FightTeamMemberInformations GetFightTeamMemberInformations()
		{
			FightTeamMemberInformations fightTeamMemberInformations = this.Caster.GetFightTeamMemberInformations();
			fightTeamMemberInformations.id = this.Id;
			return fightTeamMemberInformations;
		}
        public override GameFightFighterLightInformations GetGameFightFighterLightInformations()
        {
            return this.Caster.GetGameFightFighterLightInformations();
        }
	}
}
