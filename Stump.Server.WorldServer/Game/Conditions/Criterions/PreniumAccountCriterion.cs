using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Conditions.Criterions
{
	public class PreniumAccountCriterion : Criterion
	{
		public const string Identifier = "Pe";
		public bool IsPrenium
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
			if (!int.TryParse(base.Literal, out num))
			{
				throw new System.Exception(string.Format("Cannot build PreniumAccountCriterion, {0} is not a valid prenium id", base.Literal));
			}
			this.IsPrenium = (num == 1);
		}
		public override string ToString()
		{
			return base.FormatToString("Pe");
		}
	}
}
