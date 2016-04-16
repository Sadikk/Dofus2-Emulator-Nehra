using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.WorldServer.Commands.Commands.Patterns;
using Stump.Server.WorldServer.Database.Spells;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Spells;
using Stump.Server.WorldServer.Handlers.Inventory;
using System.Linq;
namespace Stump.Server.WorldServer.Commands.Commands
{
	public class SetSpellLevelCommand : TargetSubCommand
	{
		public SetSpellLevelCommand()
		{
			base.Aliases = new string[]
			{
				"level"
			};
			base.RequiredRole = RoleEnum.GameMaster;
			base.ParentCommand = typeof(SpellsCommands);
			base.Description = "Set the level of the given spell of the target";
			base.AddParameter<SpellTemplate>("spell", "spell", "Given spell to forget", null, false, ParametersConverter.SpellTemplateConverter);
			base.AddParameter<int>("level", "l", "", 0, false, null);
			base.AddTargetParameter(true, "Defined target");
		}
		public override void Execute(TriggerBase trigger)
		{
			Character target = base.GetTarget(trigger);
			SpellTemplate spellTemplate = trigger.Get<SpellTemplate>("spell");
			int num = trigger.Get<int>("level");
			CharacterSpell spell = target.Spells.GetSpell(spellTemplate.Id);
			if (spell == null)
			{
				trigger.ReplyError("Spell {0} not found", new object[]
				{
					trigger.Bold(spell)
				});
			}
			else
			{
				if (!spell.ByLevel.ContainsKey(num))
				{
					trigger.ReplyError("Level {0} not found. Give a level between {1} and {2}", new object[]
					{
						trigger.Bold(num),
						trigger.Bold(spell.ByLevel.Keys.Min()),
						trigger.Bold(spell.ByLevel.Keys.Max())
					});
				}
				else
				{
					spell.CurrentLevel = (byte)num;
					trigger.ReplyBold("{0}'s spell {1} is now level {2}", new object[]
					{
						target,
						spell.Template.Name,
						num
					});
					InventoryHandler.SendSpellUpgradeSuccessMessage(target.Client, spell);
				}
			}
		}
	}
}
