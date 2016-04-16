using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.WorldServer.Commands.Commands.Patterns;
using Stump.Server.WorldServer.Commands.Trigger;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class NameGoCommand : TargetCommand
	{
		public NameGoCommand()
		{
			base.Aliases = new string[]
			{
				"namego"
			};
			base.RequiredRole = RoleEnum.GameMaster_Padawan;
			base.Description = "Teleport target to you";
			base.AddTargetParameter(false, "The character to teleport");
		}
		public override void Execute(TriggerBase trigger)
		{
			Character[] targets = base.GetTargets(trigger);
			for (int i = 0; i < targets.Length; i++)
			{
				Character character = targets[i];
				Character to = ((GameTrigger)trigger).Character;
				Character target1 = character;
				character.Area.ExecuteInContext(delegate
				{
					target1.Teleport(to.Position, true);
				});
			}
		}
	}
}
