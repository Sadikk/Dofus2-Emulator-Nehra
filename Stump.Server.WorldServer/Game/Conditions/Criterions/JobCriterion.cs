using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Conditions.Criterions
{
	public class JobCriterion : Criterion
	{
		public const string Identifier = "PJ";
		public const string Identifier2 = "Pj";
		public int Id
		{
			get;
			set;
		}
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
			int level = -1;
			int id;
			if (base.Literal.Contains(","))
			{
				string[] array = base.Literal.Split(new char[]
				{
					','
				});
				if (array.Length != 2 || !int.TryParse(array[0], out id) || !int.TryParse(array[1], out level))
				{
					throw new System.Exception(string.Format("Cannot build JobCriterion, {0} is not a valid job (format 'id,level')", base.Literal));
				}
			}
			if (!int.TryParse(base.Literal, out id))
			{
				throw new System.Exception(string.Format("Cannot build JobCriterion, {0} is not a valid job id", base.Literal));
			}
			this.Id = id;
			this.Level = level;
		}
		public override string ToString()
		{
			return base.FormatToString("PJ");
		}
	}
}
