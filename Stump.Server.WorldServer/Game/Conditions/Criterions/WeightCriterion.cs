using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Conditions.Criterions
{
	public class WeightCriterion : Criterion
	{
		public const string Identifier = "PW";
		public int Weight
		{
			get;
			set;
		}
		public override bool Eval(Character character)
		{
			return base.Compare<int>(character.Inventory.Weight, this.Weight);
		}
		public override void Build()
		{
			int weight;
			if (!int.TryParse(base.Literal, out weight))
			{
				throw new System.Exception(string.Format("Cannot build WeightCriterion, {0} is not a valid weight", base.Literal));
			}
			this.Weight = weight;
		}
		public override string ToString()
		{
			return base.FormatToString("PW");
		}
	}
}
