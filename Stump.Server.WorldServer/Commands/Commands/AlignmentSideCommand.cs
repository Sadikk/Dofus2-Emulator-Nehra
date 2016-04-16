using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.WorldServer.Commands.Commands.Patterns;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class AlignmentSideCommand : TargetSubCommand
	{
		public AlignmentSideCommand()
		{
			base.Aliases = new string[]
			{
				"side"
			};
			base.RequiredRole = RoleEnum.Moderator;
			base.ParentCommand = typeof(AlignmentCommands);
			base.Description = "Set the alignement side of the given target";
			base.AddParameter<AlignmentSideEnum>("side", "s", "Alignement side", AlignmentSideEnum.ALIGNMENT_NEUTRAL, false, ParametersConverter.GetEnumConverter<AlignmentSideEnum>());
			base.AddTargetParameter(true, "Defined target");
		}
		public override void Execute(TriggerBase trigger)
		{
			Character[] targets = base.GetTargets(trigger);
			for (int i = 0; i < targets.Length; i++)
			{
				Character character = targets[i];
				character.ChangeAlignementSide(trigger.Get<AlignmentSideEnum>("side"));
			}
		}
	}
}
