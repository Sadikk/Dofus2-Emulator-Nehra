﻿using Stump.Core.Reflection;
using Stump.Server.WorldServer.Database.Achievements;
using Stump.Server.WorldServer.Game.Achievements.Criterions.Data;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Conditions;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Stump.Server.WorldServer.Game.Achievements.Criterions
{
    public abstract class AbstractCriterion<T, Data> : AbstractCriterion
        where T : AbstractCriterion<T, Data>
        where Data : CriterionData
    {
        // FIELDS
        private List<Data> m_criterions;

        // PROPERTIES
        public Data this[int index]
        {
            get
            {
                return this.m_criterions[index];
            }
        }

        // CONSTRUCTORS
        public AbstractCriterion(AchievementObjectiveRecord objective)
        {
            this.m_criterions = new List<Data>();

            this.Parse(objective);
        }

        // METHODS
        public abstract Data Parse(ComparaisonOperatorEnum @operator, params string[] parameters);
        public override void Parse(AchievementObjectiveRecord objective)
        {
            base.m_defaultObjective = objective;

            var criterions = objective.Criterion.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in criterions)
            {
                var match = Regex.Match(item, "(?<name>[a-zA-Z]{2})(?<operator>[<=>]{1})(?<content>.*)");
                if (match.Success)
                {
                    this.m_criterions.Add(this.Parse(
                        AbstractCriterion.GetOperator(match.Groups["operator"].Value[0]), 
                        match.Groups["content"].Value.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries)));
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        public T GetNext()
        {
            return base.Next as T;
        }
    }

    public abstract class AbstractCriterion
    {
        // FIELDS
        protected AchievementObjectiveRecord m_defaultObjective;
        protected List<AchievementTemplate> m_usefullFor;
        private AbstractCriterion m_next;

        // PROPERTIES
        public string Criterion
        {
            get
            {
                return this.m_defaultObjective.Criterion;
            }
        }
        public AchievementObjectiveRecord DefaultObjective
        {
            get
            {
                return this.m_defaultObjective;
            }
        }
        public IReadOnlyList<AchievementTemplate> UsefullFor
        {
            get
            {
                return this.m_usefullFor.AsReadOnly();
            }
        }
        public virtual ushort MaxValue
        {
            get
            {
                return 1;
            }
        }
        public virtual bool IsIncrementable
        {
            get
            {
                return false;
            }
        }
        public AbstractCriterion Next
        {
            get
            {
                return this.m_next;
            }
            set
            {
                this.m_next = value;
            }
        }

        // CONSTRUCTORS
        public AbstractCriterion()
        {
            this.m_usefullFor = new List<AchievementTemplate>();
        }

        // METHODS
        public static ComparaisonOperatorEnum GetOperator(char c)
        {
            ComparaisonOperatorEnum result;
            switch(c)
            {
                case '<':
                    result = ComparaisonOperatorEnum.INFERIOR;
                    break;
                case '>':
                    result = ComparaisonOperatorEnum.SUPERIOR;
                    break;
                case '=':
                    result = ComparaisonOperatorEnum.EQUALS;
                    break;

                default:
                    throw new Exception("");
            }

            return result;
        }

        public static AbstractCriterion CreateCriterion(AchievementObjectiveRecord objective)
        {
            AbstractCriterion result;
            if (!string.IsNullOrWhiteSpace(objective.Criterion) && objective.Criterion.Length >= 2)
            {
                if (!Singleton<AchievementManager>.Instance.TryGetAbstractCriterion(objective.Criterion, out result))
                {
                    var name = objective.Criterion.Substring(0, 2);

                    switch (name)
                    {
                        case AchievementCriterion.Identifier:
                            var achievementCriterion = new AchievementCriterion(objective);
                            result = achievementCriterion;

                            if (achievementCriterion.Achievement != null)
                            {
                                Singleton<AchievementManager>.Instance.AddAchievementCriterion(achievementCriterion);
                            }
                            break;

                        case AchievementPointsCriterion.Identifier:
                            result = new AchievementPointsCriterion(objective);
                            break;

                        case LevelCriterion.Identifier:
                            result = new LevelCriterion(objective);
                            break;

                        case ChallengesCriterion.Identifier:
                            result = new ChallengesCriterion(objective);
                            break;

                        case KillMonsterWithChallengeCriterion.Identifier:
                            var monsterCriterion = new KillMonsterWithChallengeCriterion(objective);
                            result = monsterCriterion;

                            if (monsterCriterion.Monster != null)
                            {
                                Singleton<AchievementManager>.Instance.AddKillMonsterWithChallengeCriterion(monsterCriterion);
                            }
                            break;

                        default:
                            return null;
                    }

                    Singleton<AchievementManager>.Instance.AddCriterion(result);
                }
            }
            else
            {
                result = null;
            }

            return result;
        }

        public void AddUsefullness(AchievementTemplate template)
        {
            if (template != null)
            {
                this.m_usefullFor.Add(template);
            }
        }

        public abstract void Parse(AchievementObjectiveRecord objective);

        public virtual DofusProtocol.Types.AchievementObjective GetAchievementObjective(AchievementObjectiveRecord objectiveRecord, PlayerAchievement player)
        {
            return new DofusProtocol.Types.AchievementObjective(objectiveRecord.Id, this.MaxValue);
        }

        public virtual bool Eval(Character character)
        {
            return false;
        }

        public virtual bool Lower(AbstractCriterion left)
        {
            return false;
        }
        public virtual bool Greater(AbstractCriterion left)
        {
            return false;
        }

        public static bool operator <(AbstractCriterion right, AbstractCriterion left)
        {
            return right.Lower(left);
        }
        public static bool operator >(AbstractCriterion right, AbstractCriterion left)
        {
            return right.Greater(left);
        }

        public virtual ushort GetPlayerValue(PlayerAchievement player)
        {
            return 0;
        }
    }
}
