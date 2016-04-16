using Stump.Server.BaseServer.Database;
using Stump.Server.BaseServer.Initialization;
using Stump.Server.WorldServer.Database.Npcs;
using Stump.Server.WorldServer.Database.Npcs.Actions;
using Stump.Server.WorldServer.Database.Npcs.Replies;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Actors.RolePlay.Npcs
{
	public class NpcManager : DataManager<NpcManager>
	{
		private System.Collections.Generic.Dictionary<uint, NpcSpawn> m_npcsSpawns;
		private System.Collections.Generic.Dictionary<int, NpcTemplate> m_npcsTemplates;
		private System.Collections.Generic.Dictionary<uint, NpcActionRecord> m_npcsActions;
		private System.Collections.Generic.Dictionary<int, NpcReplyRecord> m_npcsReplies;
		private System.Collections.Generic.Dictionary<int, NpcMessage> m_npcsMessages;
		[Initialization(InitializationPass.Fifth)]
		public override void Initialize()
		{
			this.m_npcsSpawns = base.Database.Fetch<NpcSpawn>(NpcSpawnRelator.FetchQuery, new object[0]).ToDictionary((NpcSpawn entry) => entry.Id);
			this.m_npcsTemplates = base.Database.Fetch<NpcTemplate>(NpcTemplateRelator.FetchQuery, new object[0]).ToDictionary((NpcTemplate entry) => entry.Id);
			this.m_npcsActions = base.Database.Fetch<NpcActionRecord>(NpcActionRecordRelator.FetchQuery, new object[0]).ToDictionary((NpcActionRecord entry) => entry.Id);
			this.m_npcsReplies = base.Database.Fetch<NpcReplyRecord>(NpcReplyRecordRelator.FetchQuery, new object[0]).ToDictionary((NpcReplyRecord entry) => entry.Id);
			this.m_npcsMessages = base.Database.Fetch<NpcMessage>(NpcMessageRelator.FetchQuery, new object[0]).ToDictionary((NpcMessage entry) => entry.Id);
		}
		public NpcSpawn GetNpcSpawn(uint id)
		{
			NpcSpawn npcSpawn;
			NpcSpawn result;
			if (this.m_npcsSpawns.TryGetValue(id, out npcSpawn))
			{
				result = npcSpawn;
			}
			else
			{
				result = npcSpawn;
			}
			return result;
		}
		public NpcSpawn GetOneNpcSpawn(System.Predicate<NpcSpawn> predicate)
		{
			return this.m_npcsSpawns.Values.SingleOrDefault((NpcSpawn entry) => predicate(entry));
		}
		public System.Collections.Generic.IEnumerable<NpcSpawn> GetNpcSpawns()
		{
			return this.m_npcsSpawns.Values;
		}
		public System.Collections.Generic.IEnumerable<NpcTemplate> GetNpcTemplates()
		{
			return this.m_npcsTemplates.Values;
		}
		public NpcTemplate GetNpcTemplate(int id)
		{
			NpcTemplate npcTemplate;
			return this.m_npcsTemplates.TryGetValue(id, out npcTemplate) ? npcTemplate : npcTemplate;
		}
		public NpcTemplate GetNpcTemplate(string name, bool ignorecase)
		{
			return this.m_npcsTemplates.Values.FirstOrDefault((NpcTemplate entry) => entry.Name.Equals(name, ignorecase ? System.StringComparison.InvariantCultureIgnoreCase : System.StringComparison.InvariantCulture));
		}
		public NpcMessage GetNpcMessage(int id)
		{
			NpcMessage npcMessage;
			return this.m_npcsMessages.TryGetValue(id, out npcMessage) ? npcMessage : npcMessage;
		}
		public System.Collections.Generic.List<NpcActionRecord> GetNpcActionsRecords(int id)
		{
			return (
				from entry in this.m_npcsActions
				where entry.Value.NpcId == id
				select entry.Value).ToList<NpcActionRecord>();
		}
		public System.Collections.Generic.List<NpcReplyRecord> GetMessageRepliesRecords(int id)
		{
			return (
				from entry in this.m_npcsReplies
				where entry.Value.MessageId == id
				select entry.Value).ToList<NpcReplyRecord>();
		}
		public System.Collections.Generic.List<NpcActionDatabase> GetNpcActions(int id)
		{
			return (
				from entry in this.m_npcsActions
				where entry.Value.NpcId == id
				select entry.Value.GenerateAction()).ToList<NpcActionDatabase>();
		}
		public System.Collections.Generic.List<NpcReply> GetMessageReplies(int id)
		{
			return (
				from entry in this.m_npcsReplies
				where entry.Value.MessageId == id
				select entry.Value.GenerateReply()).ToList<NpcReply>();
		}
		public void AddNpcSpawn(NpcSpawn spawn)
		{
			base.Database.Insert(spawn);
			this.m_npcsSpawns.Add(spawn.Id, spawn);
		}
		public void RemoveNpcSpawn(NpcSpawn spawn)
		{
			base.Database.Delete(spawn);
			this.m_npcsSpawns.Remove(spawn.Id);
		}
		public void AddNpcAction(NpcActionDatabase action)
		{
			base.Database.Insert(action.Record);
			this.m_npcsActions.Add(action.Record.Id, action.Record);
		}
		public void RemoveNpcAction(NpcActionDatabase action)
		{
			base.Database.Delete(action);
			this.m_npcsActions.Remove(action.Record.Id);
		}
	}
}
