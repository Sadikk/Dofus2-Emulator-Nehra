using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Conditions.Criterions
{
	public class MariedCriterion : Criterion
	{
		public const string Identifier = "PR";
		public bool Married
		{
			get;
			set;
		}
		public override bool Eval(Character character)
		{
			return true;
		}
		public override void Build()
		{
			int num;
			if (!int.TryParse(base.Literal, out num) || (num != 1 && num != 2))
			{
				throw new System.Exception(string.Format("Cannot build MariedCriterion, {0} is not a valid married condition (1 or 2)", base.Literal));
			}
			this.Married = (num == 1);
		}
		public override string ToString()
		{
			return base.FormatToString("PR");
		}
	}
}
