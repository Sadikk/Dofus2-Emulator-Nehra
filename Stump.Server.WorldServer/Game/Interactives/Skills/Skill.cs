using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Database.Interactives;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Interactives.Skills
{
	public abstract class Skill
	{
		public int Id
		{
			get;
			private set;
		}
		public InteractiveSkillRecord Record
		{
			get;
			private set;
		}
		public InteractiveObject InteractiveObject
		{
			get;
			private set;
		}
		protected Skill(int id, InteractiveSkillRecord record, InteractiveObject interactiveObject)
		{
			this.Id = id;
			this.Record = record;
			this.InteractiveObject = interactiveObject;
		}
		public virtual uint GetDuration(Character character)
		{
			return 0u;
		}
		public abstract bool IsEnabled(Character character);
		public abstract void Execute(Character character);
		public virtual void PostExecute(Character character)
		{
		}
		public InteractiveElementSkill GetInteractiveElementSkill()
		{
			return new InteractiveElementSkill((uint)this.Record.Template.Id, this.Id);
		}
	}
}
