using Stump.Core.Reflection;
using Stump.Server.WorldServer.Database.Achievements;
using Stump.Server.WorldServer.Database.Monsters;
using Stump.Server.WorldServer.Game.Achievements.Criterions.Data;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Monsters;
using Stump.Server.WorldServer.Game.Conditions;
using System;

namespace Stump.Server.WorldServer.Game.Achievements.Criterions
{
    public class KillMonsterWithChallengeCriterion : AbstractCriterion<KillMonsterWithChallengeCriterion, DefaultCriterionData>
    {
        // FIELDS
        public const string Identifier = "Ef";
        private MonsterTemplate m_monster;
        private ushort? m_maxValue;
        
        // PROPERTIES
        public int MonsterId
        {
            get
            {
                return this[0][0];
            }
        }
        public MonsterTemplate Monster
        {
            get
            {
                return this.m_monster;
            }
        }
        public int Number
        {
            get
            {
                return this[0][1];
            }
        }
        public override bool IsIncrementable
        {
            get
            {
                return false;
            }
        }
        public override ushort MaxValue
        {
            get
            {
                if (this.m_maxValue == null)
                {
                    this.m_maxValue = new ushort?((ushort)this.Number);

                    switch (base[0].Operator)
                    {
                        case ComparaisonOperatorEnum.EQUALS: break;

                        case ComparaisonOperatorEnum.INFERIOR:
                            throw new Exception();

                        case ComparaisonOperatorEnum.SUPERIOR:
                            this.m_maxValue++;
                            break;

                        default:
                            break;
                    }
                }

                return this.m_maxValue.Value;
            }
        }

        // CONSTRUCTORS
        public KillMonsterWithChallengeCriterion(AchievementObjectiveRecord objective)
            : base(objective)
        {
            this.m_monster = Singleton<MonsterManager>.Instance.GetTemplate(this.MonsterId);
        }

        // METHODS
        public override DefaultCriterionData Parse(ComparaisonOperatorEnum @operator, params string[] parameters)
        {
            return new DefaultCriterionData(@operator, parameters);
        }

        public override bool Eval(Character character)
        {
            return character.Achievement.GetRunningCriterion(this) >= this.Number;
        }

        public override bool Lower(AbstractCriterion left)
        {
            return this.Number < ((KillMonsterWithChallengeCriterion)left).Number;
        }
        public override bool Greater(AbstractCriterion left)
        {
            return this.Number > ((KillMonsterWithChallengeCriterion)left).Number;
        }

        public override ushort GetPlayerValue(PlayerAchievement player)
        {
            return (ushort)Math.Min(this.MaxValue, player.GetRunningCriterion(this));
        }
    }
}
