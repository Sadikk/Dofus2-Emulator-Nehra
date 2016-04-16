using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Conditions.Criterions
{
	public class NameCriterion : Criterion
	{
		public const string Identifier = "PN";
		public string Name
		{
			get;
			set;
		}
		public override bool Eval(Character character)
		{
			return base.Compare(character.Name, this.Name);
		}
		public override void Build()
		{
			this.Name = base.Literal;
		}
		public override string ToString()
		{
			return base.FormatToString("PN");
		}
	}
}
