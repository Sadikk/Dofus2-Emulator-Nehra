using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Conditions.Criterions
{
	public class EmoteCriterion : Criterion
	{
		public const string Identifier = "PE";
		public int Emote
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
			int emote;
			if (!int.TryParse(base.Literal, out emote))
			{
				throw new System.Exception(string.Format("Cannot build EmoteCriterion, {0} is not a valid emote id", base.Literal));
			}
			this.Emote = emote;
		}
		public override string ToString()
		{
			return base.FormatToString("PE");
		}
	}
}
