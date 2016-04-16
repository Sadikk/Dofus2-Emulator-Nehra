using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Fights.Teams;

namespace Stump.Server.WorldServer.Game.Fights.Results
{
	public class FightResult<T> : IFightResult where T : FightActor
	{
		public T Fighter
		{
			get;
			protected set;
		}
		public bool Alive
		{
			get
			{
				T fighter = this.Fighter;
				bool arg_30_0;
				if (fighter.IsAlive())
				{
					fighter = this.Fighter;
					arg_30_0 = !fighter.HasLeft();
				}
				else
				{
					arg_30_0 = false;
				}
				return arg_30_0;
			}
		}
		public int Id
		{
			get
			{
				T fighter = this.Fighter;
				return fighter.Id;
			}
		}
		public int Prospecting
		{
			get
			{
				T fighter = this.Fighter;
				return fighter.Stats[PlayerFields.Prospecting].Total;
			}
		}
		public int Wisdom
		{
			get
			{
				T fighter = this.Fighter;
				return fighter.Stats[PlayerFields.Wisdom].Total;
			}
		}
		public int Level
		{
			get
			{
				T fighter = this.Fighter;
				return (int)fighter.Level;
			}
		}
		public Fight Fight
		{
			get
			{
				T fighter = this.Fighter;
				return fighter.Fight;
			}
		}
		public FightLoot Loot
		{
			get;
			protected set;
		}
		public FightOutcomeEnum Outcome
		{
			get;
			protected set;
		}
		public FightResult(T fighter, FightOutcomeEnum outcome, FightLoot loot)
		{
			this.Fighter = fighter;
			this.Outcome = outcome;
			this.Loot = loot;
		}
		public virtual bool CanLoot(FightTeam team)
		{
			return false;
		}
		public virtual FightResultListEntry GetFightResultListEntry()
		{
			return new FightResultFighterListEntry((ushort)this.Outcome, 0, this.Loot.GetFightLoot(), this.Id, this.Alive);
		}
		public virtual void Apply()
		{
		}
	}
	public class FightResult : FightResult<FightActor>
	{
		public FightResult(FightActor fighter, FightOutcomeEnum outcome, FightLoot loot) : base(fighter, outcome, loot)
		{
		}
	}
}
