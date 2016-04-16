using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Conditions.Criterions
{
	public class SubscribeCriterion : Criterion
	{
		public const string Identifier = "PZ";
		public bool SubscriptionState
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
			int num;
			if (!int.TryParse(base.Literal, out num))
			{
				throw new System.Exception(string.Format("Cannot build SubscribeCriterion, {0} is not a valid subscription state (1 or 0)", base.Literal));
			}
			this.SubscriptionState = (num != 0);
		}
		public override string ToString()
		{
			return base.FormatToString("PZ");
		}
	}
}
