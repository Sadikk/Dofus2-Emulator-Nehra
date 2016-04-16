using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Conditions.Criterions
{
	public class KamaCriterion : Criterion
	{
		public const string Identifier = "PK";
		public int Kamas
		{
			get;
			set;
		}
		public override bool Eval(Character character)
		{
			return base.Compare<int>(character.Kamas, this.Kamas);
		}
		public override void Build()
		{
			int kamas;
			if (!int.TryParse(base.Literal, out kamas))
			{
				throw new System.Exception(string.Format("Cannot build KamaCriterion, {0} is not a valid kamas amount", base.Literal));
			}
			this.Kamas = kamas;
		}
		public override string ToString()
		{
			return base.FormatToString("PK");
		}
	}
}
