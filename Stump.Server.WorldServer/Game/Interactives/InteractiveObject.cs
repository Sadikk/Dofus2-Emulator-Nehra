using Stump.Core.Reflection;
using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Database.Interactives;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Interactives.Skills;
using Stump.Server.WorldServer.Game.Maps;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Interactives
{
	public class InteractiveObject : WorldObject
	{
		private readonly System.Collections.Generic.Dictionary<int, Skill> m_skills = new System.Collections.Generic.Dictionary<int, Skill>();
		public InteractiveSpawn Spawn
		{
			get;
			private set;
		}
		public bool CanSelectSkill
		{
			get
			{
				return this.Template != null;
			}
		}
		public override int Id
		{
			get
			{
				return this.Spawn.ElementId;
			}
			protected set
			{
				this.Spawn.ElementId = value;
			}
		}
		public InteractiveTemplate Template
		{
			get
			{
				return this.Spawn.Template;
			}
		}
		public InteractiveObject(InteractiveSpawn spawn)
		{
			this.Spawn = spawn;
			this.Position = spawn.GetPosition();
			this.GenerateSkills();
		}
		private void GenerateSkills()
		{
			foreach (InteractiveSkillRecord current in this.Spawn.GetSkills())
			{
				int num = Singleton<InteractiveManager>.Instance.PopSkillId();
				this.m_skills.Add(num, current.GenerateSkill(num, this));
			}
		}
		public Skill GetSkill(int id)
		{
			Skill skill;
			Skill result;
			if (!this.m_skills.TryGetValue(id, out skill))
			{
				result = null;
			}
			else
			{
				result = skill;
			}
			return result;
		}
		public System.Collections.Generic.IEnumerable<Skill> GetSkills()
		{
			return this.m_skills.Values;
		}
		public System.Collections.Generic.IEnumerable<Skill> GetEnabledSkills(Character character)
		{
			return 
				from entry in this.m_skills.Values
				where entry.IsEnabled(character)
				select entry;
		}
		public System.Collections.Generic.IEnumerable<Skill> GetDisabledSkills(Character character)
		{
			return 
				from entry in this.m_skills.Values
				where !entry.IsEnabled(character)
				select entry;
		}
		public System.Collections.Generic.IEnumerable<InteractiveElementSkill> GetEnabledElementSkills(Character character)
		{
			return 
				from entry in this.m_skills.Values
				where entry.IsEnabled(character)
				select entry.GetInteractiveElementSkill();
		}
		public System.Collections.Generic.IEnumerable<InteractiveElementSkill> GetDisabledElementSkills(Character character)
		{
			return 
				from entry in this.m_skills.Values
				where !entry.IsEnabled(character)
				select entry.GetInteractiveElementSkill();
		}
		public InteractiveElement GetInteractiveElement(Character character)
		{
			return new InteractiveElement(this.Id, (this.Template != null) ? this.Template.Id : -1, this.GetEnabledElementSkills(character), this.GetDisabledElementSkills(character));
		}
	}
}
