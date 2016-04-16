using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Database;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Npcs;
using Stump.Server.WorldServer.Game.Dialogs.Npcs;

namespace Stump.Server.WorldServer.Database.Npcs.Actions
{
	[Discriminator("Talk", typeof(NpcActionDatabase), new System.Type[]
	{
		typeof(NpcActionRecord)
	})]
	public class NpcTalkAction : NpcActionDatabase
	{
		private NpcMessage m_message;
		public override NpcActionTypeEnum ActionType
		{
			get
			{
				return NpcActionTypeEnum.ACTION_TALK;
			}
		}
		public int MessageId
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
		public NpcMessage Message
		{
			get
			{
				NpcMessage arg_23_0;
				if ((arg_23_0 = this.m_message) == null)
				{
					arg_23_0 = (this.m_message = Singleton<NpcManager>.Instance.GetNpcMessage(this.MessageId));
				}
				return arg_23_0;
			}
			set
			{
				this.m_message = value;
				this.MessageId = value.Id;
			}
		}
		public NpcTalkAction(NpcActionRecord record) : base(record)
		{
		}
		public override void Execute(Npc npc, Character character)
		{
			NpcDialog npcDialog = new NpcDialog(character, npc);
			npcDialog.Open();
			npcDialog.ChangeMessage(this.Message);
		}
	}
}
