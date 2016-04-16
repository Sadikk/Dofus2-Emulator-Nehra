using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using System;
namespace Stump.Server.WorldServer.Game.Conditions.Criterions
{
	public class BonesCriterion : Criterion
	{
		public const string Identifier = "PU";
		public short BonesId
		{
			get;
			set;
		}
		public override bool Eval(Character character)
		{
			return base.Compare<short>(character.Look.BonesID, this.BonesId);
		}
		public override void Build()
		{
			if (base.Literal == "B")
			{
				this.BonesId = 1;
			}
			else
			{
				short num;
				if (!short.TryParse(base.Literal, out num))
				{
					throw new System.Exception(string.Format("Cannot build BonesCriterion, {0} is not a valid bones id", base.Literal));
				}
				this.BonesId = Convert.ToInt16((num != 0) ? num : 1);
			}
		}
		public override string ToString()
		{
			return base.FormatToString("PU");
		}
	}
}
