using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Conditions.Criterions
{
	public class SexCriterion : Criterion
	{
		public const string Identifier = "PS";
		public int Sex
		{
			get;
			set;
		}
		public override bool Eval(Character character)
		{
			return base.Compare<int>((int)character.Sex, this.Sex);
		}
		public override void Build()
		{
			int sex;
			if (!int.TryParse(base.Literal, out sex))
			{
				throw new System.Exception(string.Format("Cannot build SexCriterion, {0} is not a valid sex", base.Literal));
			}
			this.Sex = sex;
		}
		public override string ToString()
		{
			return base.FormatToString("PS");
		}
	}
}
