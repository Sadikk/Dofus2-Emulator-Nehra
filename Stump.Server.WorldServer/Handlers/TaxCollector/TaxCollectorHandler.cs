using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Messages;
using Stump.Server.BaseServer.Network;
using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Actors.RolePlay.TaxCollectors;
using Stump.Server.WorldServer.Game.Fights;
using Stump.Server.WorldServer.Game.Guilds;
using Stump.Server.WorldServer.Handlers.Context;
using System.Linq;
namespace Stump.Server.WorldServer.Handlers.TaxCollector
{
	public class TaxCollectorHandler : WorldHandlerContainer
	{
        [WorldHandler(GameRolePlayTaxCollectorFightRequestMessage.Id)]
		public static void HandleGameRolePlayTaxCollectorFightRequestMessage(WorldClient client, GameRolePlayTaxCollectorFightRequestMessage message)
		{
			TaxCollectorNpc actor = client.Character.Map.GetActor<TaxCollectorNpc>(message.taxCollectorId);
			FighterRefusedReasonEnum fighterRefusedReasonEnum = client.Character.CanAttack(actor);
			if (fighterRefusedReasonEnum != FighterRefusedReasonEnum.FIGHTER_ACCEPTED)
			{
				ContextHandler.SendChallengeFightJoinRefusedMessage(client, client.Character, fighterRefusedReasonEnum);
			}
			FightPvT fightPvT = Singleton<FightManager>.Instance.CreatePvTFight(client.Character.Map);
			fightPvT.RedTeam.AddFighter(client.Character.CreateFighter(fightPvT.RedTeam));
			fightPvT.BlueTeam.AddFighter(actor.CreateFighter(fightPvT.BlueTeam));
			fightPvT.StartPlacement();
		}
        [WorldHandler(GuildFightJoinRequestMessage.Id)]
		public static void HandleGuildFightJoinRequestMessage(WorldClient client, GuildFightJoinRequestMessage message)
		{
			if (client.Character.Guild != null)
			{
				TaxCollectorNpc taxCollectorNpc = client.Character.Guild.TaxCollectors.FirstOrDefault((TaxCollectorNpc x) => x.GlobalId == message.taxCollectorId);
				if (taxCollectorNpc != null && taxCollectorNpc.IsFighting)
				{
					FightPvT fightPvT = taxCollectorNpc.Fighter.Fight as FightPvT;
					if (fightPvT != null)
					{
						FighterRefusedReasonEnum fighterRefusedReasonEnum = fightPvT.AddDefender(client.Character);
						if (fighterRefusedReasonEnum != FighterRefusedReasonEnum.FIGHTER_ACCEPTED)
						{
							ContextHandler.SendChallengeFightJoinRefusedMessage(client, client.Character, fighterRefusedReasonEnum);
						}
					}
				}
			}
		}
        [WorldHandler(GuildFightLeaveRequestMessage.Id)]
		public static void HandleGuildFightLeaveRequestMessage(WorldClient client, GuildFightLeaveRequestMessage message)
		{
			if (client.Character.Guild != null)
			{
				TaxCollectorNpc taxCollectorNpc = client.Character.Guild.TaxCollectors.FirstOrDefault((TaxCollectorNpc x) => x.GlobalId == message.taxCollectorId);
				if (taxCollectorNpc != null && taxCollectorNpc.IsFighting)
				{
					FightPvT fightPvT = taxCollectorNpc.Fighter.Fight as FightPvT;
					if (fightPvT != null)
					{
						fightPvT.RemoveDefender(client.Character);
					}
				}
			}
		}

		public static void SendTaxCollectorListMessage(IPacketReceiver client, Guild guild)
		{
			client.Send(new TaxCollectorListMessage(
				from x in guild.TaxCollectors
                select x.GetNetworkTaxCollector(), (sbyte)guild.MaxTaxCollectors, guild.TaxCollectors.Where((TaxCollectorNpc x) => x.IsFighting).Select((TaxCollectorNpc x) => x.Fighter.GetTaxCollectorFightersInformation())));
		}
		public static void SendTaxCollectorAttackedMessage(IPacketReceiver client, TaxCollectorNpc taxCollector)
		{
            client.Send(new TaxCollectorAttackedMessage((ushort)taxCollector.FirstNameId, (ushort)taxCollector.LastNameId, (short)taxCollector.Map.Position.X, (short)taxCollector.Map.Position.Y, taxCollector.Map.Id, (ushort)taxCollector.Map.SubArea.Id, taxCollector.Guild.GetBasicGuildInformations()));
		}
		public static void SendGuildFightPlayersHelpersJoinMessage(IPacketReceiver client, TaxCollectorNpc taxCollector, Character character)
		{
			client.Send(new GuildFightPlayersHelpersJoinMessage((int)taxCollector.GlobalId, character.GetCharacterBaseInformations()));
		}
		public static void SendGuildFightPlayersHelpersLeaveMessage(IPacketReceiver client, TaxCollectorNpc taxCollector, Character character)
		{
            client.Send(new GuildFightPlayersHelpersLeaveMessage((int)taxCollector.GlobalId,(uint)character.Id));
		}
		public static void SendGuildFightPlayersEnemyRemoveMessage(IPacketReceiver client, TaxCollectorNpc taxCollector, Character character)
		{
            client.Send(new GuildFightPlayersEnemyRemoveMessage((int)taxCollector.GlobalId, (uint)character.Id));
		}
		public static void SendGuildFightPlayersEnemiesListMessage(IPacketReceiver client, TaxCollectorNpc taxCollector, System.Collections.Generic.IEnumerable<Character> characters)
		{
			client.Send(new GuildFightPlayersEnemiesListMessage((int)taxCollector.GlobalId, 
				from x in characters
				select x.GetCharacterBaseInformations()));
		}
		public static void SendTaxCollectorAttackedResultMessage(IPacketReceiver client, bool deadOrAlive, TaxCollectorNpc taxCollector)
		{
			client.Send(new TaxCollectorAttackedResultMessage(deadOrAlive, taxCollector.GetTaxCollectorBasicInformations(), taxCollector.Guild.GetBasicGuildInformations()));
		}
		public static void SendTaxCollectorMovementMessage(IPacketReceiver client, bool hireOrFire, TaxCollectorNpc taxCollector, string name)
		{
			client.Send(new TaxCollectorMovementMessage(hireOrFire, taxCollector.GetTaxCollectorBasicInformations(), (uint)taxCollector.Id, name));
		}
		public static void SendTaxCollectorMovementAddMessage(IPacketReceiver client, TaxCollectorNpc taxCollector)
		{
			client.Send(new TaxCollectorMovementAddMessage(taxCollector.GetNetworkTaxCollector()));
		}
		public static void SendTaxCollectorMovementRemoveMessage(IPacketReceiver client, TaxCollectorNpc taxCollector)
		{
			client.Send(new TaxCollectorMovementRemoveMessage(taxCollector.Id));
		}
        public static void SendTaxCollectorErrorMessage(IPacketReceiver client, TaxCollectorErrorReasonEnum reason)
        {
            client.Send(new TaxCollectorErrorMessage((sbyte)reason));
        }
    }
}
