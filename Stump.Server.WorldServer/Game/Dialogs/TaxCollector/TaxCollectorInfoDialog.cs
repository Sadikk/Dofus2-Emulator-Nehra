using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Messages;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Actors.RolePlay.TaxCollectors;
using Stump.Server.WorldServer.Handlers.Dialogs;

namespace Stump.Server.WorldServer.Game.Dialogs.TaxCollector
{
	public class TaxCollectorInfoDialog : IDialog
	{
		public TaxCollectorNpc TaxCollector
		{
			get;
			private set;
		}
		public Character Character
		{
			get;
			private set;
		}
		public DialogTypeEnum DialogType
		{
			get
			{
				return DialogTypeEnum.DIALOG_DIALOG;
			}
		}
		public TaxCollectorInfoDialog(Character character, TaxCollectorNpc taxCollector)
		{
			this.TaxCollector = taxCollector;
			this.Character = character;
		}
		public void Close()
		{
			this.Character.CloseDialog(this);
			this.TaxCollector.OnDialogClosed(this);
			DialogHandler.SendLeaveDialogMessage(this.Character.Client, this.DialogType);
		}
		public void Open()
		{
			this.Character.SetDialog(this);
			this.TaxCollector.OnDialogOpened(this);
			this.Character.Client.Send(new NpcDialogCreationMessage(this.TaxCollector.Map.Id, this.TaxCollector.Id));
            this.Character.Client.Send(new TaxCollectorDialogQuestionExtendedMessage(this.TaxCollector.Guild.GetBasicGuildInformations(), (ushort)this.TaxCollector.Guild.TaxCollectorPods, (ushort)this.TaxCollector.Guild.TaxCollectorProspecting, (ushort)this.TaxCollector.Guild.TaxCollectorWisdom, (sbyte)this.TaxCollector.Guild.TaxCollectors.Count, this.TaxCollector.AttacksCount, (uint)this.TaxCollector.GatheredKamas, (ulong)this.TaxCollector.GatheredExperience, (uint)this.TaxCollector.Bag.BagWeight, (uint)this.TaxCollector.Bag.BagValue));
		}
	}
}
