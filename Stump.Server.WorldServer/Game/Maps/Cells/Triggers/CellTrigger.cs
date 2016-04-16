using Stump.Server.WorldServer.Database.World.Triggers;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Maps.Cells.Triggers
{
	public abstract class CellTrigger
	{
		public CellTriggerRecord Record
		{
			get;
			private set;
		}
		public ObjectPosition Position
		{
			get;
			private set;
		}
		public CellTriggerType TriggerType
		{
			get
			{
				return this.Record.TriggerType;
			}
		}
		protected CellTrigger(CellTriggerRecord record)
		{
			this.Record = record;
			this.Position = record.GetPosition();
		}
		public abstract void Apply(Character character);
	}
}
