using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.BaseServer.Commands.Commands;
using Stump.Server.WorldServer.Commands.Commands.Patterns;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Spells;
using System.Linq;
namespace Stump.Server.WorldServer.Commands.Commands
{
	public class InfoCharacterCommand : TargetSubCommand
	{
		public InfoCharacterCommand()
		{
			base.Aliases = new string[]
			{
				"character",
				"char",
				"player"
			};
			base.RequiredRole = RoleEnum.GameMaster;
			base.Description = "Give informations about a player";
			base.ParentCommand = typeof(InfoCommand);
			base.AddTargetParameter(false, "Defined target");
			base.AddParameter<bool>("stats", "s", "Gives informations about his stats too", false, true, null);
		}
		public override void Execute(TriggerBase trigger)
		{
			Character[] targets = base.GetTargets(trigger);
			for (int i = 0; i < targets.Length; i++)
			{
				Character character = targets[i];
				trigger.ReplyBold("{0} ({1})", new object[]
				{
					character.Name,
					character.Id
				});
				trigger.ReplyBold("Account : {0}/{1} ({2}) - {3}", new object[]
				{
					character.Account.Login,
					character.Account.Nickname,
					character.Account.Id,
					character.UserGroup.Name
				});
				trigger.ReplyBold("Ip : {0}", new object[]
				{
					character.Client.IP
				});
				trigger.ReplyBold("Level : {0}", new object[]
				{
					character.Level
				});
				trigger.ReplyBold("Map : {0}, Cell : {1}, Direction : {2}", new object[]
				{
					character.Map.Id,
					character.Cell.Id,
					character.Direction
				});
				trigger.ReplyBold("Kamas : {0}", new object[]
				{
					character.Inventory.Kamas
				});
				trigger.ReplyBold("Items : {0}", new object[]
				{
					character.Inventory.Count
				});
				trigger.ReplyBold("Spells : {0}", new object[]
				{
					character.Spells.Count<CharacterSpell>()
				});
				trigger.ReplyBold("Tokens : {0}", new object[]
				{
					character.Account.Tokens
				});
				trigger.ReplyBold("Fight : {0}", new object[]
				{
					character.IsFighting() ? character.Fight.Id.ToString(System.Globalization.CultureInfo.InvariantCulture) : "Not fighting"
				});
				if (!trigger.Get<bool>("stats"))
				{
					return;
				}
				trigger.ReplyBold("Spells Points : {0}, Stats Points : {1}", new object[]
				{
					character.SpellsPoints,
					character.StatsPoints
				});
				trigger.ReplyBold("Health : {0}/{1}", new object[]
				{
					character.Stats.Health.Total,
					character.Stats.Health.TotalMax
				});
				trigger.ReplyBold("AP : {0}, PM : {1}", new object[]
				{
					character.Stats.AP,
					character.Stats.MP
				});
				trigger.ReplyBold("Vitality : {0}, Wisdom : {1}", new object[]
				{
					character.Stats.Vitality,
					character.Stats.Wisdom
				});
				trigger.ReplyBold("Strength : {0}, Intelligence : {1}", new object[]
				{
					character.Stats.Strength,
					character.Stats.Intelligence
				});
				trigger.ReplyBold("Agility : {0}, Chance : {1}", new object[]
				{
					character.Stats.Agility,
					character.Stats.Chance
				});
			}
		}
	}
}
