using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Conditions.Criterions
{
	public class AlignementLevelCriterion : Criterion
	{
		public const string Identifier = "Pa";
		public int Level
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
			int level;
			if (!int.TryParse(base.Literal, out level))
			{
				throw new System.Exception(string.Format("Cannot build AlignementLevelCriterion, {0} is not a valid alignement level", base.Literal));
			}
			this.Level = level;
		}
		public override string ToString()
		{
			return base.FormatToString("Pa");
		}
	}
}
