using Stump.Core.Reflection;
using Stump.Server.BaseServer.Database;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Npcs;
using Stump.Server.WorldServer.Game.Dialogs.Npcs;

namespace Stump.Server.WorldServer.Database.Npcs.Replies
{
	[Discriminator("Dialog", typeof(NpcReply), new System.Type[]
	{
		typeof(NpcReplyRecord)
	})]
	public class ContinueDialogReply : NpcReply
	{
		private NpcMessage m_message;
		public int NextMessageId
		{
			get
			{
				return base.Record.GetParameter<int>(0u, false);
			}
			set
			{
				base.Record.SetParameter<int>(0u, value);
			}
		}
		public NpcMessage NextMessage
		{
			get
			{
				NpcMessage arg_23_0;
				if ((arg_23_0 = this.m_message) == null)
				{
					arg_23_0 = (this.m_message = Singleton<NpcManager>.Instance.GetNpcMessage(this.NextMessageId));
				}
				return arg_23_0;
			}
			set
			{
				this.m_message = value;
				this.NextMessageId = value.Id;
			}
		}
		public ContinueDialogReply(NpcReplyRecord record) : base(record)
		{
		}
		public override bool CanExecute(Npc npc, Character character)
		{
			return base.CanExecute(npc, character) && character.IsTalkingWithNpc();
		}
		public override bool Execute(Npc npc, Character character)
		{
			bool result;
			if (!base.Execute(npc, character))
			{
				result = false;
			}
			else
			{
				((NpcDialog)character.Dialog).ChangeMessage(this.NextMessage);
				result = true;
			}
			return result;
		}
	}
}
