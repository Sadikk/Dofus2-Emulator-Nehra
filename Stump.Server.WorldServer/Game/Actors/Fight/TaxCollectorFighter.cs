using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.RolePlay.TaxCollectors;
using Stump.Server.WorldServer.Game.Actors.Stats;
using Stump.Server.WorldServer.Game.Fights;
using Stump.Server.WorldServer.Game.Fights.Teams;
using Stump.Server.WorldServer.Game.Maps.Cells;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Actors.Fight
{
	public sealed class TaxCollectorFighter : AIFighter
	{
		private readonly StatsFields m_stats;
		public TaxCollectorNpc TaxCollectorNpc
		{
			get;
			private set;
		}
		public override string Name
		{
			get
			{
				return this.TaxCollectorNpc.Name;
			}
		}
		public override ObjectPosition MapPosition
		{
			get
			{
				return this.TaxCollectorNpc.Position;
			}
		}
		public override short Level
		{
			get
			{
				return this.TaxCollectorNpc.Level;
			}
		}
		public override StatsFields Stats
		{
			get
			{
				return this.m_stats;
			}
		}
		public TaxCollectorFighter(FightTeam team, TaxCollectorNpc taxCollector) : base(team, taxCollector.Guild.GetTaxCollectorSpells(), taxCollector.GlobalId)
		{
			this.TaxCollectorNpc = taxCollector;
			this.Look = this.TaxCollectorNpc.Look.Clone();
			this.m_stats = new StatsFields(this);
			this.m_stats.Initialize(this.TaxCollectorNpc);
			Cell cell;
			if (base.Fight.FindRandomFreeCell(this, out cell, false))
			{
				this.Position = new ObjectPosition(this.TaxCollectorNpc.Map, cell, this.TaxCollectorNpc.Direction);
			}
		}
		public override string GetMapRunningFighterName()
		{
			return this.TaxCollectorNpc.Name;
		}
		public TaxCollectorFightersInformation GetTaxCollectorFightersInformation()
		{
			System.Collections.Generic.IEnumerable<CharacterMinimalPlusLookInformations> arg_81_0;
			if (base.Fight.State == FightState.Placement && base.Fight is FightPvT)
			{
				arg_81_0 = 
					from x in (base.Fight as FightPvT).DefendersQueue
					select x.GetCharacterMinimalPlusLookInformations();
			}
			else
			{
				arg_81_0 = 
					from x in base.Team.Fighters.OfType<CharacterFighter>()
					select x.Character.GetCharacterMinimalPlusLookInformations();
			}
			System.Collections.Generic.IEnumerable<CharacterMinimalPlusLookInformations> allyCharactersInformations = arg_81_0;
			return new TaxCollectorFightersInformation(this.TaxCollectorNpc.GlobalId, allyCharactersInformations, 
				from x in base.OpposedTeam.Fighters.OfType<CharacterFighter>()
				select x.Character.GetCharacterMinimalPlusLookInformations());
		}
		public override FightTeamMemberInformations GetFightTeamMemberInformations()
		{
            return new FightTeamMemberTaxCollectorInformations(this.Id, (ushort)this.TaxCollectorNpc.FirstNameId, (ushort)this.TaxCollectorNpc.LastNameId, (byte)this.TaxCollectorNpc.Level, (uint)TaxCollectorNpc.Guild.Id, (uint)this.TaxCollectorNpc.GlobalId);
		}
		public override GameFightFighterInformations GetGameFightFighterInformations(WorldClient client = null)
		{
            return new GameFightTaxCollectorInformations(this.Id, this.Look.GetEntityLook(), this.GetEntityDispositionInformations(client), (sbyte)base.Team.Id, 0, base.IsAlive(), this.GetGameFightMinimalStats(client), Enumerable.Empty<ushort>(), (ushort)this.TaxCollectorNpc.FirstNameId, (ushort)this.TaxCollectorNpc.LastNameId, (byte)this.TaxCollectorNpc.Level);
		}
        public override GameFightFighterLightInformations GetGameFightFighterLightInformations()
        {
            return new GameFightFighterTaxCollectorLightInformations(false, IsAlive(), Id, 0, (ushort)Level, 0, (ushort)TaxCollectorNpc.FirstNameId, (ushort)TaxCollectorNpc.LastNameId);
        }
	}
}
