using Stump.Core.Collections;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Messages;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Handlers.Context.RolePlay.Party;
using System;
using System.Linq;

namespace Stump.Server.WorldServer.Game.Parties
{
	public abstract class Party
	{
		public delegate void MemberAddedHandler(Party party, Character member);
		public delegate void MemberRemovedHandler(Party party, Character member, bool kicked);

		private readonly ConcurrentList<Character> m_members = new ConcurrentList<Character>();
		private readonly ConcurrentList<Character> m_guests = new ConcurrentList<Character>();
		private readonly object m_guestLocker = new object();
		private readonly object m_memberLocker = new object();
		private bool m_restricted;

		public event Action<Party, Character> LeaderChanged;
		public event Party.MemberAddedHandler GuestAdded;
		public event Party.MemberRemovedHandler GuestRemoved;
		public event Action<Party, Character> GuestPromoted;
		public event Party.MemberRemovedHandler MemberRemoved;
		public event System.Action<Party> PartyDeleted;

		public int Id
		{
			get;
			private set;
		}
        public abstract PartyTypeEnum Type
        {
            get;
        }
        public abstract short MaxMemberCount
        {
            get;
        }
		public bool Restricted
		{
			get
			{
				return this.m_restricted;
			}
			private set
			{
				this.m_restricted = value;
				this.ForEach(delegate(Character entry)
				{
					PartyHandler.SendPartyRestrictedMessage(entry.Client, this, this.m_restricted);
				});
			}
		}
		public bool IsFull
		{
			get
			{
				return this.m_members.Count >= 8;
			}
		}
		public int GroupLevel
		{
			get
			{
				return this.m_members.Sum((Character entry) => (int)entry.Level);
			}
		}
		public int GroupProspecting
		{
			get
			{
				return this.m_members.Sum((Character entry) => entry.Stats[PlayerFields.Prospecting].Total);
			}
		}
		public int MembersCount
		{
			get
			{
				return this.m_members.Count;
			}
		}
		public System.Collections.Generic.IEnumerable<Character> Members
		{
			get
			{
				return this.m_members;
			}
		}
		public System.Collections.Generic.IEnumerable<Character> Guests
		{
			get
			{
				return this.m_guests;
			}
		}
		public Character Leader
		{
			get;
			private set;
		}
		public bool Disbanded
		{
			get;
			private set;
		}

		protected virtual void OnLeaderChanged(Character leader)
		{
			this.ForEach(delegate(Character entry)
			{
				PartyHandler.SendPartyLeaderUpdateMessage(entry.Client, this, leader);
			});
			Action<Party, Character> leaderChanged = this.LeaderChanged;
			if (leaderChanged != null)
			{
				leaderChanged(this, leader);
			}
		}
		protected virtual void OnGuestAdded(Character groupGuest)
		{
			this.ForEach(delegate(Character entry)
			{
				PartyHandler.SendPartyNewGuestMessage(entry.Client, this, groupGuest);
			});
			Party.MemberAddedHandler guestAdded = this.GuestAdded;
			if (guestAdded != null)
			{
				guestAdded(this, groupGuest);
			}
		}
		protected virtual void OnGuestRemoved(Character groupGuest, bool kicked)
		{
			Party.MemberRemovedHandler guestRemoved = this.GuestRemoved;
			if (guestRemoved != null)
			{
				guestRemoved(this, groupGuest, kicked);
			}
		}
		protected virtual void OnGuestPromoted(Character groupMember)
		{
			PartyHandler.SendPartyJoinMessage(groupMember.Client, this);
			this.UpdateMember(groupMember);
			this.BindEvents(groupMember);
			Action<Party, Character> guestPromoted = this.GuestPromoted;
			if (guestPromoted != null)
			{
				guestPromoted(this, groupMember);
			}
		}
		protected virtual void OnMemberRemoved(Character groupMember, bool kicked)
		{
			if (kicked)
			{
				PartyHandler.SendPartyKickedByMessage(groupMember.Client, this, this.Leader);
			}
			else
			{
				groupMember.Client.Send(new PartyLeaveMessage((uint)this.Id));
			}
			this.ForEach(delegate(Character entry)
			{
				PartyHandler.SendPartyMemberRemoveMessage(entry.Client, this, groupMember);
			});
			Party.MemberRemovedHandler memberRemoved = this.MemberRemoved;
			this.UnBindEvents(groupMember);
			if (memberRemoved != null)
			{
				memberRemoved(this, groupMember, kicked);
			}
		}
		protected virtual void OnGroupDisbanded()
		{
			this.ForEach(delegate(Character entry)
			{
				PartyHandler.SendPartyDeletedMessage(entry.Client, this);
			});
			this.UnBindEvents();
			System.Action<Party> partyDeleted = this.PartyDeleted;
			if (partyDeleted != null)
			{
				partyDeleted(this);
			}
		}

		internal Party(int id, Character leader)
		{
			this.Id = id;
			this.Restricted = true;
			this.m_members.Add(leader);
			this.BindEvents(leader);
			this.Leader = leader;
			PartyHandler.SendPartyJoinMessage(leader.Client, this);
		}
		public bool CanInvite(Character character)
		{
			return !this.IsMember(character) && !this.IsGuest(character);
		}
		public bool AddGuest(Character character)
		{
			bool result;
			if (this.IsFull || !this.CanInvite(character))
			{
				result = false;
			}
			else
			{
				lock (this.m_guestLocker)
				{
					this.m_guests.Add(character);
				}
				this.OnGuestAdded(character);
				result = true;
			}
			return result;
		}
		public void RemoveGuest(Character character)
		{
			lock (this.m_guestLocker)
			{
				if (this.m_guests.Remove(character))
				{
					this.OnGuestRemoved(character, false);
					if (this.MembersCount <= 1)
					{
						this.Disband();
					}
				}
			}
		}
		public bool PromoteGuestToMember(Character guest)
		{
			bool result;
			if (this.IsMember(guest))
			{
				result = false;
			}
			else
			{
				if (!this.IsGuest(guest) && !this.AddGuest(guest))
				{
					result = false;
				}
				else
				{
					lock (this.m_guestLocker)
					{
						this.m_guests.Remove(guest);
					}
					lock (this.m_memberLocker)
					{
						this.m_members.Add(guest);
					}
					this.OnGuestPromoted(guest);
					result = true;
				}
			}
			return result;
		}
		public bool AddMember(Character member)
		{
			return this.PromoteGuestToMember(member);
		}
		public void RemoveMember(Character character)
		{
			lock (this.m_memberLocker)
			{
				if (this.m_members.Remove(character))
				{
					this.OnMemberRemoved(character, false);
					if (this.MembersCount <= 1)
					{
						this.Disband();
					}
					else
					{
						if (this.Leader == character)
						{
							this.ChangeLeader(this.m_members.First<Character>());
						}
					}
				}
			}
		}
		public void Kick(Character member)
		{
			lock (this.m_memberLocker)
			{
				if (this.m_members.Remove(member))
				{
					this.OnMemberRemoved(member, true);
					if (this.MembersCount <= 1)
					{
						this.Disband();
					}
					else
					{
						if (this.Leader == member)
						{
							this.ChangeLeader(this.m_members.First<Character>());
						}
					}
				}
			}
		}
		public void ChangeLeader(Character leader)
		{
			if (this.IsInGroup(leader) && this.Leader != leader)
			{
				this.Leader = leader;
				this.OnLeaderChanged(this.Leader);
			}
		}

		public bool IsInGroup(Character character)
		{
			return this.IsMember(character) || this.IsGuest(character);
		}
		public bool IsMember(Character character)
		{
			return this.m_members.Contains(character);
		}
		public bool IsGuest(Character character)
		{
			return this.m_guests.Contains(character);
		}

		public void Disband()
		{
			if (!this.Disbanded)
			{
				this.Disbanded = true;
				Singleton<PartyManager>.Instance.Remove(this);
				this.OnGroupDisbanded();
			}
		}

		public Character GetMember(int id)
		{
			return this.m_members.SingleOrDefault((Character entry) => entry.Id == id);
		}
        public Character GetMember(uint id)
        {
            return this.m_members.SingleOrDefault((Character entry) => entry.Id == (int)id);
        }
		public Character GetGuest(int id)
		{
			return this.m_guests.SingleOrDefault((Character entry) => entry.Id == id);
		}
        public Character GetGuest(uint id)
        {
            return this.m_guests.SingleOrDefault((Character entry) => entry.Id == (int)id);
        }

		public void UpdateMember(Character character)
		{
			if (this.IsInGroup(character))
			{
				this.ForEach(delegate(Character entry)
				{
					PartyHandler.SendPartyUpdateMessage(entry.Client, this, character);
				});
			}
		}
		public void ForEach(System.Action<Character> action)
		{
			lock (this.m_memberLocker)
			{
				foreach (Character current in this.Members)
				{
					action(current);
				}
			}
		}
		public void ForEach(System.Action<Character> action, Character except)
		{
			lock (this.m_memberLocker)
			{
				foreach (Character current in this.Members)
				{
					if (current != except)
					{
						action(current);
					}
				}
			}
		}
		public void SendToAll(Message message)
		{
			lock (this.m_memberLocker)
			{
				foreach (Character current in this.m_members)
				{
					current.Client.Send(message);
				}
			}
		}
		private void OnLifeUpdated(Character character, int regainedLife)
		{
			this.UpdateMember(character);
		}
		private void OnLevelChanged(Character character, byte currentLevel, int difference)
		{
			this.UpdateMember(character);
		}
		private void BindEvents(Character member)
		{
			member.LifeRegened += new Action<Character, int>(this.OnLifeUpdated);
			member.LevelChanged += new Character.LevelChangedHandler(this.OnLevelChanged);
		}
		private void UnBindEvents(Character member)
		{
			member.LifeRegened -= new Action<Character, int>(this.OnLifeUpdated);
			member.LevelChanged -= new Character.LevelChangedHandler(this.OnLevelChanged);
		}
		private void UnBindEvents()
		{
			foreach (Character current in this.Members)
			{
				this.UnBindEvents(current);
			}
		}
	}
}
