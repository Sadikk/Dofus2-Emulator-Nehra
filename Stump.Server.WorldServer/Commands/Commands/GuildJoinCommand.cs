using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Commands.Commands.Patterns;
using Stump.Server.WorldServer.Commands.Trigger;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Guilds;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class GuildJoinCommand : InGameSubCommand
	{
		public GuildJoinCommand()
		{
			base.Aliases = new string[]
			{
				"join"
			};
			base.RequiredRole = RoleEnum.GameMaster;
			base.ParentCommand = typeof(GuildCommand);
			base.AddParameter<string>("guildname", "guild", "The name of the guild", null, false, null);
		}
		public override void Execute(GameTrigger trigger)
		{
			Character character = trigger.Character;
			if (character.GuildMember == null)
			{
				string name = trigger.Get<string>("guildname");
				Guild guild = Singleton<GuildManager>.Instance.TryGetGuild(name);
				GuildMember guildMember;
				guild.TryAddMember(character, out guildMember);
				character.GuildMember = guildMember;
				trigger.Reply(string.Format("You have join Guild: {0}", guild.Name));
			}
			else
			{
				trigger.ReplyError("You must leave your Guild before join another");
			}
		}
	}
}
