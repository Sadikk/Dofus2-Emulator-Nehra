using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.WorldServer.Commands.Commands.Patterns;
using Stump.Server.WorldServer.Database.Spells;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Spells;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class LearnSpellCommand : TargetSubCommand
	{
		public LearnSpellCommand()
		{
			base.Aliases = new string[]
			{
				"learn",
				"add"
			};
			base.RequiredRole = RoleEnum.GameMaster;
			base.ParentCommand = typeof(SpellsCommands);
			base.Description = "Learn the given spell";
			base.AddParameter<SpellTemplate>("spell", "spell", "Given spell to learn", null, false, ParametersConverter.SpellTemplateConverter);
			base.AddTargetParameter(true, "Defined target");
		}
		public override void Execute(TriggerBase trigger)
		{
			SpellTemplate spellTemplate = trigger.Get<SpellTemplate>("spell");
			Character target = base.GetTarget(trigger);
			CharacterSpell characterSpell = target.Spells.LearnSpell(spellTemplate);
			if (characterSpell != null)
			{
				trigger.Reply("'{0}' learned the spell '{1}'", new object[]
				{
					trigger.Bold(target),
					trigger.Bold(spellTemplate.Name)
				});
			}
			else
			{
				trigger.ReplyError("Spell {0} not learned. Unknow reason", new object[]
				{
					trigger.Bold(spellTemplate.Name)
				});
			}
		}
	}
}
