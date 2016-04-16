using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Fights;

namespace Stump.Server.WorldServer.AI.Fights.Threat
{
	public class ThreatCalculator
	{
		public AIFighter Fighter
		{
			get;
			private set;
		}
		public Fight Fight
		{
			get
			{
				return this.Fighter.Fight;
			}
		}
		public ThreatCalculator(AIFighter fighter)
		{
			this.Fighter = fighter;
		}
		public float CalculateThreat(FightActor fighter)
		{
			return 0f;
		}
	}
}
