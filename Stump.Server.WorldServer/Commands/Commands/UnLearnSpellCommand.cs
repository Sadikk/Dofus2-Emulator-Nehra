using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.WorldServer.Commands.Commands.Patterns;
using Stump.Server.WorldServer.Database.Spells;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class UnLearnSpellCommand : TargetSubCommand
	{
		public UnLearnSpellCommand()
		{
			base.Aliases = new string[]
			{
				"unlearn",
				"forget",
				"remove"
			};
			base.RequiredRole = RoleEnum.GameMaster;
			base.ParentCommand = typeof(SpellsCommands);
			base.Description = "Forget the given spell";
			base.AddParameter<SpellTemplate>("spell", "spell", "Given spell to forget", null, false, ParametersConverter.SpellTemplateConverter);
			base.AddTargetParameter(true, "Defined target");
			base.AddParameter<bool>("keep", "keep", "If true, keep the spell but reset it to level 1", false, true, null);
		}
		public override void Execute(TriggerBase trigger)
		{
			SpellTemplate spellTemplate = trigger.Get<SpellTemplate>("spell");
			Character target = base.GetTarget(trigger);
			if (trigger.Get<bool>("keep") ? target.Spells.ForgetSpell(spellTemplate) : target.Spells.UnLearnSpell(spellTemplate))
			{
				trigger.Reply("'{0}' forgot the spell '{1}'{2}", new object[]
				{
					trigger.Bold(target),
					trigger.Bold(spellTemplate.Name),
					trigger.Get<bool>("keep") ? " (but kept it)" : string.Empty
				});
			}
			else
			{
				trigger.ReplyError("Spell {0} not unlearned. {1} may not have this spell", new object[]
				{
					trigger.Bold(spellTemplate.Name),
					trigger.Bold(target)
				});
			}
		}
	}
}
