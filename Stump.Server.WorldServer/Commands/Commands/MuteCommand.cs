using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.WorldServer.Commands.Commands.Patterns;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class MuteCommand : TargetCommand
	{
		public MuteCommand()
		{
			base.Aliases = new string[]
			{
				"mute"
			};
			base.RequiredRole = RoleEnum.Moderator;
			base.AddTargetParameter(false, "Defined target");
			base.AddParameter<int>("time", "time", "Mute for x minutes", 5, false, null);
		}
		public override void Execute(TriggerBase trigger)
		{
			Character[] targets = base.GetTargets(trigger);
			for (int i = 0; i < targets.Length; i++)
			{
				Character character = targets[i];
				int num = (trigger.Get<int>("time") > 720) ? 720 : trigger.Get<int>("time");
				character.Mute(System.TimeSpan.FromMinutes((double)num), trigger.User as Character);
				trigger.Reply("{0} muted", new object[]
				{
					character.Name
				});
				character.OpenPopup(string.Format("Vous avez été muté pendant {0} minutes", num));
			}
		}
	}
}
