using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Conditions.Criterions
{
	public class LevelCriterion : Criterion
	{
		public const string Identifier = "PL";
		public byte Level
		{
			get;
			set;
		}
		public override bool Eval(Character character)
		{
			return base.Compare<byte>(character.Level, this.Level);
		}
		public override void Build()
		{
			byte level;
			if (!byte.TryParse(base.Literal, out level))
			{
				throw new System.Exception(string.Format("Cannot build LevelCriterion, {0} is not a valid level", base.Literal));
			}
			this.Level = level;
		}
		public override string ToString()
		{
			return base.FormatToString("PL");
		}
	}
}
