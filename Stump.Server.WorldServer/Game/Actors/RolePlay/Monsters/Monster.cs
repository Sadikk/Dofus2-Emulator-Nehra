using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Database.Monsters;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Actors.Look;
using Stump.Server.WorldServer.Game.Fights.Teams;

namespace Stump.Server.WorldServer.Game.Actors.RolePlay.Monsters
{
	public class Monster
	{
		public MonsterGroup Group
		{
			get;
			private set;
		}
		public MonsterGrade Grade
		{
			get;
			private set;
		}
		public MonsterTemplate Template
		{
			get
			{
				return this.Grade.Template;
			}
		}
		public ActorLook Look
		{
			get
			{
				return this.Template.EntityLook;
			}
		}
		public Monster(MonsterGrade grade, MonsterGroup group)
		{
			this.Grade = grade;
			this.Group = group;
		}
		public MonsterFighter CreateFighter(FightTeam team)
		{
			return new MonsterFighter(team, this);
		}
		public MonsterInGroupInformations GetMonsterInGroupInformations()
		{
			return new MonsterInGroupInformations(this.Template.Id, (sbyte)this.Grade.GradeId, this.Look.GetEntityLook());
		}
		public MonsterInGroupLightInformations GetMonsterInGroupLightInformations()
		{
			return new MonsterInGroupLightInformations(this.Template.Id, (sbyte)this.Grade.GradeId);
		}
		public override string ToString()
		{
			return string.Format("{0} ({1})", this.Template.Name, this.Template.Id);
		}
	}
}
