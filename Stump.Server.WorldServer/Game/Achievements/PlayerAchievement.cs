using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Database.Achievements;
using Stump.Server.WorldServer.Game.Achievements.Criterions;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Actors.RolePlay;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Guilds;
using Stump.Server.WorldServer.Game.Items;
using Stump.Server.WorldServer.Game.Maps;
using Stump.Server.WorldServer.Handlers.Achievements;
using Stump.Server.WorldServer.Handlers.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stump.Server.WorldServer.Game.Achievements
{
    public class PlayerAchievement
    {
        // FIELDS
        public delegate void AchievementCompleted(Character character, AchievementTemplate achievement);

        private readonly object m_lock = new object();

        private List<AchievementTemplate> m_finishedAchievements;
        private Dictionary<string, AbstractCriterion> m_finishedCriterions;
        private Dictionary<AbstractCriterion, int> m_runningCriterions;

        private LevelCriterion m_levelCriterion;
        private AchievementPointsCriterion m_achievementPointsCriterion;
        private ChallengesCriterion m_challengesCriterion;

        // PROPERTIES
        public Character Owner
        {
            get;
            private set;
        }
        public IReadOnlyList<AchievementTemplate> FinishedAchievements
        {
            get
            {
                return this.m_finishedAchievements.AsReadOnly();
            }
        }
        public IReadOnlyList<PlayerAchievementReward> RewardAchievements
        {
            get
            {
                return this.Owner.Record.AchievementRewards;
            }
        }

        // CONSTRUCTORS
        public PlayerAchievement(Character character)
        {
            this.Owner = character;

            this.InitializeEvents();
        }

        // METHODS
        public void LoadAchievements()
        {
            this.m_finishedAchievements = new List<AchievementTemplate>();
            this.m_finishedCriterions = new Dictionary<string, AbstractCriterion>();
            this.m_runningCriterions = new Dictionary<AbstractCriterion, int>();

            foreach (var achievementId in this.Owner.Record.FinishedAchievements)
            {
                var achievement = Singleton<AchievementManager>.Instance.TryGetAchievement(achievementId);
                if (achievement != null)
                {
                    this.m_finishedAchievements.Add(achievement);
                }
            }
            foreach (var finishedAchievementObjective in this.Owner.Record.FinishedAchievementObjectives)
            {
                var achievementObjective = Singleton<AchievementManager>.Instance.TryGetAchievementObjective(finishedAchievementObjective);
                if (achievementObjective != null)
                {
                    this.m_finishedCriterions.Add(achievementObjective.Criterion, achievementObjective.AbstractCriterion);
                }
            }
            foreach (var runningAchievementObjective in this.Owner.Record.RunningAchievementObjectives)
            {
                var achievementObjective = Singleton<AchievementManager>.Instance.TryGetAchievementObjective((uint)runningAchievementObjective.Key);
                if (achievementObjective != null)
                {
                    this.m_runningCriterions.Add(achievementObjective.AbstractCriterion, runningAchievementObjective.Value);
                }
            }

            foreach (var rewardableAchievement in this.Owner.Record.AchievementRewards)
            {
                rewardableAchievement.Initialize(this.Owner);
            }
            
            this.ManageCriterions();
        }

        private void InitializeEvents()
        {
            this.Owner.ChangeSubArea += this.OnChangeSubArea;
            this.Owner.FightEnded += this.OnFightEnded;
            this.Owner.LevelChanged += this.OnLevelChanged;
        }

        public AchievementTemplate TryGetFinishedAchievement(short id)
        {
            return this.m_finishedAchievements.FirstOrDefault(entry => entry.Id == id);
        }
        public IEnumerable<Achievement> TryGetFinishedAchievements(AchievementCategoryRecord category)
        {
            var result = (from template in category.Achievements
                          where this.m_finishedAchievements.Contains(template)
                          select template.GetAchievement(this));

            return result;
        }

        #region Handlers

        private void OnFightEnded(Character character, CharacterFighter fighter)
        {
            if (fighter.Fight.FightType == FightTypeEnum.FIGHT_TYPE_PvM)
            {
                if (fighter.HasWin())
                {
                    if (fighter.Fight.GetChallenges().Any(entry => entry.IsSuccessful))
                    {
                        if (fighter.Fight.RedTeam.GetAllFighters().Sum(entry => entry.Level) < fighter.Fight.GetAllFighters().Sum(entry => entry.Level))
                        {
                            // TODO : 1 challenge
                        }

                        if (fighter.Fight.Map.IsDungeonSpawn)
                        {
                            // TODO : check si c'est un boss
                        }

                        foreach (var item in fighter.Fight.BlueTeam.GetAllFighters<MonsterFighter>())
                        {
                            var criterion = Singleton<AchievementManager>.Instance.TryGetCriterionByMonster(item.Monster.Template);
                            if (criterion != null)
                            {
                                if (this.m_runningCriterions.ContainsKey(criterion))
                                {
                                    this.m_runningCriterions[criterion]++;
                                }
                                else
                                {
                                    this.m_runningCriterions.Add(criterion, 1);
                                }

                                if (!this.ContainsCriterion(criterion.Criterion))
                                {
                                    if (criterion.Eval(this.Owner))
                                    {
                                        this.AddCriterion(criterion);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void OnChangeSubArea(RolePlayActor actor, SubArea subArea)
        {
            var achievement = subArea.Record.Achievement;
            lock (this.m_lock)
            {
                if (achievement != null)
                {
                    if (!this.m_finishedAchievements.Contains(achievement))
                    {
                        this.CompleteAchievement(achievement);
                    }
                }
            }
        }

        private void OnLevelChanged(Character character, byte currentLevel, int difference)
        {
            if (difference > 0)
            {
                this.ManageIncrementalCriterions(ref this.m_levelCriterion);
            }
        }

        private void OnAchievementCompleted(AchievementTemplate achievement)
        {
            var achievementCriterion = Singleton<AchievementManager>.Instance.TryGetAchievementCriterion(achievement);
            if (achievementCriterion != null)
            {
                this.AddCriterion(achievementCriterion);
            }
        }

        private void ManageCriterions()
        {
            this.m_levelCriterion = Singleton<AchievementManager>.Instance.MinLevelCriterion;
            this.m_achievementPointsCriterion = Singleton<AchievementManager>.Instance.MinAchievementPointsCriterion;

            this.ManageIncrementalCriterions(ref this.m_levelCriterion);
            this.ManageIncrementalCriterions(ref this.m_challengesCriterion);
            this.ManageIncrementalCriterions(ref this.m_achievementPointsCriterion);
        }

        private void ManageIncrementalCriterions<T>(ref T criterion)
            where T : AbstractCriterion
        {
            while (criterion != null && criterion.Eval(this.Owner))
            {
                if (!this.ContainsCriterion(criterion.Criterion))
                {
                    this.AddCriterion(criterion);
                }

                criterion = (T)criterion.Next;
            }
        }

        #endregion

        #region Criterions manager

        public void AddCriterion(AbstractCriterion criterion)
        {
            this.m_finishedCriterions.Add(criterion.Criterion, criterion);

            foreach (var item in criterion.UsefullFor)
            {
                if (item.Objectives.All(entry => this.m_finishedCriterions.ContainsKey(entry.Criterion)))
                {
                    this.CompleteAchievement(item);
                }
            }

            this.Owner.Record.FinishedAchievementObjectives.Add(criterion.DefaultObjective.Id);
        }
        public bool ContainsCriterion(string criterion)
        {
            return this.m_finishedCriterions.ContainsKey(criterion);
        }

        #endregion

        private void CompleteAchievement(AchievementTemplate achievement)
        {
            lock (this.m_lock)
            {
                var reward = this.Owner.Record.AchievementRewards.FirstOrDefault(entry => entry == this.Owner.Level);
                if (reward == null)
                {
                    reward = new PlayerAchievementReward(this.Owner, achievement);

                    this.Owner.Record.AchievementRewards.Add(reward);
                }
                else
                {
                    reward.AddRewardableAchievement(achievement);
                }

                this.Owner.Record.FinishedAchievements.Add((ushort)achievement.Id);
                this.Owner.Record.AchievementPoints += (int)achievement.Points;

                this.m_finishedAchievements.Add(achievement);
            }

            AchievementHandler.SendAchievementFinishedMessage(this.Owner.Client, (ushort)achievement.Id, this.Owner.Level);

            this.OnAchievementCompleted(achievement);
        }

        public List<AchievementRewardable> GetRewardableAchievements()
        {
            var achievements = new List<AchievementRewardable>();
            foreach (var item in this.RewardAchievements)
            {
                achievements.AddRange(item.GetRewardableAchievements());
            }

            return achievements;
        }

        public int GetRunningCriterion(AbstractCriterion criterion)
        {
            return this.m_runningCriterions.ContainsKey(criterion) ? this.m_runningCriterions[criterion] : 0;
        }

        public bool RewardAchievement(AchievementTemplate achievement, out int experience, out int guildExperience)
        {
            bool result;
            PlayerAchievementReward reward = null;
            experience = 0;
            guildExperience = 0;

            if (achievement != null)
            {
                lock (this.m_lock)
                {
                    foreach (var item in this.Owner.Record.AchievementRewards)
                    {
                        if (item.Contains(achievement))
                        {
                            reward = item;
                            break;
                        }
                    }
                }

                if (reward != null)
                {
                    result = this.RewardAchievement(achievement, reward, out experience, out guildExperience);
                }
                else
                {
                    result = false;
                }

            }
            else
            {
                result = false;
            }

            return result;
        }
        public bool RewardAchievement(AchievementTemplate achievement, PlayerAchievementReward owner, out int experience, out int guildExperience)
        {
            experience = 0;
            guildExperience = 0;
            if (!owner.Remove(achievement))
            {
                return false;
            }

            experience = achievement.GetExperienceReward(this.Owner.Client);
            if (experience > 0)
            {
                if (this.Owner.GuildMember != null && this.Owner.GuildMember.GivenPercent > 0)
                {
                    int guildXP = (int)(experience * (this.Owner.GuildMember.GivenPercent * 0.01));
                    int adjustedGuildExperience = (int)this.Owner.Guild.AdjustGivenExperience(this.Owner, (long)guildXP);
                    adjustedGuildExperience = Math.Min(Guild.MaxGuildXP, adjustedGuildExperience);

                    experience = (int)(experience * (100.0 - this.Owner.GuildMember.GivenPercent) * 0.01);
                    if (adjustedGuildExperience > 0)
                    {
                        guildExperience = adjustedGuildExperience;
                    }
                }
            }
            if (experience < 0) { experience = 0; }

            var kamas = achievement.GetKamasReward(this.Owner.Client);
            if (kamas > 0) { this.Owner.Inventory.AddKamas(kamas); }

            foreach (var item in achievement.Rewards)
            {
                for (int i = 0; i < item.ItemsReward.Length; i++)
                {
                    var id = item.ItemsReward[i];
                    var quantity = item.ItemsQuantityReward[i];

                    var itemTemplate = Singleton<ItemManager>.Instance.TryGetTemplate((int)id);
                    if (itemTemplate != null)
                    {
                        this.Owner.Inventory.AddItem(itemTemplate, quantity);
                    }
                }

                foreach (var emoteId in item.EmotesReward)
                {
                    // TODO
                }

                foreach (int spellId in item.SpellsReward)
                {
                    if (!this.Owner.Spells.HasSpell(spellId))
                    {
                        this.Owner.Spells.LearnSpell(spellId);
                    }
                }

                foreach (ushort titleId in item.TitlesReward)
                {
                    if (!this.Owner.HasTitle(titleId))
                    {
                        this.Owner.AddTitle(titleId);
                    }
                }

                foreach (ushort ornamentId in item.OrnamentsReward)
                {
                    if (!this.Owner.HasOrnament(ornamentId))
                    {
                        this.Owner.AddOrnament(ornamentId);
                    }
                }
            }
            // TODO : items

            if (!owner.Any())
            {
                this.Owner.Record.AchievementRewards.Remove(owner);
            }

            return true;
        }

        public void RewardAllAchievements(System.Action<AchievementTemplate, bool> action)
        {
            var totalExperience = 0;
            var totalGuildExperience = 0;
            int experience;
            int guildExperience;
            lock (this.m_lock)
            {
                while (this.RewardAchievements.Count > 0)
                {
                    var achievementReward = this.RewardAchievements[0];
                    while (achievementReward.RewardAchievements.Count > 0)
                    {
                        var achievement = achievementReward.RewardAchievements[0];

                        action(achievement, this.RewardAchievement(achievement, achievementReward, out experience, out guildExperience));
                        totalExperience += experience;
                        totalGuildExperience += guildExperience;
                    }
                }
            }

            if (totalExperience > 0) { this.Owner.AddExperience(totalExperience); }
            else { totalExperience = 0; }

            if (this.Owner.GuildMember != null && totalGuildExperience > 0) { this.Owner.GuildMember.AddXP(totalGuildExperience); }
            else { totalGuildExperience = 0; }

            CharacterHandler.SendCharacterExperienceGainMessage(this.Owner.Client, (ulong)totalExperience, 0L, (ulong)totalGuildExperience, 0L);
        }
    }
}
