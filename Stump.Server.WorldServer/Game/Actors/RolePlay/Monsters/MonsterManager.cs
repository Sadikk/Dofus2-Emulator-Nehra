using Stump.Server.BaseServer.Database;
using Stump.Server.BaseServer.Initialization;
using Stump.Server.WorldServer.Database.Monsters;
using System;
using System.Linq;

namespace Stump.Server.WorldServer.Game.Actors.RolePlay.Monsters
{
	public class MonsterManager : DataManager<MonsterManager>
	{
		private System.Collections.Generic.Dictionary<int, MonsterTemplate> m_monsterTemplates;
		private System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<MonsterSpell>> m_monsterSpells;
		private System.Collections.Generic.Dictionary<int, MonsterSpawn> m_monsterSpawns;
		private System.Collections.Generic.Dictionary<int, MonsterDisableSpawn> m_monsterDisableSpawns;
		private System.Collections.Generic.Dictionary<int, MonsterDungeonSpawn> m_monsterDungeonsSpawns;
		private System.Collections.Generic.Dictionary<int, DroppableItem> m_droppableItems;
		private System.Collections.Generic.Dictionary<int, MonsterGrade> m_monsterGrades;
		private System.Collections.Generic.Dictionary<int, MonsterSuperRace> m_monsterSuperRaces;

		[Initialization(InitializationPass.Sixth)]
		public override void Initialize()
		{
			this.m_monsterTemplates = base.Database.Query<MonsterTemplate>(MonsterTemplateRelator.FetchQuery, new object[0]).ToDictionary((MonsterTemplate entry) => entry.Id);
			this.m_monsterGrades = base.Database.Query<MonsterGrade>(MonsterGradeRelator.FetchQuery, new object[0]).ToDictionary((MonsterGrade entry) => entry.Id);
			this.m_monsterSpells = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<MonsterSpell>>();

			foreach (MonsterSpell current in base.Database.Query<MonsterSpell>(MonsterSpellRelator.FetchQuery, new object[0]))
			{
				System.Collections.Generic.List<MonsterSpell> list;
				if (!this.m_monsterSpells.TryGetValue(current.MonsterGradeId, out list))
				{
					this.m_monsterSpells.Add(current.MonsterGradeId, list = new System.Collections.Generic.List<MonsterSpell>());
				}
				list.Add(current);
			}

			this.m_monsterSpawns = base.Database.Query<MonsterSpawn>(MonsterSpawnRelator.FetchQuery, new object[0]).ToDictionary((MonsterSpawn entry) => entry.Id);
			this.m_monsterDisableSpawns = base.Database.Query<MonsterDisableSpawn>(MonsterDisableSpawnRelator.FetchQuery, new object[0]).ToDictionary((MonsterDisableSpawn entry) => entry.Id);
			this.m_monsterDungeonsSpawns = base.Database.Query<MonsterDungeonSpawn, MonsterDungeonSpawnEntity, MonsterDungeonSpawn>(new Func<MonsterDungeonSpawn, MonsterDungeonSpawnEntity, MonsterDungeonSpawn>(new MonsterDungeonSpawnRelator().Map), MonsterDungeonSpawnRelator.FetchQuery, new object[0]).ToDictionary((MonsterDungeonSpawn entry) => entry.Id);
			this.m_droppableItems = base.Database.Query<DroppableItem>(DroppableItemRelator.FetchQuery, new object[0]).ToDictionary((DroppableItem entry) => entry.Id);
			this.m_monsterSuperRaces = base.Database.Query<MonsterSuperRace>(MonsterSuperRaceRelator.FetchQuery, new object[0]).ToDictionary((MonsterSuperRace entry) => entry.Id);

            foreach (var pair in this.m_monsterDungeonsSpawns)
            {
                foreach (MonsterDungeonSpawnEntity current in pair.Value.GroupMonsters)
                {
                    current.PossibleMonsterGrades = (from grade in this.m_monsterGrades
                                                     where grade.Value.MonsterId == current.MonsterId
                                                     where current.PossibleMonsterGradeIds.Contains(grade.Value.GradeId)
                                                     select grade.Value).ToList();
                }
            }
		}
		public MonsterGrade[] GetMonsterGrades()
		{
			return this.m_monsterGrades.Values.ToArray<MonsterGrade>();
		}
		public MonsterGrade GetMonsterGrade(int id)
		{
			MonsterGrade monsterGrade;
			return (!this.m_monsterGrades.TryGetValue(id, out monsterGrade)) ? null : monsterGrade;
		}
		public MonsterGrade GetMonsterGrade(int monsterId, int grade)
		{
            MonsterTemplate template = this.GetTemplate(monsterId);

            if (template == null)
            {
                return null;
            }

			return (template.Grades.Count <= grade - 1) ? null : template.Grades[grade - 1];
		}
		public System.Collections.Generic.List<MonsterGrade> GetMonsterGrades(int monsterId)
		{
			return (
				from entry in this.m_monsterGrades
				where entry.Value.MonsterId == monsterId
				select entry.Value).ToList<MonsterGrade>();
		}
		public System.Collections.Generic.List<MonsterSpell> GetMonsterGradeSpells(int id)
		{
			System.Collections.Generic.List<MonsterSpell> list;
			return this.m_monsterSpells.TryGetValue(id, out list) ? list : new System.Collections.Generic.List<MonsterSpell>();
		}
		public System.Collections.Generic.List<DroppableItem> GetMonsterDroppableItems(int id)
		{
			return (
				from entry in this.m_droppableItems
				where entry.Value.MonsterOwnerId == id
				select entry.Value).ToList<DroppableItem>();
		}
		public MonsterSuperRace GetSuperRace(int id)
		{
			MonsterSuperRace monsterSuperRace;
			return (!this.m_monsterSuperRaces.TryGetValue(id, out monsterSuperRace)) ? null : monsterSuperRace;
		}
		public MonsterTemplate GetTemplate(int id)
		{
			MonsterTemplate monsterTemplate;
			return (!this.m_monsterTemplates.TryGetValue(id, out monsterTemplate)) ? null : monsterTemplate;
		}
		public MonsterTemplate[] GetTemplates()
		{
			return this.m_monsterTemplates.Values.ToArray<MonsterTemplate>();
		}
		public MonsterTemplate GetTemplate(string name, bool ignoreCommandCase)
		{
			return this.m_monsterTemplates.Values.FirstOrDefault((MonsterTemplate entry) => entry.Name.Equals(name, ignoreCommandCase ? System.StringComparison.InvariantCultureIgnoreCase : System.StringComparison.InvariantCulture));
		}
		public void AddMonsterSpell(MonsterSpell spell)
		{
			base.Database.Insert(spell);
			System.Collections.Generic.List<MonsterSpell> list;
			if (!this.m_monsterSpells.TryGetValue(spell.MonsterGradeId, out list))
			{
				this.m_monsterSpells.Add(spell.MonsterGradeId, list = new System.Collections.Generic.List<MonsterSpell>());
			}
			list.Add(spell);
		}
		public void RemoveMonsterSpell(MonsterSpell spell)
		{
			base.Database.Delete(spell);
			this.m_monsterSpells.Remove(spell.Id);
		}
		public MonsterSpawn[] GetMonsterSpawns()
		{
			return this.m_monsterSpawns.Values.ToArray<MonsterSpawn>();
		}
		public bool GetMonsterDisableSpawns(int id, int areaId)
		{
			return this.m_monsterDisableSpawns.Values.FirstOrDefault((MonsterDisableSpawn x) => x.MonsterId == id && (x.SubAreaId == areaId || x.SubAreaId == -1)) != null;
		}
		public MonsterDungeonSpawn[] GetMonsterDungeonsSpawns()
		{
			return this.m_monsterDungeonsSpawns.Values.ToArray<MonsterDungeonSpawn>();
		}
		public MonsterSpawn GetOneMonsterSpawn(System.Predicate<MonsterSpawn> predicate)
		{
			return this.m_monsterSpawns.Values.SingleOrDefault((MonsterSpawn entry) => predicate(entry));
		}
		public void AddMonsterSpawn(MonsterSpawn spawn)
		{
			base.Database.Insert(spawn);
			this.m_monsterSpawns.Add(spawn.Id, spawn);
		}
		public void AddMonsterDisableSpawn(MonsterDisableSpawn spawn)
		{
			base.Database.Insert(spawn);
			this.m_monsterDisableSpawns.Add(spawn.Id, spawn);
		}
		public void RemoveMonsterSpawn(MonsterSpawn spawn)
		{
			base.Database.Delete(spawn);
			this.m_monsterSpawns.Remove(spawn.Id);
		}
		public void RemoveMonsterDisableSpawn(MonsterDisableSpawn spawn)
		{
			base.Database.Delete(spawn);
			this.m_monsterSpawns.Remove(spawn.Id);
		}
		public void AddMonsterDrop(DroppableItem drop)
		{
			base.Database.Insert(drop);
			this.m_droppableItems.Add(drop.Id, drop);
		}
		public void RemoveMonsterDrop(DroppableItem drop)
		{
			base.Database.Delete(drop);
			this.m_droppableItems.Remove(drop.Id);
		}
	}
}
