namespace Stump.Server.WorldServer.Database.Interactives
{
	public class InteractiveTemplateRelator
	{
		public static string FetchQuery = "SELECT * FROM interactives_templates LEFT JOIN interactives_templates_skills ON interactives_templates_skills.InteractiveTemplateId=interactives_templates.Id LEFT JOIN interactives_skills ON interactives_skills.Id=interactives_templates_skills.SkillId";
		private InteractiveTemplate m_current;
		public InteractiveTemplate Map(InteractiveTemplate template, InteractiveTemplateSkills binding, InteractiveSkillRecord skill)
		{
			InteractiveTemplate result;
			if (template == null)
			{
				result = this.m_current;
			}
			else
			{
				if (this.m_current != null && this.m_current.Id == template.Id)
				{
					if (binding.InteractiveTemplateId == this.m_current.Id && binding.SkillId == skill.Id)
					{
						this.m_current.Skills.Add(skill);
					}
					result = null;
				}
				else
				{
					InteractiveTemplate current = this.m_current;
					this.m_current = template;
					if (binding.InteractiveTemplateId == this.m_current.Id && binding.SkillId == skill.Id)
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
