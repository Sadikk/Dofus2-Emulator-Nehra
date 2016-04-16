using Stump.Core.Extensions;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Messages;
using Stump.DofusProtocol.Types;
using Stump.Server.BaseServer.Network;
using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Game;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Dialogs.Guilds;
using Stump.Server.WorldServer.Game.Guilds;
using Stump.Server.WorldServer.Handlers.TaxCollector;
using System.Linq;
namespace Stump.Server.WorldServer.Handlers.Guilds
{
	public class GuildHandler : WorldHandlerContainer
	{
        private GuildHandler() { }

        [WorldHandler(GuildGetInformationsMessage.Id)]
		public static void HandleGuildGetInformationsMessage(WorldClient client, GuildGetInformationsMessage message)
		{
			if (client.Character.Guild != null)
			{
				switch (message.infoType)
				{
				case 1:
					GuildHandler.SendGuildInformationsGeneralMessage(client, client.Character.Guild);
					break;
				case 2:
					GuildHandler.SendGuildInformationsMembersMessage(client, client.Character.Guild);
					break;
				case 3:
					GuildHandler.SendGuildInfosUpgradeMessage(client, client.Character.Guild);
					break;
				case 4:
					GuildHandler.SendGuildInformationsPaddocksMessage(client);
					break;
				case 5:
					GuildHandler.SendGuildHousesInformationMessage(client);
					break;
				case 6:
					TaxCollectorHandler.SendTaxCollectorListMessage(client, client.Character.Guild);
					break;
				}
			}
		}
        [WorldHandler(GuildCharacsUpgradeRequestMessage.Id)]
		public static void HandleGuildCharacsUpgradeRequestMessage(WorldClient client, GuildCharacsUpgradeRequestMessage message)
		{
			if (client.Character.Guild != null && client.Character.GuildMember.HasRight(GuildRightsBitEnum.GUILD_RIGHT_MANAGE_GUILD_BOOSTS))
			{
				switch (message.charaTypeTarget)
				{
				case 0:
					client.Character.Guild.UpgradeTaxCollectorPods();
					break;
				case 1:
					client.Character.Guild.UpgradeTaxCollectorProspecting();
					break;
				case 2:
					client.Character.Guild.UpgradeTaxCollectorWisdom();
					break;
				case 3:
					client.Character.Guild.UpgradeMaxTaxCollectors();
					break;
				}
				GuildHandler.SendGuildInfosUpgradeMessage(client.Character.Guild.Clients, client.Character.Guild);
			}
		}
        [WorldHandler(GuildSpellUpgradeRequestMessage.Id)]
		public static void HandleGuildSpellUpgradeRequestMessage(WorldClient client, GuildSpellUpgradeRequestMessage message)
		{
			if (client.Character.Guild != null && client.Character.Guild.UpgradeSpell(message.spellId))
			{
				GuildHandler.SendGuildInfosUpgradeMessage(client.Character.Guild.Clients, client.Character.Guild);
			}
		}
        [WorldHandler(GuildCreationValidMessage.Id)]
		public static void HandleGuildCreationValidMessage(WorldClient client, GuildCreationValidMessage message)
		{
			GuildCreationPanel guildCreationPanel = client.Character.Dialog as GuildCreationPanel;
			if (guildCreationPanel != null)
			{
				guildCreationPanel.CreateGuild(message.guildName, message.guildEmblem);
			}
		}
        [WorldHandler(GuildChangeMemberParametersMessage.Id)]
		public static void HandleGuildChangeMemberParametersMessage(WorldClient client, GuildChangeMemberParametersMessage message)
		{
			if (client.Character.Guild != null)
			{
				Stump.Server.WorldServer.Game.Guilds.GuildMember guildMember = client.Character.Guild.TryGetMember((int)message.memberId);
				if (guildMember != null)
				{
					client.Character.Guild.ChangeParameters(client.Character, guildMember,(short) message.rank, (byte)message.experienceGivenPercent, message.rights);
				}
			}
		}
        [WorldHandler(GuildKickRequestMessage.Id)]
		public static void HandleGuildKickRequestMessage(WorldClient client, GuildKickRequestMessage message)
		{
			if (client.Character.Guild != null)
			{
                Stump.Server.WorldServer.Game.Guilds.GuildMember guildMember = client.Character.Guild.TryGetMember((int)message.kickedId);
				if (guildMember != null)
				{
					guildMember.Guild.KickMember(client.Character, guildMember);
				}
			}
		}
        [WorldHandler(GuildInvitationMessage.Id)]
		public static void HandleGuildInvitationMessage(WorldClient client, GuildInvitationMessage message)
		{
			if (client.Character.Guild != null)
			{
				if (!client.Character.GuildMember.HasRight(GuildRightsBitEnum.GUILD_RIGHT_INVITE_NEW_MEMBERS))
				{
					client.Character.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 207, new object[0]);
				}
				else
				{
                    Character character = Singleton<World>.Instance.GetCharacter((int)message.targetId);
					if (character == null)
					{
						client.Character.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 208, new object[0]);
					}
					else
					{
						if (character.Guild != null)
						{
							client.Character.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 206, new object[0]);
						}
						else
						{
							if (character.IsBusy())
							{
								client.Character.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 209, new object[0]);
							}
							else
							{
								if (!client.Character.Guild.CanAddMember())
								{
									client.Character.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 55, new object[]
									{
										Guild.MaxMembersNumber
									});
								}
								else
								{
									GuildInvitationRequest guildInvitationRequest = new GuildInvitationRequest(client.Character, character);
									guildInvitationRequest.Open();
								}
							}
						}
					}
				}
			}
		}
        [WorldHandler(GuildInvitationByNameMessage.Id)]
		public static void HandleGuildInvitationByNameMessage(WorldClient client, GuildInvitationByNameMessage message)
		{
			if (client.Character.Guild != null)
			{
				if (!client.Character.GuildMember.HasRight(GuildRightsBitEnum.GUILD_RIGHT_INVITE_NEW_MEMBERS))
				{
					client.Character.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 207, new object[0]);
				}
				else
				{
					Character character = Singleton<World>.Instance.GetCharacter(message.name);
					if (character == null)
					{
						client.Character.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 208, new object[0]);
					}
					else
					{
						if (character.Guild != null)
						{
							client.Character.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 206, new object[0]);
						}
						else
						{
							if (character.IsBusy())
							{
								client.Character.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 209, new object[0]);
							}
							else
							{
								GuildInvitationRequest guildInvitationRequest = new GuildInvitationRequest(client.Character, character);
								guildInvitationRequest.Open();
							}
						}
					}
				}
			}
		}
        [WorldHandler(GuildInvitationAnswerMessage.Id)]
		public static void HandleGuildInvitationAnswerMessage(WorldClient client, GuildInvitationAnswerMessage message)
		{
			GuildInvitationRequest guildInvitationRequest = client.Character.RequestBox as GuildInvitationRequest;
			if (guildInvitationRequest != null)
			{
				if (client.Character == guildInvitationRequest.Source && !message.accept)
				{
					guildInvitationRequest.Cancel();
				}
				else
				{
					if (client.Character == guildInvitationRequest.Target)
					{
						if (message.accept)
						{
							guildInvitationRequest.Accept();
						}
						else
						{
							guildInvitationRequest.Deny();
						}
					}
				}
			}
		}
        [WorldHandler(GuildMemberSetWarnOnConnectionMessage.Id)]
		public static void HandleGuildMemberSetWarnOnConnectionMessage(WorldClient client, GuildMemberSetWarnOnConnectionMessage message)
		{
			client.Character.WarnOnGuildConnection = message.enable;
		}

		public static void SendGuildMemberWarnOnConnectionStateMessage(IPacketReceiver client, bool state)
		{
			client.Send(new GuildMemberWarnOnConnectionStateMessage(state));
		}
		public static void SendGuildInvitedMessage(IPacketReceiver client, Character recruter)
		{
            client.Send(new GuildInvitedMessage((uint)recruter.Id, recruter.Name, recruter.Guild.GetBasicGuildInformations()));
		}
		public static void SendGuildInvitationStateRecrutedMessage(IPacketReceiver client, GuildInvitationStateEnum state)
		{
			client.Send(new GuildInvitationStateRecrutedMessage((sbyte)state));
		}
		public static void SendGuildInvitationStateRecruterMessage(IPacketReceiver client, Character recruted, GuildInvitationStateEnum state)
		{
			client.Send(new GuildInvitationStateRecruterMessage(recruted.Name, (sbyte)state));
		}
		public static void SendGuildLeftMessage(IPacketReceiver client)
		{
			client.Send(new GuildLeftMessage());
		}
		public static void SendGuildCreationResultMessage(IPacketReceiver client, GuildCreationResultEnum result)
		{
			client.Send(new GuildCreationResultMessage((sbyte)result));
		}
		public static void SendGuildMembershipMessage(IPacketReceiver client, Stump.Server.WorldServer.Game.Guilds.GuildMember member)
		{
			client.Send(new GuildMembershipMessage(member.Guild.GetGuildInformations(), (uint)member.Rights, true));
		}
		public static void SendGuildInformationsGeneralMessage(IPacketReceiver client, Guild guild)
		{
            client.Send(new GuildInformationsGeneralMessage(true, false, guild.Level, (ulong)guild.ExperienceLevelFloor, (ulong)guild.Experience, (ulong)guild.ExperienceNextLevelFloor, guild.CreationDate.GetUnixTimeStamp(), (ushort)guild.Members.Count, (ushort)guild.Members.Where(entry => entry.IsConnected).Count()));
		}
		public static void SendGuildInformationsMembersMessage(IPacketReceiver client, Guild guild)
		{
			client.Send(new GuildInformationsMembersMessage(
				from x in guild.Members
				select x.GetNetworkGuildMember()));
		}
		public static void SendGuildInformationsMemberUpdateMessage(IPacketReceiver client, Stump.Server.WorldServer.Game.Guilds.GuildMember member)
		{
			client.Send(new GuildInformationsMemberUpdateMessage(member.GetNetworkGuildMember()));
		}
		public static void SendGuildInfosUpgradeMessage(IPacketReceiver client, Guild guild)
		{
            client.Send(new GuildInfosUpgradeMessage((sbyte)guild.MaxTaxCollectors, (sbyte)guild.TaxCollectors.Count, (ushort)guild.TaxCollectorHealth, (ushort)guild.TaxCollectorDamageBonuses, (ushort)guild.TaxCollectorPods, (ushort)guild.TaxCollectorProspecting, (ushort)guild.TaxCollectorWisdom, (ushort)guild.Boost, Guild.TAX_COLLECTOR_SPELLS, 
				from x in guild.GetTaxCollectorSpellsLevels()
				select (sbyte)x));
		}
		public static void SendGuildInformationsPaddocksMessage(IPacketReceiver client)
		{
			client.Send(new GuildInformationsPaddocksMessage(0, new PaddockContentInformations[0]));
		}
		public static void SendGuildHousesInformationMessage(IPacketReceiver client)
		{
			client.Send(new GuildHousesInformationMessage(new HouseInformationsForGuild[0]));
		}
		public static void SendGuildJoinedMessage(IPacketReceiver client, Stump.Server.WorldServer.Game.Guilds.GuildMember member)
		{
			client.Send(new GuildJoinedMessage(member.Guild.GetGuildInformations(), (uint)member.Rights, true));
		}
		public static void SendGuildMemberLeavingMessage(IPacketReceiver client, Stump.Server.WorldServer.Game.Guilds.GuildMember member, bool kicked)
		{
			client.Send(new GuildMemberLeavingMessage(kicked, member.Id));
		}
		public static void SendGuildCreationStartedMessage(IPacketReceiver client)
		{
			client.Send(new GuildCreationStartedMessage());
		}
	}
}
