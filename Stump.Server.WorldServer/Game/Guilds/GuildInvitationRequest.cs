using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Dialogs;
using Stump.Server.WorldServer.Handlers.Guilds;

namespace Stump.Server.WorldServer.Game.Guilds
{
	public class GuildInvitationRequest : RequestBox
	{
		public GuildInvitationRequest(Character source, Character target) : base(source, target)
		{
		}
		protected override void OnOpen()
		{
			GuildHandler.SendGuildInvitationStateRecruterMessage(base.Source.Client, base.Target, GuildInvitationStateEnum.GUILD_INVITATION_SENT);
			GuildHandler.SendGuildInvitationStateRecrutedMessage(base.Target.Client, GuildInvitationStateEnum.GUILD_INVITATION_SENT);
			GuildHandler.SendGuildInvitedMessage(base.Target.Client, base.Source);
		}
		protected override void OnAccept()
		{
			GuildHandler.SendGuildInvitationStateRecruterMessage(base.Source.Client, base.Target, GuildInvitationStateEnum.GUILD_INVITATION_OK);
			GuildHandler.SendGuildInvitationStateRecrutedMessage(base.Target.Client, GuildInvitationStateEnum.GUILD_INVITATION_OK);
			Guild guild = base.Source.Guild;
			if (guild != null)
			{
				guild.TryAddMember(base.Target);
			}
		}
		protected override void OnDeny()
		{
			GuildHandler.SendGuildInvitationStateRecruterMessage(base.Source.Client, base.Target, GuildInvitationStateEnum.GUILD_INVITATION_CANCELED);
			GuildHandler.SendGuildInvitationStateRecruterMessage(base.Target.Client, base.Target, GuildInvitationStateEnum.GUILD_INVITATION_CANCELED);
		}
		protected override void OnCancel()
		{
			GuildHandler.SendGuildInvitationStateRecruterMessage(base.Source.Client, base.Target, GuildInvitationStateEnum.GUILD_INVITATION_CANCELED);
			GuildHandler.SendGuildInvitationStateRecruterMessage(base.Target.Client, base.Target, GuildInvitationStateEnum.GUILD_INVITATION_CANCELED);
		}
	}
}
