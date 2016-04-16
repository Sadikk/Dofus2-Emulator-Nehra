using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Conditions.Criterions
{
	public class MaxRankCriterion : Criterion
	{
		public const string Identifier = "P¨Q";
		public override bool Eval(Character character)
		{
			return true;
		}
		public override void Build()
		{
		}
		public override string ToString()
		{
			return base.FormatToString("P¨Q");
		}
	}
}
