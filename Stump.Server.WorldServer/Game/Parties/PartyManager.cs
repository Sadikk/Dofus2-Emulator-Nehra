using Stump.Core.Pool;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Parties
{
	public class PartyManager : EntityManager<PartyManager, Party>
	{
		private readonly UniqueIdProvider m_idProvider = new UniqueIdProvider();
        private readonly object m_lock = new object();

        public ClassicalParty CreateClassicalParty(Character leader)
        {
            ClassicalParty party;
            lock (this.m_lock)
            {
                party = new ClassicalParty(this.m_idProvider.Pop(), leader);
            }

            base.AddEntity(party.Id, party);
            return party;
        }
        public ArenaParty CreateArenaParty(Character leader)
        {
            ArenaParty party;
            lock (this.m_lock)
            {
                party = new ArenaParty(this.m_idProvider.Pop(), leader);
            }

            base.AddEntity(party.Id, party);
            return party;
        }

		public void Remove(Party party)
		{
			base.RemoveEntity(party.Id);
			this.m_idProvider.Push(party.Id);
		}

		public Party GetParty(int id)
		{
			return base.GetEntityOrDefault(id);
		}
        public T GetParty<T>(int id)
            where T : Party
        {
            var party = this.GetParty(id);
            if (party is T)
            {
                return (T)party;
            }

            return default(T);
        }
	}
}
