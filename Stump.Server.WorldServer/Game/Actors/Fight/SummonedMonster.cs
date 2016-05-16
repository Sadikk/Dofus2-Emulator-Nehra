using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Enums.HomeMade;
using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Database.Monsters;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Stats;
using Stump.Server.WorldServer.Game.Fights.Teams;
using Stump.Server.WorldServer.Game.Maps.Cells;
using System.Linq;

namespace Stump.Server.WorldServer.Game.Actors.Fight
{
	public class SummonedMonster : SummonedFighter
	{
        //FIELDS
        private readonly StatsFields m_stats;
        private bool m_isVisibleInTimeLine;

        //PROPERTIES
        public MonsterGrade Monster
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
				return (short)this.Monster.Level;
			}
		}
		public override StatsFields Stats
		{
			get
			{
				return this.m_stats;
			}
		}
		public override string Name
		{
			get
			{
				return this.Monster.Template.Name;
			}
		}
        public bool IsTreeSummon
        {
            get;
            private set;
        }
        public bool IsLeafyTree
        {
            get
            {
                return this.HasState(SpellStatesEnum.Leafy);
            }
        }
        public override bool IsVisibleInTimeline
        {
            get
            {
                return this.m_isVisibleInTimeLine;
            }
        }

        //CONSTRUCTOR
        public SummonedMonster(int id, FightTeam team, FightActor summoner, MonsterGrade template, Cell cell, bool isVisibleTimeline = true, bool treeSummon = false)
            : base(id, team, template.Spells.ToArray(), summoner, cell)
        {
            this.m_isVisibleInTimeLine = isVisibleTimeline;
            this.IsTreeSummon = treeSummon;
            this.Monster = template;
            this.Look = this.Monster.Template.EntityLook;
            this.m_stats = new StatsFields(this);
            this.m_stats.Initialize(template);
            this.AdjustStats();
        }
        public SummonedMonster(int id, FightTeam team, FightActor summoner, MonsterGrade template, Cell cell)
            : base(id, team, template.Spells.ToArray(), summoner, cell)
        {
            this.Monster = template;
            this.Look = this.Monster.Template.EntityLook;
            this.m_stats = new StatsFields(this);
            this.m_stats.Initialize(template);
            this.AdjustStats();
        }

        private void AdjustStats()
		{
			this.m_stats.Health.Base = (int)((short)((double)this.m_stats.Health.Base * (1.0 + (double)base.Summoner.Level / 100.0)));
			this.m_stats.Intelligence.Base = (int)((short)((double)this.m_stats.Intelligence.Base * (1.0 + (double)base.Summoner.Level / 100.0)));
			this.m_stats.Chance.Base = (int)((short)((double)this.m_stats.Chance.Base * (1.0 + (double)base.Summoner.Level / 100.0)));
			this.m_stats.Strength.Base = (int)((short)((double)this.m_stats.Strength.Base * (1.0 + (double)base.Summoner.Level / 100.0)));
			this.m_stats.Agility.Base = (int)((short)((double)this.m_stats.Agility.Base * (1.0 + (double)base.Summoner.Level / 100.0)));
			this.m_stats.Wisdom.Base = (int)((short)((double)this.m_stats.Wisdom.Base * (1.0 + (double)base.Summoner.Level / 100.0)));
		}

		public override string GetMapRunningFighterName()
		{
			return this.Monster.Id.ToString(System.Globalization.CultureInfo.InvariantCulture);
		}
		public override FightTeamMemberInformations GetFightTeamMemberInformations()
		{
			return new FightTeamMemberMonsterInformations(this.Id, this.Monster.Template.Id, (sbyte)this.Monster.GradeId);
		}
		public override GameFightFighterInformations GetGameFightFighterInformations(WorldClient client = null)
		{
            return new GameFightMonsterInformations(this.Id, this.Look.GetEntityLook(), this.GetEntityDispositionInformations(client), base.Team.Id, 0, base.IsAlive(), this.GetGameFightMinimalStats(client), Enumerable.Empty<ushort>(), (ushort)this.Monster.Template.Id, (sbyte)this.Monster.GradeId);
		}
		public override GameFightMinimalStats GetGameFightMinimalStats(WorldClient client = null)
		{
            return new GameFightMinimalStats((uint)this.Stats.Health.Total, (uint)this.Stats.Health.TotalMax, (uint)this.Stats.Health.Base, (uint)this.Stats[PlayerFields.PermanentDamagePercent].Total, 0u, (short)this.Stats.AP.Total, (short)this.Stats.AP.TotalMax, (short)this.Stats.MP.Total, (short)this.Stats.MP.TotalMax, base.Summoner.Id, true, (short)this.Stats[PlayerFields.NeutralResistPercent].Total, (short)this.Stats[PlayerFields.EarthResistPercent].Total, (short)this.Stats[PlayerFields.WaterResistPercent].Total, (short)this.Stats[PlayerFields.AirResistPercent].Total, (short)this.Stats[PlayerFields.FireResistPercent].Total, (short)this.Stats[PlayerFields.NeutralElementReduction].Total, (short)this.Stats[PlayerFields.EarthElementReduction].Total, (short)this.Stats[PlayerFields.WaterElementReduction].Total, (short)this.Stats[PlayerFields.AirElementReduction].Total, (short)this.Stats[PlayerFields.FireElementReduction].Total, (short)this.Stats[PlayerFields.PushDamageReduction].Total, (short)this.Stats[PlayerFields.CriticalDamageReduction].Total, (ushort)this.Stats[PlayerFields.DodgeAPProbability].Total, (ushort)this.Stats[PlayerFields.DodgeMPProbability].Total, (short)this.Stats[PlayerFields.TackleBlock].Total, (short)this.Stats[PlayerFields.TackleEvade].Total, (sbyte)((client == null) ? base.VisibleState : base.GetVisibleStateFor(client.Character)));
		}
        public override GameFightFighterLightInformations GetGameFightFighterLightInformations()
        {
            return new GameFightFighterMonsterLightInformations(false, IsAlive(), Id, 0, (ushort)Level, 0, (ushort)Monster.Template.Id);
        }
        protected override void PostDead()
        {
            if(this.IsTreeSummon)
            {
                this.Summoner.SpawnTreeAfterSummonDeath(this.Cell);
            }
        }
    }
}
