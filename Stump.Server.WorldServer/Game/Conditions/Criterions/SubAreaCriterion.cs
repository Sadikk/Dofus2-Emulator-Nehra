using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Conditions.Criterions
{
	public class SubAreaCriterion : Criterion
	{
		public const string Identifier = "PB";
		public int SubArea
		{
			get;
			set;
		}
		public override bool Eval(Character character)
		{
			return base.Compare<int>(character.SubArea.Id, this.SubArea);
		}
		public override void Build()
		{
			int subArea;
			if (!int.TryParse(base.Literal, out subArea))
			{
				throw new System.Exception(string.Format("Cannot build SubAreaCriterion, {0} is not a valid subarea id", base.Literal));
			}
			this.SubArea = subArea;
		}
		public override string ToString()
		{
			return base.FormatToString("PB");
		}
	}
}
