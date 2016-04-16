using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Items.Player;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Conditions.Criterions
{
	public class HasItemCriterion : Criterion
	{
		public const string Identifier = "PO";
		public int Item
		{
			get;
			set;
		}
		public override bool Eval(Character character)
		{
			bool result;
			if (base.Operator == ComparaisonOperatorEnum.EQUALS)
			{
				result = character.Inventory.Any((BasePlayerItem entry) => entry.Template.Id == this.Item);
			}
			else
			{
				result = (base.Operator != ComparaisonOperatorEnum.INEQUALS || character.Inventory.All((BasePlayerItem entry) => entry.Template.Id != this.Item));
			}
			return result;
		}
		public override void Build()
		{
			int item;
			if (!int.TryParse(base.Literal, out item))
			{
				throw new System.Exception(string.Format("Cannot build HasItemCriterion, {0} is not a valid item id", base.Literal));
			}
			this.Item = item;
		}
		public override string ToString()
		{
			return base.FormatToString("PO");
		}
	}
}
