using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Commands.Commands.Patterns;
using Stump.Server.WorldServer.Commands.Trigger;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class GuildBossCommand : InGameSubCommand
	{
		public GuildBossCommand()
		{
			base.Aliases = new string[]
			{
				"boss"
			};
			base.RequiredRole = RoleEnum.GameMaster;
			base.ParentCommand = typeof(GuildCommand);
		}
		public override void Execute(GameTrigger trigger)
		{
			Character character = trigger.Character;
			if (character.GuildMember != null)
			{
				character.Guild.SetBoss(character.GuildMember);
			}
			else
			{
				trigger.ReplyError("You must be in a guild to do that !");
			}
		}
	}
}
