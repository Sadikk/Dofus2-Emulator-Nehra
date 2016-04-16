using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Guilds;
using Stump.Server.WorldServer.Handlers.Dialogs;
using Stump.Server.WorldServer.Handlers.Guilds;

namespace Stump.Server.WorldServer.Game.Dialogs.Guilds
{
	public class GuildCreationPanel : IDialog
	{
		public Character Character
		{
			get;
			private set;
		}
		public DialogTypeEnum DialogType
		{
			get
			{
				return DialogTypeEnum.DIALOG_GUILD_CREATE;
			}
		}
		public GuildCreationPanel(Character character)
		{
			this.Character = character;
		}
		public void Open()
		{
			if (this.Character.Guild != null)
			{
				GuildHandler.SendGuildCreationResultMessage(this.Character.Client, GuildCreationResultEnum.GUILD_CREATE_ERROR_ALREADY_IN_GUILD);
			}
			else
			{
				this.Character.SetDialog(this);
				GuildHandler.SendGuildCreationStartedMessage(this.Character.Client);
			}
		}

		public void CreateGuild(string guildName, Stump.DofusProtocol.Types.GuildEmblem emblem)
		{
			if (this.Character.Guild != null)
			{
				GuildHandler.SendGuildCreationResultMessage(this.Character.Client, GuildCreationResultEnum.GUILD_CREATE_ERROR_ALREADY_IN_GUILD);
			}
			else
			{
				GuildCreationResultEnum result = Singleton<GuildManager>.Instance.CreateGuild(this.Character, guildName, emblem);
				
                GuildHandler.SendGuildCreationResultMessage(this.Character.Client, result);

                if (result == GuildCreationResultEnum.GUILD_CREATE_OK)
                {
                    this.Close();
                }
			}
		}

        public void Close()
        {
            this.Character.CloseDialog(this);
            DialogHandler.SendLeaveDialogMessage(this.Character.Client, this.DialogType);
        }
	}
}
