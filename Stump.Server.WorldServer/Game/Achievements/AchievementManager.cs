using NLog;
using Stump.Server.BaseServer.Database;
using Stump.Server.BaseServer.Initialization;
using Stump.Server.WorldServer.Database.Achievements;
using Stump.Server.WorldServer.Database.Monsters;
using Stump.Server.WorldServer.Game.Achievements.Criterions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stump.Server.WorldServer.Game.Achievements
{
    public class AchievementManager : DataManager<AchievementManager>
    {
        // FIELDS
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private Dictionary<uint, AchievementTemplate> m_achievementsTemplates;
        private Dictionary<uint, AchievementRewardRecord> m_achievemenstRewards;
        private Dictionary<uint, AchievementCategoryRecord> m_achievementsCategories;
        private Dictionary<uint, AchievementObjectiveRecord> m_achievementsObjectives;

        private Dictionary<AchievementTemplate, AchievementCriterion> m_achievementCriterions;
        private Dictionary<string, AbstractCriterion> m_criterions;
        private Dictionary<MonsterTemplate, KillMonsterWithChallengeCriterion> m_monsterCriterions;
        private Dictionary<Type, AbstractCriterion> m_incrementableCriterions;

        // PROPERTIES
        public LevelCriterion MinLevelCriterion
        {
            get
            {
                return this.m_incrementableCriterions[typeof(LevelCriterion)] as LevelCriterion;
            }
        }
        public AchievementPointsCriterion MinAchievementPointsCriterion
        {
            get
            {
                return this.m_incrementableCriterions[typeof(AchievementPointsCriterion)] as AchievementPointsCriterion;
            }
        }

        // CONSTRUCTORS
        private AchievementManager() { }

        // METHODS
        [Initialization(InitializationPass.Seventh)]
        public override void Initialize()
        {
            base.Initialize();

            this.m_criterions = new Dictionary<string, AbstractCriterion>();
            this.m_monsterCriterions = new Dictionary<MonsterTemplate, KillMonsterWithChallengeCriterion>();
            this.m_achievementCriterions = new Dictionary<AchievementTemplate, AchievementCriterion>();
            this.m_incrementableCriterions = new Dictionary<Type, AbstractCriterion>();

            this.m_achievementsTemplates = base.Database.Query<AchievementTemplate>(AchievementTemplateRelator.FetchQuery, new object[0]).ToDictionary(entry => entry.Id);

            this.m_achievemenstRewards = base.Database.Query<AchievementRewardRecord>(AchievementRewardRelator.FetchQuery, new object[0]).ToDictionary(entry => entry.Id);
            this.m_achievementsCategories = base.Database.Query<AchievementCategoryRecord>(AchievementCategoryRelator.FetchQuery, new object[0]).ToDictionary(entry => entry.Id);
            this.m_achievementsObjectives = base.Database.Query<AchievementObjectiveRecord>(AchievementObjectiveRelator.FetchQuery, new object[0]).ToDictionary(entry => entry.Id);

            foreach (var pair in this.m_achievementsTemplates)
            {
                pair.Value.Initialize();
            }
            foreach(var pair in this.m_achievementsObjectives)
            {
                pair.Value.Initialize();
            }
        }

        public IEnumerable<ushort> GetAchievementsIds()
        {
            return this.m_achievementsTemplates.Keys.Select(entry => (ushort)entry);
        }
        public IEnumerable<ushort> GetAchievementsIdsByCategory(uint category)
        {
            return this.m_achievementsTemplates
                .Where(entry => entry.Value.CategoryId == category)
                .Select(entry => (ushort)entry.Key);
        }

        public void AddCriterion(AbstractCriterion criterion)
        {
            if (criterion == null)
            { }

            this.m_criterions.Add(criterion.Criterion, criterion);
            if (criterion.IsIncrementable)
            {
                this.AddIncrementableCriterion(criterion);
            }
        }
        public bool AddAchievementCriterion(AchievementCriterion criterion)
        {
            bool result;
            if (this.m_achievementCriterions.ContainsKey(criterion.Achievement))
            {
                result = false;
            }
            else
            {
                this.m_achievementCriterions.Add(criterion.Achievement, criterion);
                result = true;
            }
            return result;
        }
        public bool AddKillMonsterWithChallengeCriterion(KillMonsterWithChallengeCriterion criterion)
        {
            bool result;
            if (this.m_monsterCriterions.ContainsKey(criterion.Monster))
            {
                result = false;
            }
            else
            {
                this.m_monsterCriterions.Add(criterion.Monster, criterion);
                result = true;
            }
            return result;
        }

        private bool AddIncrementableCriterion(AbstractCriterion criterion)
        {
            var criterionType = criterion.GetType();
            if (!this.m_incrementableCriterions.ContainsKey(criterionType))
            {
                this.m_incrementableCriterions.Add(criterionType, criterion);
            }
            else
            {
                var min = this.m_incrementableCriterions[criterionType];
                if (min < criterion)
                {
                    var temp = min;
                    var next = min.Next;
                    while (next != null && next < criterion)
                    {
                        temp = next;
                        next = temp.Next;
                    }

                    if (next == null)
                    {
                        temp.Next = criterion;
                    }
                    else
                    {
                        criterion.Next = next;
                        temp.Next = criterion;
                    }
                }
                else
                {
                    criterion.Next = min;
                    this.m_incrementableCriterions[criterionType] = criterion;
                }
            }

            return true;
        }

        public AchievementCriterion TryGetAchievementCriterion(AchievementTemplate achievement)
        {
            return this.m_achievementCriterions.ContainsKey(achievement) ? this.m_achievementCriterions[achievement] : null;
        }
        public bool TryGetAbstractCriterion(string criterion, out AbstractCriterion result)
        {
            result = null;
            if (this.m_criterions.ContainsKey(criterion))
            {
                result = this.m_criterions[criterion];
                return true;
            }

            return false;
        }
        public AchievementTemplate TryGetAchievement(uint id)
        {
            return this.m_achievementsTemplates.ContainsKey(id) ? this.m_achievementsTemplates[id] : null;
        }
        public AchievementCategoryRecord TryGetAchievementCategory(uint id)
        {
            return this.m_achievementsCategories.ContainsKey(id) ? this.m_achievementsCategories[id] : null;
        }
        public AchievementObjectiveRecord TryGetAchievementObjective(uint id)
        {
            return this.m_achievementsObjectives.ContainsKey(id) ? this.m_achievementsObjectives[id] : null;
        }
        public AchievementRewardRecord TryGetAchievementReward(uint id)
        {
            return this.m_achievemenstRewards.ContainsKey(id) ? this.m_achievemenstRewards[id] : null;
        }

        public KillMonsterWithChallengeCriterion TryGetCriterionByMonster(MonsterTemplate template)
        {
            return this.m_monsterCriterions.ContainsKey(template) ? this.m_monsterCriterions[template] : null;
        }
    }
}
