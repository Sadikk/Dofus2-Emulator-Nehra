using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Messages;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Prisms;
using Stump.Server.WorldServer.Handlers.Dialogs;

namespace Stump.Server.WorldServer.Game.Dialogs.Prisms
{
    public class PrismInfoDialog : IDialog
    {
        public PrismNpc Prism { get; private set; }
        public Character Character { get; private set; }
        public DialogTypeEnum DialogType { get { return DialogTypeEnum.DIALOG_DIALOG; } }

        public PrismInfoDialog(Character character, PrismNpc prism)
        {
            this.Prism = prism;
            this.Character = character;
        }

        public void Open()
        {
            this.Character.SetDialog(this);
            this.Prism.OnDialogOpened(this);

            this.Character.Client.Send(new NpcDialogCreationMessage(this.Prism.Map.Id, this.Prism.Id));
            this.Character.Client.Send(new AlliancePrismDialogQuestionMessage());
        }

        public void Close()
        {
            this.Character.CloseDialog(this);
            this.Prism.OnDialogClosed(this);

            DialogHandler.SendLeaveDialogMessage(this.Character.Client, this.DialogType);
        }
    }
}
