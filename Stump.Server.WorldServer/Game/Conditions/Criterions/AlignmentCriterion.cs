using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Conditions.Criterions
{
	public class AlignmentCriterion : Criterion
	{
		public const string Identifier = "Ps";
		public override bool Eval(Character character)
		{
			return true;
		}
		public override void Build()
		{
			int num;
			if (!int.TryParse(base.Literal, out num))
			{
				throw new System.Exception(string.Format("Cannot build AlignmentCriterion, {0} is not a valid alignement id", base.Literal));
			}
		}
		public override string ToString()
		{
			return base.FormatToString("Ps");
		}
	}
}
