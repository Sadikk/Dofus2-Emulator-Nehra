using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.Npcs;
using Stump.Server.WorldServer.Database.Npcs.Replies;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Npcs;
using Stump.Server.WorldServer.Handlers.Context.RolePlay;
using Stump.Server.WorldServer.Handlers.Dialogs;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Dialogs.Npcs
{
	public class NpcDialog : IDialog
	{
		public DialogTypeEnum DialogType
		{
			get
			{
				return DialogTypeEnum.DIALOG_DIALOG;
			}
		}
		public Character Character
		{
			get;
			private set;
		}
		public Npc Npc
		{
			get;
			private set;
		}
		public NpcMessage CurrentMessage
		{
			get;
			protected set;
		}
		public NpcDialog(Character character, Npc npc)
		{
			this.Character = character;
			this.Npc = npc;
		}
		public virtual void Open()
		{
			this.Character.SetDialog(this);
			ContextRoleplayHandler.SendNpcDialogCreationMessage(this.Character.Client, this.Npc);
		}
		public virtual void Close()
		{
			DialogHandler.SendLeaveDialogMessage(this.Character.Client, this.DialogType);
			this.Character.CloseDialog(this);
		}
		public virtual void Reply(short replyId)
		{
			NpcMessage currentMessage = this.CurrentMessage;
			NpcReply[] array = (
				from entry in this.CurrentMessage.Replies
				where entry.ReplyId == (int)replyId
				select entry).ToArray<NpcReply>();
			if (array.Any((NpcReply x) => !x.CanExecute(this.Npc, this.Character)))
			{
				this.Character.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 34, new object[0]);
			}
			else
			{
				NpcReply[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					NpcReply reply = array2[i];
					this.Reply(reply);
				}
				if (array.Length == 0 || currentMessage == this.CurrentMessage)
				{
					this.Close();
				}
			}
		}
		public void Reply(NpcReply reply)
		{
			reply.Execute(this.Npc, this.Character);
		}
		public void ChangeMessage(short id)
		{
			NpcMessage npcMessage = Singleton<NpcManager>.Instance.GetNpcMessage((int)id);
			if (npcMessage != null)
			{
				this.ChangeMessage(npcMessage);
			}
		}
		public virtual void ChangeMessage(NpcMessage message)
		{
			this.CurrentMessage = message;
			System.Collections.Generic.IEnumerable<short> replies = (
				from entry in message.Replies
				where entry.CriteriaExpression == null || entry.CriteriaExpression.Eval(this.Character)
				select (short)entry.ReplyId).Distinct<short>();
			ContextRoleplayHandler.SendNpcDialogQuestionMessage(this.Character.Client, this.CurrentMessage, replies, new string[0]);
		}
	}
}
