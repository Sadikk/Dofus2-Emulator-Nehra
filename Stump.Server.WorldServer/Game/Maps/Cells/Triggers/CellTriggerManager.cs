using Stump.Server.BaseServer.Database;
using Stump.Server.BaseServer.Initialization;
using Stump.Server.WorldServer.Database.World.Triggers;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Maps.Cells.Triggers
{
	public class CellTriggerManager : DataManager<CellTriggerManager>
	{
		private System.Collections.Generic.Dictionary<int, CellTriggerRecord> m_cellTriggers;
		[Initialization(InitializationPass.Fourth)]
		public override void Initialize()
		{
			this.m_cellTriggers = base.Database.Query<CellTriggerRecord>(CellTriggerRecordRelator.FetchQuery, new object[0]).ToDictionary((CellTriggerRecord entry) => entry.Id);
		}
		public System.Collections.Generic.IEnumerable<CellTriggerRecord> GetCellTriggers()
		{
			return this.m_cellTriggers.Values;
		}
		public CellTriggerRecord GetOneCellTrigger(System.Predicate<CellTriggerRecord> predicate)
		{
			return this.m_cellTriggers.Values.FirstOrDefault((CellTriggerRecord entry) => predicate(entry));
		}
		public CellTriggerRecord GetCellTrigger(int id)
		{
			CellTriggerRecord cellTriggerRecord;
			CellTriggerRecord result;
			if (this.m_cellTriggers.TryGetValue(id, out cellTriggerRecord))
			{
				result = cellTriggerRecord;
			}
			else
			{
				result = cellTriggerRecord;
			}
			return result;
		}
		public void AddCellTrigger(CellTriggerRecord cellTrigger)
		{
			base.Database.Insert(cellTrigger);
			this.m_cellTriggers.Add(cellTrigger.Id, cellTrigger);
		}
	}
}
