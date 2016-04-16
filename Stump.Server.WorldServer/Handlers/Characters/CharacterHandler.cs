using Stump.Core.Attributes;
using Stump.Core.Extensions;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Messages;
using Stump.DofusProtocol.Types;
using Stump.Server.BaseServer;
using Stump.Server.BaseServer.Network;
using Stump.Server.WorldServer.Core.IPC;
using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Database.Characters;
using Stump.Server.WorldServer.Game.Accounts;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Fights;
using Stump.Server.WorldServer.Game.Guilds;
using Stump.Server.WorldServer.Handlers.Achievements;
using Stump.Server.WorldServer.Handlers.Alliances;
using Stump.Server.WorldServer.Handlers.Basic;
using Stump.Server.WorldServer.Handlers.Chat;
using Stump.Server.WorldServer.Handlers.Context;
using Stump.Server.WorldServer.Handlers.Context.RolePlay;
using Stump.Server.WorldServer.Handlers.Friends;
using Stump.Server.WorldServer.Handlers.Guilds;
using Stump.Server.WorldServer.Handlers.Initialization;
using Stump.Server.WorldServer.Handlers.Inventory;
using Stump.Server.WorldServer.Handlers.PvP;
using Stump.Server.WorldServer.Handlers.Shortcuts;
using Stump.Server.WorldServer.Handlers.Startup;
using System.Linq;

namespace Stump.Server.WorldServer.Handlers.Characters
{
	public class CharacterHandler : WorldHandlerContainer
	{
		[Variable]
		public static bool EnableNameSuggestion = true;
		[Variable]
		public static int MaxDayCharacterDeletion = 5;

        private CharacterHandler() { }

        [WorldHandler(CharacterFirstSelectionMessage.Id, ShouldBeLogged = false, IsGamePacket = false)]
		public static void HandleCharacterFirstSelectionMessage(WorldClient client, CharacterFirstSelectionMessage message)
		{
			CharacterHandler.HandleCharacterSelectionMessage(client, message);
		}
        [WorldHandler(CharacterSelectionMessage.Id, ShouldBeLogged = false, IsGamePacket = false)]
        public static void HandleCharacterSelectionMessage(WorldClient client, CharacterSelectionMessage message)
        {
            CharacterRecord characterRecord = client.Characters.First((CharacterRecord entry) => entry.Id == message.id);
            if (characterRecord == null)
            {
                client.Send(new CharacterSelectedErrorMessage());
            }
            else
            {
                CharacterHandler.CommonCharacterSelection(client, characterRecord);
            }
        }
        [WorldHandler(CharacterSelectedForceReadyMessage.Id, ShouldBeLogged = false, IsGamePacket = false)]
        public static void HandleCharacterSelectedForceReadyMessage(WorldClient client, CharacterSelectedForceReadyMessage message)
        {
            if (client.Character == null)
            {
                client.Send(new CharacterSelectedErrorMessage());
            }
            else
            {
                CharacterHandler.CommonCharacterBasicInformations(client);
            }
        }
        [WorldHandler(CharactersListRequestMessage.Id, ShouldBeLogged = false, IsGamePacket = false)]
        public static void HandleCharactersListRequestMessage(WorldClient client, CharactersListRequestMessage message)
        {
            if (client.Account != null && client.Account.Login != "")
            {
                CharacterRecord characterRecord = CharacterHandler.FindCharacterFightReconnection(client);
                if (characterRecord != null)
                {
                    var fight = Singleton<FightManager>.Instance.GetFight(characterRecord.LeftFightId.Value);

                    CharacterHandler.SendCharacterSelectedForceMessage(client, characterRecord);
                    CharacterHandler.CommonCharacterForceSelection(client, characterRecord, fight);
                }
                else
                {
                    CharacterHandler.SendCharactersListMessage(client);
                    if (client.WorldAccount != null && client.StartupActions.Count > 0)
                    {
                        StartupHandler.SendStartupActionsListMessage(client, client.StartupActions);
                    }
                }
            }
            else
            {
                client.Send(new IdentificationFailedMessage((sbyte)IdentificationFailureReasonEnum.UNKNOWN_AUTH_ERROR));
                client.DisconnectLater(1000);
            }
        }
        [WorldHandler(CharacterCreationRequestMessage.Id, ShouldBeLogged = false, IsGamePacket = false)]
        public static void HandleCharacterCreationRequestMessage(WorldClient client, CharacterCreationRequestMessage message)
        {
            if (!IPCAccessor.Instance.IsConnected)
            {
                CharacterHandler.OnCharacterCreationFailed(client, CharacterCreationResultEnum.ERR_NO_REASON);
            }
            else
            {
                Singleton<CharacterManager>.Instance.CreateCharacter(client, message.name, message.breed, message.sex, message.colors, message.cosmeticId, delegate
                {
                    CharacterHandler.OnCharacterCreationSuccess(client);
                }, delegate(CharacterCreationResultEnum x)
                {
                    CharacterHandler.OnCharacterCreationFailed(client, x);
                });
            }
        }
        [WorldHandler(CharacterNameSuggestionRequestMessage.Id, ShouldBeLogged = false, IsGamePacket = false)]
        public static void HandleCharacterNameSuggestionRequestMessage(WorldClient client, CharacterNameSuggestionRequestMessage message)
        {
            if (!CharacterHandler.EnableNameSuggestion)
            {
                client.Send(new CharacterNameSuggestionFailureMessage(2));
            }
            else
            {
                string suggestion = Singleton<CharacterManager>.Instance.GenerateName();
                client.Send(new CharacterNameSuggestionSuccessMessage(suggestion));
            }
        }
        [WorldHandler(CharacterDeletionRequestMessage.Id, ShouldBeLogged = false, IsGamePacket = false)]
        public static void HandleCharacterDeletionRequestMessage(WorldClient client, CharacterDeletionRequestMessage message)
        {
            if (!IPCAccessor.Instance.IsConnected)
            {
                client.Send(new CharacterDeletionErrorMessage(1));
                client.DisconnectLater(1000);
            }
            else
            {
                CharacterRecord characterRecord = client.Characters.Find((CharacterRecord entry) => entry.Id == message.characterId);
                if (characterRecord == null)
                {
                    client.Send(new CharacterDeletionErrorMessage(1));
                    client.DisconnectLater(1000);
                }
                else
                {
                    Stump.Server.WorldServer.Game.Guilds.GuildMember guildMember = Singleton<GuildManager>.Instance.TryGetGuildMember(characterRecord.Id);
                    if (guildMember != null && guildMember.IsBoss)
                    {
                        client.Send(new CharacterDeletionErrorMessage(1));
                        client.DisconnectLater(1000);
                    }
                    else
                    {
                        string secretAnswerHash = message.secretAnswerHash;
                        if (Singleton<ExperienceManager>.Instance.GetCharacterLevel(characterRecord.Experience) <= 20 || (client.Account.SecretAnswer != null && secretAnswerHash == (message.characterId + "~" + client.Account.SecretAnswer).GetMD5()))
                        {
                            if (client.Account.DeletedCharactersCount > CharacterHandler.MaxDayCharacterDeletion)
                            {
                                client.Send(new CharacterDeletionErrorMessage(2));
                            }
                            else
                            {
                                Singleton<CharacterManager>.Instance.DeleteCharacterOnAccount(characterRecord, client);
                                CharacterHandler.SendCharactersListMessage(client);
                                BasicHandler.SendBasicNoOperationMessage(client);
                            }
                        }
                        else
                        {
                            client.Send(new CharacterDeletionErrorMessage(3));
                        }
                    }
                }
            }
        }
        [WorldHandler(CharacterReplayRequestMessage.Id, ShouldBeLogged = false, IsGamePacket = false)]
        public static void HandleCharacterReplayRequestMessage(WorldClient client, CharacterReplayRequestMessage message)
        {
        }

		public static void SendCharactersListMessage(WorldClient client)
		{
			System.Collections.Generic.List<CharacterBaseInformations> characters = (
				from characterRecord in client.Characters.OrderByDescending(entry => entry.LastUsage)
                select new CharacterBaseInformations((uint)characterRecord.Id, Singleton<ExperienceManager>.Instance.GetCharacterLevel(characterRecord.Experience), characterRecord.Name, characterRecord.EntityLook.GetEntityLook(), (sbyte)characterRecord.Breed, characterRecord.Sex != SexTypeEnum.SEX_MALE)).ToList<CharacterBaseInformations>();

            client.Send(new CharactersListMessage(characters, false));
		}
		public static void SendCharactersListWithModificationsMessage(WorldClient client)
		{
			System.Collections.Generic.List<CharacterBaseInformations> list = new System.Collections.Generic.List<CharacterBaseInformations>();
			System.Collections.Generic.List<CharacterToRecolorInformation> list2 = new System.Collections.Generic.List<CharacterToRecolorInformation>();
			System.Collections.Generic.List<CharacterToRelookInformation> list3 = new System.Collections.Generic.List<CharacterToRelookInformation>();
			System.Collections.Generic.List<int> list4 = new System.Collections.Generic.List<int>();
			System.Collections.Generic.List<int> list5 = new System.Collections.Generic.List<int>();
			foreach (CharacterRecord current in client.Characters)
			{
                list.Add(new CharacterBaseInformations((uint)current.Id, Singleton<ExperienceManager>.Instance.GetCharacterLevel(current.Experience), current.Name, current.EntityLook.GetEntityLook(), (sbyte)current.Breed, current.Sex != SexTypeEnum.SEX_MALE));
				if (current.Rename)
				{
					list4.Add(current.Id);
				}
				if (current.Recolor)
				{
                    list2.Add(new CharacterToRecolorInformation((uint)current.Id, current.EntityLook.GetEntityLook().indexedColors, (uint)current.Head));
				}
				if (current.Relook)
				{
                    list3.Add(new CharacterToRelookInformation((uint)current.Id, current.EntityLook.GetEntityLook().indexedColors, (uint)current.Head));
				}
				if (!current.Recolor || !current.Rename)
				{
					list5.Add(current.Id);
				}
			}
			client.Send(new CharactersListWithModificationsMessage(list, false, list2, list4, list5, list3));
		}
		public static void SendCharacterSelectedSuccessMessage(WorldClient client)
		{
			client.Send(new CharacterSelectedSuccessMessage(client.Character.GetCharacterBaseInformations(), false));
		}
		public static void SendCharacterCapabilitiesMessage(WorldClient client)
		{
			client.Send(new CharacterCapabilitiesMessage(4095));
		}
		public static void SendCharacterCreationResultMessage(IPacketReceiver client, CharacterCreationResultEnum result)
		{
			client.Send(new CharacterCreationResultMessage((sbyte)result));
		}
		public static void SendLifePointsRegenBeginMessage(IPacketReceiver client, byte regenRate)
		{
			client.Send(new LifePointsRegenBeginMessage(regenRate));
		}
		public static void SendUpdateLifePointsMessage(WorldClient client)
		{
            client.Send(new UpdateLifePointsMessage((uint)client.Character.Stats.Health.Total, (uint)client.Character.Stats.Health.TotalMax));
		}
		public static void SendLifePointsRegenEndMessage(WorldClient client, int recoveredLife)
		{
            client.Send(new LifePointsRegenEndMessage((uint)client.Character.Stats.Health.Total, (uint)client.Character.Stats.Health.TotalMax, (uint)recoveredLife));
		}
		public static void SendCharacterStatsListMessage(WorldClient client)
		{
            client.Send(new CharacterStatsListMessage(new CharacterCharacteristicsInformations((ulong)client.Character.Experience, 
                (ulong)client.Character.LowerBoundExperience,
                (ulong)client.Character.UpperBoundExperience, 
                client.Character.Kamas,
                (ushort)client.Character.StatsPoints,
                0, 
                client.Character.SpellsPoints,
                client.Character.GetActorAlignmentExtendInformations(),
                (uint)client.Character.Stats.Health.Total, 
                (uint)client.Character.Stats.Health.TotalMax, 
                (ushort)client.Character.Energy,
                (ushort)client.Character.EnergyMax, 
                (short)client.Character.Stats[PlayerFields.AP].Total,
                (short)client.Character.Stats[PlayerFields.MP].Total,
                client.Character.Stats[PlayerFields.Initiative],
                client.Character.Stats[PlayerFields.Prospecting], 
                client.Character.Stats[PlayerFields.AP],
                client.Character.Stats[PlayerFields.MP],
                client.Character.Stats[PlayerFields.Strength], 
                client.Character.Stats[PlayerFields.Vitality], 
                client.Character.Stats[PlayerFields.Wisdom], 
                client.Character.Stats[PlayerFields.Chance],
                client.Character.Stats[PlayerFields.Agility],
                client.Character.Stats[PlayerFields.Intelligence],
                client.Character.Stats[PlayerFields.Range],
                client.Character.Stats[PlayerFields.SummonLimit], 
                client.Character.Stats[PlayerFields.DamageReflection], 
                client.Character.Stats[PlayerFields.CriticalHit], (ushort)client.Character.Inventory.WeaponCriticalHit, client.Character.Stats[PlayerFields.CriticalMiss], client.Character.Stats[PlayerFields.HealBonus], client.Character.Stats[PlayerFields.DamageBonus], client.Character.Stats[PlayerFields.WeaponDamageBonus], client.Character.Stats[PlayerFields.DamageBonusPercent], client.Character.Stats[PlayerFields.TrapBonus], client.Character.Stats[PlayerFields.TrapBonusPercent], client.Character.Stats[PlayerFields.GlyphBonusPercent], client.Character.Stats[PlayerFields.PermanentDamagePercent], client.Character.Stats[PlayerFields.TackleBlock], client.Character.Stats[PlayerFields.TackleEvade], client.Character.Stats[PlayerFields.APAttack], client.Character.Stats[PlayerFields.MPAttack], client.Character.Stats[PlayerFields.PushDamageBonus], client.Character.Stats[PlayerFields.CriticalDamageBonus], client.Character.Stats[PlayerFields.NeutralDamageBonus], client.Character.Stats[PlayerFields.EarthDamageBonus], client.Character.Stats[PlayerFields.WaterDamageBonus], client.Character.Stats[PlayerFields.AirDamageBonus], client.Character.Stats[PlayerFields.FireDamageBonus], client.Character.Stats[PlayerFields.DodgeAPProbability], client.Character.Stats[PlayerFields.DodgeMPProbability], client.Character.Stats[PlayerFields.NeutralResistPercent], client.Character.Stats[PlayerFields.EarthResistPercent], client.Character.Stats[PlayerFields.WaterResistPercent], client.Character.Stats[PlayerFields.AirResistPercent], client.Character.Stats[PlayerFields.FireResistPercent], client.Character.Stats[PlayerFields.NeutralElementReduction], client.Character.Stats[PlayerFields.EarthElementReduction], client.Character.Stats[PlayerFields.WaterElementReduction], client.Character.Stats[PlayerFields.AirElementReduction], client.Character.Stats[PlayerFields.FireElementReduction], client.Character.Stats[PlayerFields.PushDamageReduction], client.Character.Stats[PlayerFields.CriticalDamageReduction], client.Character.Stats[PlayerFields.PvpNeutralResistPercent], client.Character.Stats[PlayerFields.PvpEarthResistPercent], client.Character.Stats[PlayerFields.PvpWaterResistPercent], client.Character.Stats[PlayerFields.PvpAirResistPercent], client.Character.Stats[PlayerFields.PvpFireResistPercent], client.Character.Stats[PlayerFields.PvpNeutralElementReduction], client.Character.Stats[PlayerFields.PvpEarthElementReduction], client.Character.Stats[PlayerFields.PvpWaterElementReduction], client.Character.Stats[PlayerFields.PvpAirElementReduction], client.Character.Stats[PlayerFields.PvpFireElementReduction], new System.Collections.Generic.List<CharacterSpellModification>(), 0)));
		}
		public static void SendCharacterLevelUpMessage(IPacketReceiver client, byte level)
		{
			client.Send(new CharacterLevelUpMessage(level));
		}
		public static void SendCharacterLevelUpInformationMessage(IPacketReceiver client, Character character, byte level)
		{
            client.Send(new CharacterLevelUpInformationMessage(level, character.Name, (uint)character.Id));
		}
        public static void SendCharacterExperienceGainMessage(IPacketReceiver client, ulong experienceCharacter, ulong experienceMount, ulong experienceGuild, ulong experienceIncarnation)
        {
            client.Send(new CharacterExperienceGainMessage(experienceCharacter, experienceMount, experienceGuild, experienceIncarnation));
        }
        public static void SendCharacterSelectedForceMessage(IPacketReceiver client, CharacterRecord character)
        {
            client.Send(new CharacterSelectedForceMessage(character.Id));
        }
        
        private static void OnCharacterCreationSuccess(WorldClient client)
        {
            CharacterHandler.SendCharacterCreationResultMessage(client, CharacterCreationResultEnum.OK);
            BasicHandler.SendBasicNoOperationMessage(client);

            CharacterHandler.SendCharactersListMessage(client);
        }
		private static void OnCharacterCreationFailed(WorldClient client, CharacterCreationResultEnum result)
		{
			CharacterHandler.SendCharacterCreationResultMessage(client, result);
		}

        public static void CommonCharacterForceSelection(WorldClient client, CharacterRecord character, Fight fight)
        {
            if (client.WorldAccount == null)
            {
                client.WorldAccount = Singleton<AccountManager>.Instance.FindById(client.Account.Id) ?? Singleton<AccountManager>.Instance.CreateWorldAccount(client);
            }

            client.Character = new Character(character, client);
            var characterFighter = fight.GetFirstFighter<CharacterFighter>(character.Id);
            if (characterFighter == null)
            {
                client.Disconnect();
            }
            else
            {
                characterFighter.ChangeSource(client.Character);
                client.Character.SetFighter(characterFighter);
            }
        }
		public static void CommonCharacterSelection(WorldClient client, CharacterRecord character)
		{
            if (client.WorldAccount == null)
            {
                client.WorldAccount = Singleton<AccountManager>.Instance.FindById(client.Account.Id) ?? Singleton<AccountManager>.Instance.CreateWorldAccount(client);
            }

            client.Character = new Character(character, client);

            CharacterHandler.CommonCharacterBasicInformations(client);
		}

        public static void CommonCharacterBasicInformations(WorldClient client)
        {
            CharacterHandler.SendCharacterSelectedSuccessMessage(client);
            ContextHandler.SendNotificationListMessage(client, new int[] { 2147483647 });
            InventoryHandler.SendInventoryContentMessage(client);
            ShortcutHandler.SendShortcutBarContentMessage(client, ShortcutBarEnum.GENERAL_SHORTCUT_BAR);
            ShortcutHandler.SendShortcutBarContentMessage(client, ShortcutBarEnum.SPELL_SHORTCUT_BAR);
            ContextRoleplayHandler.SendEmoteListMessage(client, (
                from entry in Enumerable.Range(0, 21)
                select (byte)entry).ToList<byte>());

            PvPHandler.SendAlignmentRankUpdateMessage(client);
            if (client.Character.Guild != null)
            {
                GuildHandler.SendGuildMembershipMessage(client, client.Character.GuildMember);
                GuildHandler.SendGuildInformationsGeneralMessage(client, client.Character.Guild);
                GuildHandler.SendGuildInformationsMembersMessage(client, client.Character.Guild);
                if (client.Character.Guild.Alliance != null)
                {
                    AllianceHandler.SendAllianceMembershipMessage(client, client.Character.Guild.Alliance);
                    AllianceHandler.SendAllianceInsiderInfoMessage(client, client.Character.Guild.Alliance);
                }
            }
            ChatHandler.SendEnabledChannelsMessage(client, new sbyte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 13 }, new sbyte[0]);
            InventoryHandler.SendSpellListMessage(client, true);
            InitializationHandler.SendSetCharacterRestrictionsMessage(client);
            InventoryHandler.SendInventoryWeightMessage(client);
            FriendHandler.SendFriendWarnOnConnectionStateMessage(client, client.Character.FriendsBook.WarnOnConnection);
            FriendHandler.SendFriendWarnOnLevelGainStateMessage(client, client.Character.FriendsBook.WarnOnLevel);
            GuildHandler.SendGuildMemberWarnOnConnectionStateMessage(client, client.Character.WarnOnGuildConnection);
            AchievementHandler.SendAchievementListMessage(client, client.Character.Record.FinishedAchievements, client.Character.Achievement.GetRewardableAchievements());
            client.Character.SendConnectionMessages();
            ContextRoleplayHandler.SendGameRolePlayArenaUpdatePlayerInfosMessage(client);
            CharacterHandler.SendCharacterCapabilitiesMessage(client);

            client.WorldAccount.LastConnection = new System.DateTime?(System.DateTime.Now);
            client.WorldAccount.LastIp = client.IP;
            client.WorldAccount.ConnectedCharacter = new int?(client.Character.Id);

            client.Character.Record.LastUsage = new System.DateTime?(System.DateTime.Now);
            ServerBase<WorldServer>.Instance.DBAccessor.Database.Update(client.WorldAccount);
            ServerBase<WorldServer>.Instance.DBAccessor.Database.Update(client.Character.Record);
        }

        private static CharacterRecord FindCharacterFightReconnection(WorldClient client)
        {
            return (
                from x in client.Characters
                where x.LeftFightId.HasValue
                select x into characterInFight
                let fight = Singleton<FightManager>.Instance.GetFight(characterInFight.LeftFightId.Value)
                where fight != null
                let fighter = fight.GetFirstFighter<CharacterFighter>(characterInFight.Id)
                where fighter != null && fighter.IsDisconnected
                select characterInFight).FirstOrDefault<CharacterRecord>();
        }
	}
}
