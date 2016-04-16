using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Conditions.Criterions
{
	public class FriendListCriterion : Criterion
	{
		public const string Identifier = "Pb";
		public int Friend
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
			int friend;
			if (!int.TryParse(base.Literal, out friend))
			{
				throw new System.Exception(string.Format("Cannot build FriendListCriterion, {0} is not a valid friend id", base.Literal));
			}
			this.Friend = friend;
		}
		public override string ToString()
		{
			return base.FormatToString("Pb");
		}
	}
}
