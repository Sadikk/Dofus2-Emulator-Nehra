using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Conditions.Criterions
{
	public class PvpRankCriterion : Criterion
	{
		public const string Identifier = "PP";
		public const string Identifier2 = "Pp";
		public sbyte Rank
		{
			get;
			set;
		}
		public override bool Eval(Character character)
		{
			return base.Compare<sbyte>(character.AlignmentGrade, this.Rank);
		}
		public override void Build()
		{
			sbyte rank;
			if (!sbyte.TryParse(base.Literal, out rank))
			{
				throw new System.Exception(string.Format("Cannot build PvpRankCriterion, {0} is not a valid rank", base.Literal));
			}
			this.Rank = rank;
		}
		public override string ToString()
		{
			return base.FormatToString("PP");
		}
	}
}
