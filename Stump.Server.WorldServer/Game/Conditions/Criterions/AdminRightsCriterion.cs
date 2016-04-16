using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Conditions.Criterions
{
	public class AdminRightsCriterion : Criterion
	{
		public const string Identifier = "PX";
		public RoleEnum Role
		{
			get;
			set;
		}
		public override bool Eval(Character character)
		{
			return base.Compare<int>((int)character.UserGroup.Role, (int)this.Role);
		}
		public override void Build()
		{
			if (base.Literal == "G")
			{
				this.Role = RoleEnum.Player;
			}
			int role;
			if (!int.TryParse(base.Literal, out role))
			{
				throw new System.Exception(string.Format("Cannot build AdminRightsCriterion, {0} is not a valid role", base.Literal));
			}
			this.Role = (RoleEnum)role;
		}
		public override string ToString()
		{
			return base.FormatToString("PX");
		}
	}
}
