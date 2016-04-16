using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Conditions.Criterions
{
	public class MonthCriterion : Criterion
	{
		public const string Identifier = "SG";
		public int Month
		{
			get;
			set;
		}
		public override bool Eval(Character character)
		{
			return base.Compare<int>(System.DateTime.Now.Month, this.Month);
		}
		public override void Build()
		{
			int num;
			if (!int.TryParse(base.Literal, out num) || num < 1 || num > 12)
			{
				throw new System.Exception(string.Format("Cannot build MonthCriterion, {0} is not a valid month", base.Literal));
			}
			this.Month = num;
		}
		public override string ToString()
		{
			return base.FormatToString("SG");
		}
	}
}
