using Stump.Core.Pool;
using Stump.Server.BaseServer.Database;
using Stump.Server.BaseServer.Initialization;
using Stump.Server.WorldServer.Database.Interactives;
using System;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Interactives
{
	public class InteractiveManager : DataManager<InteractiveManager>
	{
		private readonly UniqueIdProvider m_idProvider = new UniqueIdProvider();
		private System.Collections.Generic.Dictionary<int, InteractiveSpawn> m_interactivesSpawns;
		private System.Collections.Generic.Dictionary<int, InteractiveTemplate> m_interactivesTemplates;
		private System.Collections.Generic.Dictionary<int, InteractiveSkillTemplate> m_skillsTemplates;
		[Initialization(InitializationPass.Fourth)]
		public override void Initialize()
		{
			this.m_interactivesTemplates = base.Database.Query<InteractiveTemplate, InteractiveTemplateSkills, InteractiveSkillRecord, InteractiveTemplate>(new Func<InteractiveTemplate, InteractiveTemplateSkills, InteractiveSkillRecord, InteractiveTemplate>(new InteractiveTemplateRelator().Map), InteractiveTemplateRelator.FetchQuery, new object[0]).ToDictionary((InteractiveTemplate entry) => entry.Id);
			this.m_interactivesSpawns = base.Database.Query<InteractiveSpawn, InteractiveSpawnSkills, InteractiveSkillRecord, InteractiveSpawn>(new Func<InteractiveSpawn, InteractiveSpawnSkills, InteractiveSkillRecord, InteractiveSpawn>(new InteractiveSpawnRelator().Map), InteractiveSpawnRelator.FetchQuery, new object[0]).ToDictionary((InteractiveSpawn entry) => entry.Id);
			this.m_skillsTemplates = base.Database.Query<InteractiveSkillTemplate>(InteractiveSkillTemplateRelator.FetchQuery, new object[0]).ToDictionary((InteractiveSkillTemplate entry) => entry.Id);
		}
		public int PopSkillId()
		{
			return this.m_idProvider.Pop();
		}
		public void FreeSkillId(int id)
		{
			this.m_idProvider.Push(id);
		}
		public System.Collections.Generic.IEnumerable<InteractiveSpawn> GetInteractiveSpawns()
		{
			return this.m_interactivesSpawns.Values;
		}
		public InteractiveSpawn GetOneSpawn(System.Predicate<InteractiveSpawn> predicate)
		{
			return this.m_interactivesSpawns.Values.SingleOrDefault((InteractiveSpawn entry) => predicate(entry));
		}
		public InteractiveTemplate GetTemplate(int id)
		{
			InteractiveTemplate interactiveTemplate;
			InteractiveTemplate result;
			if (this.m_interactivesTemplates.TryGetValue(id, out interactiveTemplate))
			{
				result = interactiveTemplate;
			}
			else
			{
				result = interactiveTemplate;
			}
			return result;
		}
		public InteractiveSkillTemplate GetSkillTemplate(int id)
		{
			InteractiveSkillTemplate interactiveSkillTemplate;
			InteractiveSkillTemplate result;
			if (this.m_skillsTemplates.TryGetValue(id, out interactiveSkillTemplate))
			{
				result = interactiveSkillTemplate;
			}
			else
			{
				result = interactiveSkillTemplate;
			}
			return result;
		}
		public void AddInteractiveSpawn(InteractiveSpawn spawn)
		{
			base.Database.Insert(spawn);
			this.m_interactivesSpawns.Add(spawn.Id, spawn);
		}
	}
}
