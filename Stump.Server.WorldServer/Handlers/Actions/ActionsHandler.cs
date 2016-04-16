using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Messages;
using Stump.Server.BaseServer.Network;
using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Database.Items.Templates;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Actors.Look;
using Stump.Server.WorldServer.Game.Fights.Buffs;
using System.Linq;
namespace Stump.Server.WorldServer.Handlers.Actions
{
	public class ActionsHandler : WorldHandlerContainer
	{
        private ActionsHandler() { }

        [WorldHandler(GameActionAcknowledgementMessage.Id)]
        public static void HandleGameActionAcknowledgementMessage(WorldClient client, GameActionAcknowledgementMessage message)
        {
            if (message.valid && client.Character.IsFighting() && client.Character.Fighter.IsFighterTurn())
            {
                client.Character.Fighter.Fight.AcknowledgeAction();
            }
        }

		public static void SendGameActionFightDeathMessage(IPacketReceiver client, FightActor fighter)
		{
			client.Send(new GameActionFightDeathMessage(103, fighter.Id, fighter.Id));
		}
		public static void SendGameActionFightSummonMessage(IPacketReceiver client, SummonedFighter summon)
		{
			client.Send(new GameActionFightSummonMessage(181, summon.Summoner.Id, summon.GetGameFightFighterInformations()));
		}
		public static void SendGameActionFightInvisibilityMessage(IPacketReceiver client, FightActor source, FightActor target, GameActionFightInvisibilityStateEnum state)
		{
			client.Send(new GameActionFightInvisibilityMessage(150, source.Id, target.Id, (sbyte)state));
		}
		public static void SendGameActionFightDispellEffectMessage(IPacketReceiver client, FightActor source, FightActor target, Buff buff)
		{
			client.Send(new GameActionFightDispellEffectMessage(514, source.Id, target.Id, buff.Id));
		}
		public static void SendGameActionFightReflectDamagesMessage(IPacketReceiver client, FightActor source, FightActor target, int amount)
		{
			client.Send(new GameActionFightReflectDamagesMessage(107, source.Id, target.Id));
		}
		public static void SendGameActionFightPointsVariationMessage(IPacketReceiver client, ActionsEnum action, FightActor source, FightActor target, short delta)
		{
			client.Send(new GameActionFightPointsVariationMessage((ushort)action, source.Id, target.Id, delta));
		}
		public static void SendGameActionFightTackledMessage(IPacketReceiver client, FightActor source, System.Collections.Generic.IEnumerable<FightActor> tacklers)
		{
			client.Send(new GameActionFightTackledMessage(104, source.Id, 
				from entry in tacklers
				select entry.Id));
		}
		public static void SendGameActionFightLifePointsLostMessage(IPacketReceiver client, FightActor source, FightActor target, short loss, short permanentDamages)
		{
            client.Send(new GameActionFightLifePointsLostMessage(100, source.Id, target.Id, (ushort)loss, (ushort)permanentDamages));
		}
		public static void SendGameActionFightDodgePointLossMessage(IPacketReceiver client, ActionsEnum action, FightActor source, FightActor target, short amount)
		{
            client.Send(new GameActionFightDodgePointLossMessage((ushort)action, source.Id, target.Id, (ushort)amount));
		}
		public static void SendGameActionFightReduceDamagesMessage(IPacketReceiver client, FightActor source, FightActor target, int amount)
		{
            client.Send(new GameActionFightReduceDamagesMessage(105, source.Id, target.Id, (ushort)amount));
		}
		public static void SendGameActionFightReflectSpellMessage(IPacketReceiver client, FightActor source, FightActor target)
		{
			client.Send(new GameActionFightReflectSpellMessage(106, source.Id, target.Id));
		}
		public static void SendGameActionFightTeleportOnSameMapMessage(IPacketReceiver client, FightActor source, FightActor target, Cell destination)
		{
			client.Send(new GameActionFightTeleportOnSameMapMessage(4, source.Id, target.Id, destination.Id));
		}
		public static void SendGameActionFightSlideMessage(IPacketReceiver client, FightActor source, FightActor target, short startCellId, short endCellId)
		{
			client.Send(new GameActionFightSlideMessage(5, source.Id, target.Id, startCellId, endCellId));
		}
		public static void SendGameActionFightCloseCombatMessage(IPacketReceiver client, FightActor source, FightActor target, Cell cell, FightSpellCastCriticalEnum castCritical, bool silentCast, WeaponTemplate weapon)
		{
			ActionsEnum actionsEnum = ActionsEnum.ACTION_FIGHT_CLOSE_COMBAT;
			switch (castCritical)
			{
			case FightSpellCastCriticalEnum.CRITICAL_HIT:
				actionsEnum = ActionsEnum.ACTION_FIGHT_CLOSE_COMBAT_CRITICAL_HIT;
				break;
			case FightSpellCastCriticalEnum.CRITICAL_FAIL:
				actionsEnum = ActionsEnum.ACTION_FIGHT_CLOSE_COMBAT_CRITICAL_MISS;
				break;
			}
            client.Send(new GameActionFightCloseCombatMessage((ushort)actionsEnum, source.Id, (target == null) ? 0 : target.Id, cell.Id, (sbyte)castCritical, silentCast, (ushort)weapon.Id));
		}
		public static void SendGameActionFightChangeLookMessage(IPacketReceiver client, FightActor source, FightActor target, ActorLook look)
		{
			client.Send(new GameActionFightChangeLookMessage(149, source.Id, target.Id, look.GetEntityLook()));
		}
		public static void SendGameActionFightExchangePositionsMessage(IPacketReceiver client, FightActor caster, FightActor target)
		{
			client.Send(new GameActionFightExchangePositionsMessage(8, caster.Id, target.Id, caster.Cell.Id, target.Cell.Id));
		}
		public static void SendSequenceStartMessage(IPacketReceiver client, FightActor entity, SequenceTypeEnum sequenceType)
		{
			client.Send(new SequenceStartMessage((sbyte)sequenceType, entity.Id));
		}
		public static void SendSequenceEndMessage(IPacketReceiver client, FightActor entity, SequenceTypeEnum sequenceType, SequenceTypeEnum lastSequenceType)
		{
            client.Send(new SequenceEndMessage((ushort)lastSequenceType, entity.Id, (sbyte)sequenceType));
		}
	}
}
