using Game.Dialogs.Alliances;
using Stump.Core.Extensions;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Messages;
using Stump.DofusProtocol.Types;
using Stump.Server.BaseServer.Network;
using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Game.Alliances;
using Stump.Server.WorldServer.Game.Guilds;
using System.Linq;

namespace Stump.Server.WorldServer.Handlers.Alliances
{
    public class AllianceHandler : WorldHandlerContainer
	{
        private AllianceHandler() { }

        [WorldHandler(AllianceCreationValidMessage.Id)]
        public static void HandleAllianceCreationValidMessage(WorldClient client, AllianceCreationValidMessage message)
        {
            var allianceCreationPanel = client.Character.Dialog as AllianceCreationPanel;
            if (allianceCreationPanel != null)
            {
                allianceCreationPanel.CreateAlliance(message.allianceName, message.allianceTag, message.allianceEmblem);
            }
        }
        [WorldHandler(AllianceInsiderInfoRequestMessage.Id)]
        public static void HandleAllianceInsiderInfoRequestMessage(WorldClient client, AllianceInsiderInfoRequestMessage message)
        {
            if (client.Character.Guild != null)
            {
                if (client.Character.Guild.Alliance != null)
                {
                    AllianceHandler.SendAllianceInsiderInfoMessage(client, client.Character.Guild.Alliance);
                }
            }
        }
        [WorldHandler(GuildFactsRequestMessage.Id)]
        public static void HandleGuildFactsRequestMessage(WorldClient client, GuildFactsRequestMessage message)
        {
            var guild = Singleton<GuildManager>.Instance.TryGetGuild((int)message.guildId);
            if (guild != null)
            {
                AllianceHandler.SendGuildFactsMessage(client, guild);
            }
        }
        [WorldHandler(AllianceFactsRequestMessage.Id)]
        public static void HandleAllianceFactsRequestMessage(WorldClient client, AllianceFactsRequestMessage message)
        {
            var alliance = Singleton<AllianceManager>.Instance.TryGetAlliance((int)message.allianceId);
            if (alliance != null)
            {
                AllianceHandler.SendAllianceFactsMessage(client, alliance);
            }
        }
        [WorldHandler(SetEnableAVARequestMessage.Id)]
        public static void HandleSetEnableAVARequestMessage(WorldClient client, SetEnableAVARequestMessage message)
        {
        }

        public static void SendAllianceCreationStartedMessage(IPacketReceiver client)
        {
            client.Send(new AllianceCreationStartedMessage());
        }
        public static void SendAllianceCreationResultMessage(IPacketReceiver client, SocialGroupCreationResultEnum result)
        {
            client.Send(new AllianceCreationResultMessage((sbyte)result));
        }
        public static void SendAllianceInsiderInfoMessage(IPacketReceiver client, Alliance alliance)
        {
            client.Send(new AllianceInsiderInfoMessage(
                alliance.GetAllianceFactSheetInformations(),
                alliance.GetGuildsInformations(),
                alliance.GetPrismsInformations()));
        }
        public static void SendGuildFactsMessage(IPacketReceiver client, Guild guild)
        {
            client.Send(new GuildFactsMessage(
                new GuildFactSheetInformations((uint)guild.Id, guild.Name, guild.Emblem.GetNetworkGuildEmblem(), (uint)guild.Boss.Id, guild.Level, (ushort)guild.Members.Count),
                guild.Record.CreationDate.GetUnixTimeStamp(),
                (ushort)guild.TaxCollectors.Count,
                false, 
                (from x in guild.Members
                 select new CharacterMinimalInformations((uint)x.Id, x.Character.Level, x.Character.Name))));
        }
        public static void SendAllianceJoinedMessage(IPacketReceiver client, Alliance alliance)
        {
            client.Send(new AllianceJoinedMessage(
                new AllianceInformations((uint)alliance.Id, alliance.Tag, alliance.Name, alliance.Emblem.GetNetworkGuildEmblem()),
                false));
        }
        public static void SendAllianceMembershipMessage(IPacketReceiver client, Alliance alliance)
        {
            client.Send(new AllianceMembershipMessage(alliance.GetAllianceInformations(), false));
        }
        public static void SendAllianceFactsMessage(IPacketReceiver client, Alliance alliance)
        {
            client.Send(new AllianceFactsMessage(
                alliance.GetAllianceFactSheetInformations(),
                alliance.GetGuildsInAllianceInformations(),
                new ushort[0],
                (uint)alliance.Boss.Boss.Id,
                alliance.Boss.Boss.Name));
        }
    }
}
