using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Npcs;
using Stump.Server.WorldServer.Game.Conditions;

namespace Stump.Server.WorldServer.Database.Npcs.Replies
{
	public abstract class NpcReply
	{
		public int Id
		{
			get
			{
				return this.Record.Id;
			}
			set
			{
				this.Record.Id = value;
			}
		}
		public int ReplyId
		{
			get
			{
				return this.Record.ReplyId;
			}
			set
			{
				this.Record.ReplyId = value;
			}
		}
		public int MessageId
		{
			get
			{
				return this.Record.MessageId;
			}
			set
			{
				this.Record.MessageId = value;
			}
		}
		public ConditionExpression CriteriaExpression
		{
			get
			{
				return this.Record.CriteriaExpression;
			}
			set
			{
				this.Record.CriteriaExpression = value;
			}
		}
		public NpcMessage Message
		{
			get
			{
				return this.Record.Message;
			}
			set
			{
				this.Record.Message = value;
			}
		}
		public NpcReplyRecord Record
		{
			get;
			private set;
		}
		public NpcReply()
		{
			this.Record = new NpcReplyRecord();
		}
		public NpcReply(NpcReplyRecord record)
		{
			this.Record = record;
		}
		public virtual bool CanExecute(Npc npc, Character character)
		{
			return this.Record.CriteriaExpression == null || this.Record.CriteriaExpression.Eval(character);
		}
		public virtual bool Execute(Npc npc, Character character)
		{
			bool result;
			if (!this.CanExecute(npc, character))
			{
				character.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 34, new object[0]);
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}
	}
}
