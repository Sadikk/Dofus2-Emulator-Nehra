using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Conditions.Criterions
{
	public class StaticCriterion : Criterion
	{
		public const string Identifier = "Sc";
		public override bool Eval(Character character)
		{
			return true;
		}
		public override void Build()
		{
		}
		public override string ToString()
		{
			return base.FormatToString("Sc");
		}
	}
}
