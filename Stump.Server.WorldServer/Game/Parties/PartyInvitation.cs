using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Notifications;
using Stump.Server.WorldServer.Handlers.Context.RolePlay.Party;

namespace Stump.Server.WorldServer.Game.Parties
{
	public class PartyInvitation : Notification
	{
		public Party Party
		{
			get;
			private set;
		}
		public Character Source
		{
			get;
			private set;
		}
		public Character Target
		{
			get;
			private set;
		}
		public PartyInvitation(Party party, Character source, Character target)
		{
			this.Party = party;
			this.Source = source;
			this.Target = target;
		}
		public void Accept()
		{
			this.Target.EnterParty(this.Party);
		}
		public void Deny()
		{
			this.Target.RemoveInvitation(this);
			this.Party.RemoveGuest(this.Target);
			PartyHandler.SendPartyInvitationCancelledForGuestMessage(this.Target.Client, this.Target, this);
			this.Party.ForEach(delegate(Character entry)
			{
				PartyHandler.SendPartyRefuseInvitationNotificationMessage(entry.Client, this);
			});
		}
		public void Cancel()
		{
			this.Target.RemoveInvitation(this);
			this.Party.RemoveGuest(this.Target);
			PartyHandler.SendPartyInvitationCancelledForGuestMessage(this.Target.Client, this.Source, this);
			this.Party.ForEach(delegate(Character entry)
			{
				PartyHandler.SendPartyCancelInvitationNotificationMessage(entry.Client, this);
			});
		}
		public override void Display()
		{
			PartyHandler.SendPartyInvitationMessage(this.Target.Client, this.Party, this.Source);
		}
	}
}
