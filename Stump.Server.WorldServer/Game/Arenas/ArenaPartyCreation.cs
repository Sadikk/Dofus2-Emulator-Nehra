using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Parties;
using System;
using System.Collections.Generic;

namespace Stump.Server.WorldServer.Game.Arenas
{
    public class ArenaPartyCreation
    {
        // FIELDS
        private readonly object m_lock = new object();

        private ArenaParty m_party;
        private List<Character> m_characters;

        // PROPERTIES

        // CONSTRUCTORS
        public ArenaPartyCreation(ArenaParty party)
        {
            this.m_characters = new List<Character>();
            this.m_party = party;
        }
        public ArenaPartyCreation(ArenaParty party, params Character[] characters)
        {
            this.m_characters = new List<Character>();
            this.m_characters.AddRange(characters);
            this.m_party = party;
        }
        public ArenaPartyCreation(params Character[] characters)
        {
            this.m_characters = new List<Character>();
            this.m_characters.AddRange(characters);
            this.m_party = null;
        }

        // METHODS
        public void Foreach(Action<Character> action)
        {
            lock (this.m_lock)
            {
                foreach (var item in this.m_characters)
                {
                    action(item);
                }
            }
        }

        public void SendFightProposition(ArenaInvitation invitation)
        {
            this.Foreach(entry =>
{
    //entry.SetArenaInvitation(invitation);

    
});
        }
    }
}
