using NLog;
using Stump.Core.Attributes;
using Stump.Server.BaseServer.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stump.Core.Threading;
using Stump.Server.BaseServer.Initialization;
using Stump.Core.Collections;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Game.Parties;
using Stump.Server.WorldServer.Database.Elo;

namespace Stump.Server.WorldServer.Game.Arenas
{
    
    public class ArenaManager : DataManager<ArenaManager>
    {
        // FIELDS
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();

        [Variable]
        public static byte MinimumLevelArena = 50;
        [Variable]
        private static int QueueRefresherElapsedTime = 10000;

        private static Task m_queueRefresherTask;

        private readonly object m_lock = new object();

        private Dictionary<short, EloRecord> m_elos;

        private readonly ConcurrentList<Character> m_queue;
        private readonly ConcurrentList<ArenaParty> m_partyQueue;

        private readonly AsyncRandom m_asyncRandom;

        // PROPERTIES
        public object SyncRoot { get { return this.m_lock; } }

        // CONSTRUCTORS
        private ArenaManager()
        {
            this.m_queue = new ConcurrentList<Character>();
            this.m_partyQueue = new ConcurrentList<ArenaParty>();

            this.m_asyncRandom = new AsyncRandom();
        }

        // METHODS
        [Initialization(InitializationPass.Last)]
        public override void Initialize()
        {
            base.Initialize();

            this.m_elos = base.Database.Query<EloRecord>(EloRelator.FetchQuery, new object[0])
                .ToDictionary(entry => entry.Difference);

            //ArenaManager.m_queueRefresherTask = Task.Factory.StartNewDelayed(ArenaManager.QueueRefresherElapsedTime, new Action(Singleton<ArenaManager>.Instance.TryCreateTeams));
        }

        public bool CanArena(WorldClient client)
        {
            bool result;
            if (client.Character.Level < ArenaManager.MinimumLevelArena)
            {
                // TODO : message
                result = false;
            }
            else
            {
                if (false) // TODO : Banned from queue
                {
                    // TODO : message
                    result = false;
                }
                else
                {
                    result = true;
                }
            }

            return result;
        }
        private bool CanArena(WorldClient client, ArenaParty party)
        {
            bool result;

            result = true; // TODO

            return result;
        }

        public bool IsInQueue(WorldClient client)
        {
            return false;
        }
        public bool IsInQueue(ArenaParty party)
        {
            return false;
        }

        public void Register(WorldClient client)
        {

        }

        public void Unregister(WorldClient client)
        { 
        }

        /// <summary>
        /// Try to get 2 players with approximatly the same hidden rank.
        /// If these 2 players are found, they are removed from the list.
        /// </summary>
        /// <param name="characters">An ordered by descending with the hidden arena rank</param>
        /// <param name="redOpponent"></param>
        /// <param name="blueOpponent"></param>
        /// <returns></returns>
        private bool CanGetBalancedOpponents(ref List<Character> characters, ref Character redOpponent, ref Character blueOpponent)
        {
            var result = false;
            var i = 0;

            while (i < characters.Count - 1 && result == false)
            {
                result = Math.Abs(characters[i].Record.HiddenArenaRank - characters[i + 1].Record.HiddenArenaRank) < 100;
                if (result)
                {
                    redOpponent = characters[i];
                    blueOpponent = characters[i + 1];

                    characters.Remove(redOpponent);
                    characters.Remove(blueOpponent);
                }

                i++;
            }

            return result;
        }
        /// <summary>
        /// Check if the fight between p3 and p2 can be balanced. If is true, the method calculate the best desired rank for the last player.
        /// </summary>
        /// <param name="p3"></param>
        /// <param name="p2"></param>
        /// <param name="bestRank"></param>
        /// <returns></returns>
        private bool CanBeBalanced(ArenaParty p3, ArenaParty p2, ref int bestRank)
        {
            bool result;

            var orderedP3 = p3.Members.OrderByDescending(entry => entry.Record.HiddenArenaRank);
            var orderedP2 = p2.Members.OrderByDescending(entry => entry.Record.HiddenArenaRank);

            result = false; // TODO

            return result;
        }

        private ArenaPartyCreation TryCreateBalancedTeam(ArenaParty party, ref List<ArenaParty> p2, ref List<Character> characters)
        {
            var i = 0;
            var searchedRank = -1;

            ArenaPartyCreation creation;
            while (p2.Count > i && !this.CanBeBalanced(party, p2[i], ref searchedRank)) { ++i; }

            if (i < p2.Count)
            {
                var character = characters.FirstOrDefault(entry => Math.Abs(entry.Record.HiddenArenaRank - searchedRank) < 150);
                if (character == null)
                {
                    creation = null;
                }
                else
                {
                    creation = new ArenaPartyCreation(p2[i], character);

                    p2.Remove(p2[i]);
                    characters.Remove(character);
                }
            }
            else
            {
                creation = null;
            }

            return creation;
        }

        private double GetProbability(short diff)
        {
            var result = 0.5d;

            var pair = this.m_elos.FirstOrDefault(entry => entry.Key > diff);
            if (!pair.Equals(default(KeyValuePair<short, EloRecord>)))
            {
                result = pair.Value.Probability;
            }

            return result;
        }

        private void TryCreateTeams()
        {
            try
            {
                var time = DateTime.Now;

                Character red = null, blue = null;

                List<ArenaParty> p3;
                List<ArenaParty> p2;
                List<Character>  p1;

                #region Initializing

                lock (this.m_lock)
                {
                    p3 = this.m_partyQueue.Where(entry => entry.MembersCount == 3 && !entry.IsTag()).ToList();
                    p2 = this.m_partyQueue.Where(entry => entry.MembersCount == 2 && !entry.IsTag()).ToList();
                    p1 = this.m_queue.Where(entry => entry.ArenaInvitation == null).ToList().OrderByDescending(entry => entry.Record.HiddenArenaRank).ToList();
                }

                #endregion

                #region Handle P3

                while(p3.Count > 0)
                {
                    var current = p3[0];
                    p3.Remove(current);

                    var opponent = p3.FirstOrDefault(entry => Math.Abs(this.GetProbability((short)((entry.GetHiddenRankSum() - current.GetHiddenRankSum()) / 3)) - 0.5) < 1.0);
                    if (opponent == null)
                    {
                        var party = this.TryCreateBalancedTeam(current, ref p2, ref p1);
                        if (party != null)
                        {
                            var invitation = new ArenaInvitation(new ArenaPartyCreation(current), party);
                        }
                    }
                    else
                    {
                        p3.Remove(opponent);

                        var invitation = new ArenaInvitation(new ArenaPartyCreation(current), new ArenaPartyCreation(opponent));
                    }
                }
                #endregion

                #region Handle P2

                while (p2.Count > 0)
                {
                    var current = p2[0];
                    p2.Remove(current);

                    var opponent = p2.FirstOrDefault(entry => Math.Abs(this.GetProbability((short)((entry.GetHiddenRankSum() - current.GetHiddenRankSum()) / 2)) - 0.5) < 1.0);
                    if (opponent == null)
                    {
                        // TODO
                    }
                    else
                    {
                        if (this.CanGetBalancedOpponents(ref p1, ref red, ref blue))
                        {
                            p2.Remove(opponent);

                            var invitation = new ArenaInvitation(new ArenaPartyCreation(current, red), new ArenaPartyCreation(opponent, blue));
                        }
                    }
                }

                #endregion

                #region Handle P1

                var possibleCombinaisons = new List<KeyValuePair<Character, Character>>();
                while (this.CanGetBalancedOpponents(ref p1, ref red, ref blue))
                {
                    possibleCombinaisons.Add(new KeyValuePair<Character, Character>(red, blue));
                }

                for (int i = 0; i < possibleCombinaisons.Count / 3; i++)
                {
                    var redTeam = new Character[3];
                    var blueTeam = new Character[3];

                    for (int j = 0; j < 3; j++)
                    {
                        var pair = possibleCombinaisons[i * 3 + j];

                        redTeam[j] = pair.Key;
                        blueTeam[j] = pair.Value;
                    }

                    var invitation = new ArenaInvitation(new ArenaPartyCreation(redTeam), new ArenaPartyCreation(blueTeam));
                }

                #endregion
                
                this.logger.Debug(string.Format("TryCreateTeams() executed : {0} ms", (DateTime.Now - time).Milliseconds));
            }
            finally
            {
                ArenaManager.m_queueRefresherTask = Task.Factory.StartNewDelayed(ArenaManager.QueueRefresherElapsedTime, new Action(this.TryCreateTeams));
            }
        }
    }
}
