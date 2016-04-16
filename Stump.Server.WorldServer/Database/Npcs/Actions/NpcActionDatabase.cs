using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Npcs;
using Stump.Server.WorldServer.Game.Conditions;

namespace Stump.Server.WorldServer.Database.Npcs.Actions
{
	public abstract class NpcActionDatabase : NpcAction
	{
		public NpcActionRecord Record
		{
			get;
			private set;
		}
		public uint Id
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
		public int NpcId
		{
			get
			{
				return this.Record.NpcId;
			}
			set
			{
				this.Record.NpcId = value;
			}
		}
		public NpcTemplate Template
		{
			get
			{
				return this.Record.Template;
			}
			set
			{
				this.Record.Template = value;
			}
		}
		public ConditionExpression ConditionExpression
		{
			get
			{
				return this.Record.ConditionExpression;
			}
			set
			{
				this.Record.ConditionExpression = value;
			}
		}
		public NpcActionDatabase(NpcActionRecord record)
		{
			this.Record = record;
		}
		public override bool CanExecute(Npc npc, Character character)
		{
			return this.ConditionExpression == null || this.ConditionExpression.Eval(character);
		}
	}
}
