using Stump.Core.Reflection;
using Stump.Server.BaseServer.Database;
using Stump.Server.BaseServer.Initialization;
using Stump.Server.WorldServer.Database;
using Stump.Server.WorldServer.Database.Challenges;
using Stump.Server.WorldServer.Game.Fights;
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.Core.Extensions;

namespace Stump.Server.WorldServer.Game.Challenges
{
    public class ChallengeManager : DataManager<ChallengeManager>, ISaveable
    {
        // FIELDS
        private Dictionary<int, ChallengeRecord> m_challenges;
        private List<ChallengeChecker> m_checkerPatterns;

        // PROPERTIES

        // CONSTRUCTORS
        private ChallengeManager() { }

        // METHODS
        [Initialization(InitializationPass.Seventh)]
        public override void Initialize()
        {
            this.m_challenges = base.Database.Query<ChallengeRecord>(ChallengeRelator.FetchQuery, new object[0])
                .ToDictionary(entry => entry.Id);

            this.m_checkerPatterns = (from type in typeof(ChallengeChecker).Assembly.GetTypes()
                                      where type.IsSubclassOf(typeof(ChallengeChecker))
                                      select (ChallengeChecker)Activator.CreateInstance(type)).ToList<ChallengeChecker>();

            Singleton<World>.Instance.RegisterSaveableInstance(this);
        }

        public ChallengeRecord GetChallenge(int id)
        {
            return this.m_challenges.ContainsKey(id) ? this.m_challenges[id] : null;
        }

        public void GenerateChallenges(Fight fight)
        {
            var checkers = this.m_checkerPatterns.Where(entry => entry.IsCompatible(fight))
                .ToList();

            for (int i = 0; i < fight.NumberChallenges && checkers.Count > 0; i++)
            {
                var current = checkers.RandomElementOrDefault();

                fight.AddChallenge(current.BuildChallenge(fight));

                checkers.Remove(current);
            }
        }

        public void Save()
        {
        }
    }
}
