using Stump.Core.Extensions;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Messages;
using Stump.DofusProtocol.Types;
using Stump.Server.BaseServer.Network;
using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Database.Breeds;
using Stump.Server.WorldServer.Database.Npcs;
using Stump.Server.WorldServer.Game.Actors;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Actors.Interfaces;
using Stump.Server.WorldServer.Game.Actors.RolePlay;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Npcs;
using Stump.Server.WorldServer.Game.Fights;
using Stump.Server.WorldServer.Handlers.Basic;
using System.Linq;

namespace Stump.Server.WorldServer.Handlers.Context.RolePlay
{
	public class ContextRoleplayHandler : WorldHandlerContainer
	{
        private ContextRoleplayHandler() { }

		private static readonly System.Collections.Generic.Dictionary<StatsBoostTypeEnum, PlayerFields> m_statsEnumRelations = new System.Collections.Generic.Dictionary<StatsBoostTypeEnum, PlayerFields>
		{
			{
				StatsBoostTypeEnum.Strength,
				PlayerFields.Strength 
            },
			{
				StatsBoostTypeEnum.Agility,
				PlayerFields.Agility
			},
			{
				StatsBoostTypeEnum.Chance,
				PlayerFields.Chance
			},
			{
				StatsBoostTypeEnum.Wisdom,
				PlayerFields.Wisdom
			},
			{
				StatsBoostTypeEnum.Intelligence,
				PlayerFields.Intelligence
			},
			{
				StatsBoostTypeEnum.Vitality,
				PlayerFields.Vitality
			}
		};

        [WorldHandler(EmotePlayRequestMessage.Id)]
		public static void HandleEmotePlayRequestMessage(WorldClient client, EmotePlayRequestMessage message)
		{
			client.Character.PlayEmote((EmotesEnum)message.emoteId);
		}
        [WorldHandler(ChangeMapMessage.Id)]
		public static void HandleChangeMapMessage(WorldClient client, ChangeMapMessage message)
		{
			MapNeighbour clientMapRelativePosition = client.Character.Map.GetClientMapRelativePosition(message.mapId);
			if (clientMapRelativePosition != MapNeighbour.None && client.Character.Position.Cell.MapChangeData != 0)
			{
				client.Character.Teleport(clientMapRelativePosition);
			}
		}
        [WorldHandler(MapInformationsRequestMessage.Id)]
		public static void HandleMapInformationsRequestMessage(WorldClient client, MapInformationsRequestMessage message)
		{
			ContextRoleplayHandler.SendMapComplementaryInformationsDataMessage(client);
			short fightCount = client.Character.Map.GetFightCount();
			if (fightCount > 0)
			{
				ContextRoleplayHandler.SendMapFightCountMessage(client, fightCount);
			}
		}
        [WorldHandler(MapRunningFightListRequestMessage.Id)]
		public static void HandleMapRunningFightListRequestMessage(WorldClient client, MapRunningFightListRequestMessage message)
		{
			ContextRoleplayHandler.SendMapRunningFightListMessage(client, client.Character.Map.Fights);
		}
        [WorldHandler(MapRunningFightDetailsRequestMessage.Id)]
		public static void HandleMapRunningFightDetailsRequestMessage(WorldClient client, MapRunningFightDetailsRequestMessage message)
		{
			Fight fight = Singleton<FightManager>.Instance.GetFight(message.fightId);
			if (fight != null && !(fight.Map != client.Character.Map))
			{
				ContextRoleplayHandler.SendMapRunningFightDetailsMessage(client, fight);
				BasicHandler.SendBasicNoOperationMessage(client);
			}
		}
        [WorldHandler(NpcGenericActionRequestMessage.Id)]
		public static void HandleNpcGenericActionRequestMessage(WorldClient client, NpcGenericActionRequestMessage message)
		{
			IInteractNpc interactNpc = client.Character.Map.GetActor<RolePlayActor>(message.npcId) as IInteractNpc;
			if (interactNpc != null)
			{
				interactNpc.InteractWith((NpcActionTypeEnum)message.npcActionId, client.Character);
			}
		}
        [WorldHandler(NpcDialogReplyMessage.Id)]
		public static void HandleNpcDialogReplyMessage(WorldClient client, NpcDialogReplyMessage message)
		{
			client.Character.ReplyToNpc((short)message.replyId);
		}
        [WorldHandler(QuestListRequestMessage.Id)]
		public static void HandleQuestListRequestMessage(WorldClient client, QuestListRequestMessage message)
		{
			ContextRoleplayHandler.SendQuestListMessage(client);
		}
        [WorldHandler(StatsUpgradeRequestMessage.Id)]
		public static void HandleStatsUpgradeRequestMessage(WorldClient client, StatsUpgradeRequestMessage message)
		{
			StatsBoostTypeEnum statId = (StatsBoostTypeEnum)message.statId;
			if (statId < StatsBoostTypeEnum.Strength || statId > StatsBoostTypeEnum.Intelligence)
			{
				throw new System.Exception("Wrong statsid");
			}
			if (message.boostPoint > 0)
			{
				Breed breed = client.Character.Breed;
				int num = client.Character.Stats[ContextRoleplayHandler.m_statsEnumRelations[statId]].Base;
				short num2 = (short)message.boostPoint;
				if (num2 >= 1 && message.boostPoint <= (short)client.Character.StatsPoints)
				{
					uint[][] thresholds = breed.GetThresholds(statId);
					int thresholdIndex = breed.GetThresholdIndex(num, thresholds);
					while ((long)num2 >= (long)((ulong)thresholds[thresholdIndex][1]))
					{
						short num3;
						short num4;
						if (thresholdIndex < thresholds.Length - 1 && (double)num2 / thresholds[thresholdIndex][1] > (double)((ulong)thresholds[thresholdIndex + 1][0] - (ulong)((long)num)))
						{
							num3 = (short)((ulong)thresholds[thresholdIndex + 1][0] - (ulong)((long)num));
							num4 = (short)((long)num3 * (long)((ulong)thresholds[thresholdIndex][1]));
							if (thresholds[thresholdIndex].Length > 2)
							{
								num3 = (short)((long)num3 * (long)((ulong)thresholds[thresholdIndex][2]));
							}
						}
						else
						{
							num3 = (short)System.Math.Floor((double)num2 / thresholds[thresholdIndex][1]);
							num4 = (short)((long)num3 * (long)((ulong)thresholds[thresholdIndex][1]));
							if (thresholds[thresholdIndex].Length > 2)
							{
								num3 = (short)((long)num3 * (long)((ulong)thresholds[thresholdIndex][2]));
							}
						}
						num += (int)num3;
						num2 -= num4;
						thresholdIndex = breed.GetThresholdIndex(num, thresholds);
					}
					client.Character.Stats[ContextRoleplayHandler.m_statsEnumRelations[statId]].Base = num;
					Character expr_1A0 = client.Character;
					expr_1A0.StatsPoints -= (ushort)(message.boostPoint - num2);
                    ContextRoleplayHandler.SendStatsUpgradeResultMessage(client, (short)message.boostPoint);
					client.Character.RefreshStats();
				}
			}
		}

		public static void SendStatsUpgradeResultMessage(IPacketReceiver client, short usedpts)
		{
            client.Send(new StatsUpgradeResultMessage((sbyte)StatsUpgradeResultEnum.SUCCESS, (ushort)usedpts));
		}
        public static void SendEmotePlayMessage(IPacketReceiver client, Character character, EmotesEnum emote)
        {
            client.Send(new EmotePlayMessage((byte)emote, (double)System.DateTime.Now.GetUnixTimeStampLong(), character.Id, character.Account.Id));
        }
        public static void SendEmotePlayMessage(IPacketReceiver client, ContextActor actor, EmotesEnum emote)
        {
            client.Send(new EmotePlayMessage((byte)emote, (double)System.DateTime.Now.GetUnixTimeStampLong(), actor.Id, 0));
        }
        public static void SendEmoteListMessage(IPacketReceiver client, System.Collections.Generic.IEnumerable<byte> emoteList)
        {
            client.Send(new EmoteListMessage(emoteList));
        }
        public static void SendGameRolePlayPlayerFightFriendlyAnsweredMessage(IPacketReceiver client, Character replier, Character source, Character target, bool accepted)
        {
            client.Send(new GameRolePlayPlayerFightFriendlyAnsweredMessage(replier.Id, (uint)source.Id, (uint)target.Id, accepted));
        }
        public static void SendGameRolePlayPlayerFightFriendlyRequestedMessage(IPacketReceiver client, Character requester, Character source, Character target)
        {
            client.Send(new GameRolePlayPlayerFightFriendlyRequestedMessage(requester.Id, (uint)source.Id, (uint)target.Id));
        }
        public static void SendGameRolePlayArenaUpdatePlayerInfosMessage(IPacketReceiver client)
        {
            client.Send(new GameRolePlayArenaUpdatePlayerInfosMessage(0, 0, 0, 0, 0));
        }
        public static void SendQuestListMessage(IPacketReceiver client)
        {
            client.Send(new QuestListMessage(Enumerable.Empty<ushort>(), Enumerable.Empty<ushort>(), new QuestActiveInformations[0], Enumerable.Empty<ushort>()));
        }
        public static void SendSpellForgottenMessage(IPacketReceiver client)
        {
            client.Send(new SpellForgottenMessage(new System.Collections.Generic.List<ushort>(), 0));
        }
        public static void SendNpcDialogCreationMessage(IPacketReceiver client, Npc npc)
        {
            client.Send(new NpcDialogCreationMessage(npc.Position.Map.Id, npc.Id));
        }
        public static void SendNpcDialogQuestionMessage(IPacketReceiver client, NpcMessage message, System.Collections.Generic.IEnumerable<short> replies, params string[] parameters)
        {
            client.Send(new NpcDialogQuestionMessage((ushort)message.Id, parameters, replies.Cast<ushort>()));
        }
        public static void SendMapRunningFightListMessage(IPacketReceiver client, System.Collections.Generic.IEnumerable<Fight> fights)
        {
            client.Send(new MapRunningFightListMessage(
                from entry in fights
                select entry.GetFightExternalInformations()));
        }
        public static void SendMapRunningFightDetailsMessage(IPacketReceiver client, Fight fight)
        {
            FightActor[] array = fight.RedTeam.GetAllFighters().ToArray<FightActor>();
            FightActor[] second = fight.BlueTeam.GetAllFighters().ToArray<FightActor>();
            client.Send(new MapRunningFightDetailsMessage(fight.Id, array.Select(entry => entry.GetGameFightFighterLightInformations()), second.Select(entry => entry.GetGameFightFighterLightInformations())));
        }
        public static void SendCurrentMapMessage(IPacketReceiver client, int mapId)
        {
            client.Send(new CurrentMapMessage(mapId, "649ae451ca33ec53bbcbcc33becf15f4"));
        }
        public static void SendMapFightCountMessage(IPacketReceiver client, short fightsCount)
        {
            client.Send(new MapFightCountMessage((ushort)fightsCount));
        }
        public static void SendMapComplementaryInformationsDataMessage(WorldClient client)
        {
            client.Send(client.Character.Map.GetMapComplementaryInformationsDataMessage(client.Character));
        }
        public static void SendGameRolePlayShowActorMessage(IPacketReceiver client, Character character, RolePlayActor actor)
        {
            client.Send(new GameRolePlayShowActorMessage(actor.GetGameContextActorInformations(character) as GameRolePlayActorInformations));
        }
	}
}
