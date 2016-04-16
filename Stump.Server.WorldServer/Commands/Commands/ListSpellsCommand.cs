using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.WorldServer.Commands.Commands.Patterns;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Spells;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class ListSpellsCommand : TargetSubCommand
	{
		public ListSpellsCommand()
		{
			base.Aliases = new string[]
			{
				"list"
			};
			base.RequiredRole = RoleEnum.GameMaster;
			base.ParentCommand = typeof(SpellsCommands);
			base.Description = "List the spells of the target";
			base.AddTargetParameter(true, "Defined target");
		}
		public override void Execute(TriggerBase trigger)
		{
			Character target = base.GetTarget(trigger);
			foreach (CharacterSpell current in target.Spells)
			{
				trigger.Reply("{0} ({1}) - Level {2}", new object[]
				{
					trigger.Bold(current.Template.Name),
					trigger.Bold(current.Id),
					trigger.Bold(current.CurrentLevel)
				});
			}
		}
	}
}
