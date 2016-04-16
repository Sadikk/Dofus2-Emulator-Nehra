namespace Stump.Server.WorldServer.Database.Interactives
{
	public class InteractiveSpawnRelator
	{
		public static string FetchQuery = "SELECT * FROM interactives_spawns LEFT JOIN interactives_spawns_skills ON interactives_spawns_skills.InteractiveSpawnId=interactives_spawns.Id LEFT JOIN interactives_skills ON interactives_skills.Id=interactives_spawns_skills.SkillId";
		private InteractiveSpawn m_current;
		public InteractiveSpawn Map(InteractiveSpawn spawn, InteractiveSpawnSkills binding, InteractiveSkillRecord skill)
		{
			InteractiveSpawn result;
			if (spawn == null)
			{
				result = this.m_current;
			}
			else
			{
				if (this.m_current != null && this.m_current.Id == spawn.Id)
				{
					if (binding.InteractiveSpawnId == this.m_current.Id && binding.SkillId == skill.Id)
					{
						this.m_current.Skills.Add(skill);
					}
					result = null;
				}
				else
				{
					InteractiveSpawn current = this.m_current;
					this.m_current = spawn;
					if (binding.InteractiveSpawnId == this.m_current.Id && binding.SkillId == skill.Id)
					{
						this.m_current.Skills.Add(skill);
					}
					result = current;
				}
			}
			return result;
		}
	}
}
