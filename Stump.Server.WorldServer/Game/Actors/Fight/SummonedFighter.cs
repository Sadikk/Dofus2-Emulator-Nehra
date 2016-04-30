using Stump.Core.Reflection;
using Stump.Server.WorldServer.Database.I18n;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Fights.Teams;
using Stump.Server.WorldServer.Game.Spells;
using Stump.Server.WorldServer.Handlers.Context;
using System.Collections.Generic;

namespace Stump.Server.WorldServer.Game.Actors.Fight
{
	public abstract class SummonedFighter : AIFighter
	{
		public sealed override int Id
		{
			get;
			protected set;
		}
        public FightActor Summoner
		{
			get;
			protected set;
		}

	    protected SummonedFighter(int id, FightTeam team, IEnumerable<Spell> spells,
	        FightActor summoner, Cell cell) : base(team, spells)
        {
            base.Position = summoner.Position.Clone();
            base.Cell = cell;

	        this.Id = id;
	        this.Summoner = summoner;
	    }

	    public override int GetTackledAP()
		{
			return 0;
		}
		public override int GetTackledMP()
		{
			return 0;
		}
		protected override void OnDead(FightActor killedBy)
		{
			base.OnDead(killedBy);
			base.Fight.TimeLine.RemoveFighter(this);
			this.Summoner.RemoveSummon(this);
			ContextHandler.SendGameFightTurnListMessage(base.Fight.Clients, base.Fight);
		}
	}
}
