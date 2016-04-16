using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Conditions.Criterions
{
	public class RankCriterion : Criterion
	{
		public const string Identifier = "Pq";
		public int Rank
		{
			get;
			set;
		}
		public override bool Eval(Character character)
		{
			return false;
		}
		public override void Build()
		{
			int rank;
			if (!int.TryParse(base.Literal, out rank))
			{
				throw new System.Exception(string.Format("Cannot build RankCriterion, {0} is not a valid rank", base.Literal));
			}
			this.Rank = rank;
		}
		public override string ToString()
		{
			return base.FormatToString("Pq");
		}
	}
}
