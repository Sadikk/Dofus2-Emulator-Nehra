using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.WorldServer.Commands.Commands.Patterns;
using Stump.Server.WorldServer.Database.Monsters;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Spells;
using System.Linq;
namespace Stump.Server.WorldServer.Commands.Commands
{
	public class LearnMonsterSpellCommand : TargetSubCommand
	{
		public LearnMonsterSpellCommand()
		{
			base.Aliases = new string[]
			{
				"learnmonster"
			};
			base.RequiredRole = RoleEnum.GameMaster;
			base.ParentCommand = typeof(SpellsCommands);
			base.Description = "Learn the given spell";
			string arg_57_1 = "monster";
			string arg_57_2 = "monster";
			string arg_57_3 = "Target monster to learn spells";
			ConverterHandler<MonsterTemplate> monsterTemplateConverter = ParametersConverter.MonsterTemplateConverter;
			base.AddParameter<MonsterTemplate>(arg_57_1, arg_57_2, arg_57_3, null, true, monsterTemplateConverter);
			base.AddTargetParameter(true, "Defined target");
		}
		public override void Execute(TriggerBase trigger)
		{
			MonsterTemplate monsterTemplate = trigger.Get<MonsterTemplate>("monster");
			Character target = base.GetTarget(trigger);
			foreach (MonsterSpell current in monsterTemplate.Grades.FirstOrDefault<MonsterGrade>().SpellsTemplates)
			{
				CharacterSpell characterSpell = target.Spells.LearnSpell(current.SpellId);
				if (characterSpell != null)
				{
					trigger.Reply("'{0}' learned the spell '{1}'", new object[]
					{
						trigger.Bold(target),
						trigger.Bold(characterSpell.Template.Name)
					});
				}
				else
				{
					trigger.ReplyError("Spell {0} not learned. Unknow reason", new object[]
					{
						trigger.Bold(current.SpellId)
					});
				}
			}
		}
	}
}
