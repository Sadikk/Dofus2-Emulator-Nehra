using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Messages;
using Stump.Server.BaseServer.Network;
using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Game;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Parties;
using System.Linq;

namespace Stump.Server.WorldServer.Handlers.Context.RolePlay.Party
{
	public class PartyHandler : WorldHandlerContainer
	{
        private PartyHandler() { }

        [WorldHandler(PartyInvitationRequestMessage.Id)]
		public static void HandlePartyInvitationRequestMessage(WorldClient client, PartyInvitationRequestMessage message)
		{
			Character character = Singleton<World>.Instance.GetCharacter(message.name);
			if (character == null)
			{
				PartyHandler.SendPartyCannotJoinErrorMessage(client, PartyJoinErrorEnum.PARTY_JOIN_ERROR_PLAYER_NOT_FOUND);
			}
			else
			{
				if (character.IsAway)
				{
					PartyHandler.SendPartyCannotJoinErrorMessage(client, PartyJoinErrorEnum.PARTY_JOIN_ERROR_PLAYER_BUSY);
				}
				else
				{
					client.Character.Invite(character);
				}
			}
		}
        [WorldHandler(PartyInvitationDetailsRequestMessage.Id)]
		public static void HandlePartyInvitationDetailsRequestMessage(WorldClient client, PartyInvitationDetailsRequestMessage message)
		{
			PartyInvitation invitation = client.Character.GetInvitation(message.partyId);
			if (invitation != null)
			{
				PartyHandler.SendPartyInvitationDetailsMessage(client, invitation);
			}
		}
        [WorldHandler(PartyAcceptInvitationMessage.Id)]
		public static void HandlePartyAcceptInvitationMessage(WorldClient client, PartyAcceptInvitationMessage message)
		{
			PartyInvitation invitation = client.Character.GetInvitation(message.partyId);
			if (invitation != null)
			{
				invitation.Accept();
			}
		}
        [WorldHandler(PartyRefuseInvitationMessage.Id)]
		public static void HandlePartyRefuseInvitationMessage(WorldClient client, PartyRefuseInvitationMessage message)
		{
			PartyInvitation invitation = client.Character.GetInvitation(message.partyId);
			if (invitation != null)
			{
				invitation.Deny();
			}
		}
        [WorldHandler(PartyCancelInvitationMessage.Id)]
		public static void HandlePartyCancelInvitationMessage(WorldClient client, PartyCancelInvitationMessage message)
		{
			if (client.Character.IsInParty())
			{
				Character guest = client.Character.Party.GetGuest(message.guestId);
				if (guest != null)
				{
					PartyInvitation invitation = guest.GetInvitation(client.Character.Party.Id);
					if (invitation != null)
					{
						invitation.Cancel();
					}
				}
			}
		}
        [WorldHandler(PartyLeaveRequestMessage.Id)]
		public static void HandlePartyLeaveRequestMessage(WorldClient client, PartyLeaveRequestMessage message)
		{
			if (client.Character.IsInParty())
			{
				client.Character.LeaveParty();
			}
		}
        [WorldHandler(PartyAbdicateThroneMessage.Id)]
		public static void HandlePartyAbdicateThroneMessage(WorldClient client, PartyAbdicateThroneMessage message)
		{
			if (client.Character.IsPartyLeader())
			{
				Character member = client.Character.Party.GetMember(message.playerId);
				client.Character.Party.ChangeLeader(member);
			}
		}
        [WorldHandler(PartyKickRequestMessage.Id)]
		public static void HandlePartyKickRequestMessage(WorldClient client, PartyKickRequestMessage message)
		{
			if (client.Character.IsPartyLeader())
			{
				Character member = client.Character.Party.GetMember(message.playerId);
				client.Character.Party.Kick(member);
			}
		}

		public static void SendPartyKickedByMessage(IPacketReceiver client, Game.Parties.Party party, Character kicker)
		{
            client.Send(new PartyKickedByMessage((uint)party.Id, (uint)kicker.Id));
		}
        public static void SendPartyLeaderUpdateMessage(IPacketReceiver client, Game.Parties.Party party, Character leader)
		{
            client.Send(new PartyLeaderUpdateMessage((uint)party.Id, (uint)leader.Id));
		}
        public static void SendPartyRestrictedMessage(IPacketReceiver client, Game.Parties.Party party, bool restricted)
		{
            client.Send(new PartyRestrictedMessage((uint)party.Id, restricted));
		}
        public static void SendPartyUpdateMessage(IPacketReceiver client, Game.Parties.Party party, Character member)
		{
            client.Send(new PartyUpdateMessage((uint)party.Id, member.GetPartyMemberInformations()));
		}
        public static void SendPartyNewGuestMessage(IPacketReceiver client, Game.Parties.Party party, Character guest)
		{
            client.Send(new PartyNewGuestMessage((uint)party.Id, guest.GetPartyGuestInformations(party)));
		}
        public static void SendPartyMemberRemoveMessage(IPacketReceiver client, Game.Parties.Party party, Character leaver)
		{
            client.Send(new PartyMemberRemoveMessage((uint)party.Id, (uint)leaver.Id));
		}
		public static void SendPartyInvitationCancelledForGuestMessage(IPacketReceiver client, Character canceller, PartyInvitation invitation)
		{
            client.Send(new PartyInvitationCancelledForGuestMessage((uint)invitation.Party.Id, (uint)canceller.Id));
		}
		public static void SendPartyCancelInvitationNotificationMessage(IPacketReceiver client, PartyInvitation invitation)
		{
            client.Send(new PartyCancelInvitationNotificationMessage((uint)invitation.Party.Id, (uint)invitation.Source.Id, (uint)invitation.Target.Id));
		}
		public static void SendPartyRefuseInvitationNotificationMessage(IPacketReceiver client, PartyInvitation invitation)
		{
            client.Send(new PartyRefuseInvitationNotificationMessage((uint)invitation.Party.Id, (uint)invitation.Target.Id));
		}
        public static void SendPartyDeletedMessage(IPacketReceiver client, Game.Parties.Party party)
		{
            client.Send(new PartyDeletedMessage((uint)party.Id));
		}
        public static void SendPartyJoinMessage(IPacketReceiver client, Game.Parties.Party party)
		{
            client.Send(new PartyJoinMessage((uint)party.Id, (sbyte)party.Type, (uint)party.Leader.Id, 8, 
				from entry in party.Members
				select entry.GetPartyMemberInformations(), party.Guests.Select((Character entry) => entry.GetPartyGuestInformations(party)), party.Restricted, "partyzizi"));
		}
        public static void SendPartyInvitationMessage(WorldClient client, Game.Parties.Party party, Character from)
		{
            client.Send(new PartyInvitationMessage((uint)party.Id, (sbyte)party.Type, "partyzizi", 8, (uint)from.Id, from.Name, (uint)client.Character.Id));
		}
		public static void SendPartyInvitationDetailsMessage(IPacketReceiver client, PartyInvitation invitation)
		{
            client.Send(new PartyInvitationDetailsMessage((uint)invitation.Party.Id, (sbyte)invitation.Party.Type, "partyzizi", (uint)invitation.Source.Id, invitation.Source.Name, (uint)invitation.Party.Leader.Id, 
				from entry in invitation.Party.Members
				select entry.GetPartyInvitationMemberInformations(), invitation.Party.Guests.Select((Character entry) => entry.GetPartyGuestInformations(invitation.Party))));
		}
        public static void SendPartyCannotJoinErrorMessage(IPacketReceiver client, Game.Parties.Party party, PartyJoinErrorEnum reason)
		{
            client.Send(new PartyCannotJoinErrorMessage((uint)party.Id, (sbyte)reason));
		}
		public static void SendPartyCannotJoinErrorMessage(IPacketReceiver client, PartyJoinErrorEnum reason)
		{
			client.Send(new PartyCannotJoinErrorMessage(0, (sbyte)reason));
		}
	}
}
