using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Messages;
using Stump.DofusProtocol.Types;
using Stump.Server.BaseServer.Network;
using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Monsters;
using Stump.Server.WorldServer.Game.Fights;
using Stump.Server.WorldServer.Game.Fights.Buffs;
using Stump.Server.WorldServer.Game.Fights.Teams;
using Stump.Server.WorldServer.Game.Fights.Triggers;
using Stump.Server.WorldServer.Game.Maps.Cells;
using Stump.Server.WorldServer.Game.Maps.Pathfinding;
using Stump.Server.WorldServer.Game.Spells;
using Stump.Server.WorldServer.Handlers.Basic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stump.Server.WorldServer.Handlers.Context
{
	public class ContextHandler : WorldHandlerContainer
	{
        private ContextHandler() { }

        [WorldHandler(GameActionFightCastRequestMessage.Id)]
		public static void HandleGameActionFightCastRequestMessage(WorldClient client, GameActionFightCastRequestMessage message)
		{
			if (client.Character.IsFighting())
			{
				CharacterSpell spell = client.Character.Spells.GetSpell((int)message.spellId);
				if (spell != null)
				{
					client.Character.Fighter.CastSpell(spell, client.Character.Fight.Map.Cells[(int)message.cellId]);
				}
			}
		}
        [WorldHandler(GameActionFightCastOnTargetRequestMessage.Id)]
		public static void HandleGameActionFightCastOnTargetRequestMessage(WorldClient client, GameActionFightCastOnTargetRequestMessage message)
		{
			if (client.Character.IsFighting())
			{
				CharacterSpell spell = client.Character.Spells.GetSpell((int)message.spellId);
				if (spell != null)
				{
					FightActor oneFighter = client.Character.Fight.GetOneFighter(message.targetId);
					if (oneFighter != null)
					{
						client.Character.Fighter.CastSpell(spell, oneFighter.Cell);
					}
				}
			}
		}
        [WorldHandler(GameFightTurnFinishMessage.Id)]
		public static void HandleGameFightTurnFinishMessage(WorldClient client, GameFightTurnFinishMessage message)
		{
			if (client.Character.IsFighting())
			{
				client.Character.Fighter.PassTurn();
			}
		}
        [WorldHandler(GameFightTurnReadyMessage.Id)]
		public static void HandleGameFightTurnReadyMessage(WorldClient client, GameFightTurnReadyMessage message)
		{
			if (client.Character.IsFighting())
			{
				client.Character.Fighter.ToggleTurnReady(message.isReady);
			}
		}
        [WorldHandler(GameFightReadyMessage.Id)]
		public static void HandleGameFightReadyMessage(WorldClient client, GameFightReadyMessage message)
		{
			if (client.Character.IsFighting())
			{
				client.Character.Fighter.ToggleReady(message.isReady);
			}
		}
        [WorldHandler(GameContextQuitMessage.Id)]
		public static void HandleGameContextQuitMessage(WorldClient client, GameContextQuitMessage message)
		{
			if (client.Character.IsFighting())
			{
				client.Character.Fighter.LeaveFight();
			}
			else
			{
				if (client.Character.IsSpectator())
				{
					client.Character.Spectator.Leave();
				}
			}
		}
        [WorldHandler(GameFightPlacementPositionRequestMessage.Id)]
		public static void HandleGameFightPlacementPositionRequestMessage(WorldClient client, GameFightPlacementPositionRequestMessage message)
		{
			if (client.Character.Fighter.Position.Cell.Id != message.cellId)
			{
				client.Character.Fighter.ChangePrePlacement(client.Character.Fight.Map.Cells[(int)message.cellId]);
			}
		}
        [WorldHandler(GameRolePlayPlayerFightRequestMessage.Id)]
		public static void HandleGameRolePlayPlayerFightRequestMessage(WorldClient client, GameRolePlayPlayerFightRequestMessage message)
		{
			Character actor = client.Character.Map.GetActor<Character>((int)message.targetId);
			if (actor != null)
			{
				if (message.friendly)
				{
					FighterRefusedReasonEnum fighterRefusedReasonEnum = client.Character.CanRequestFight(actor);
					if (fighterRefusedReasonEnum != FighterRefusedReasonEnum.FIGHTER_ACCEPTED)
					{
						ContextHandler.SendChallengeFightJoinRefusedMessage(client, client.Character, fighterRefusedReasonEnum);
					}
					else
					{
						FightRequest fightRequest = new FightRequest(client.Character, actor);
						client.Character.OpenRequestBox(fightRequest);
						actor.OpenRequestBox(fightRequest);
						fightRequest.Open();
					}
				}
				else
				{
					FighterRefusedReasonEnum fighterRefusedReasonEnum = client.Character.CanAgress(actor);
					if (fighterRefusedReasonEnum != FighterRefusedReasonEnum.FIGHTER_ACCEPTED)
					{
						ContextHandler.SendChallengeFightJoinRefusedMessage(client, client.Character, fighterRefusedReasonEnum);
					}
					else
					{
						Fight fight = Singleton<FightManager>.Instance.CreateAgressionFight(actor.Map, client.Character.AlignmentSide, actor.AlignmentSide);
						fight.RedTeam.AddFighter(client.Character.CreateFighter(fight.RedTeam));
						fight.BlueTeam.AddFighter(actor.CreateFighter(fight.BlueTeam));
						fight.StartPlacement();
					}
				}
			}
		}
        [WorldHandler(GameRolePlayPlayerFightFriendlyAnswerMessage.Id)]
		public static void HandleGameRolePlayPlayerFightFriendlyAnswerMessage(WorldClient client, GameRolePlayPlayerFightFriendlyAnswerMessage message)
		{
			if (client.Character.IsInRequest() && client.Character.RequestBox is FightRequest)
			{
				if (message.accept)
				{
					client.Character.RequestBox.Accept();
				}
				else
				{
					if (client.Character == client.Character.RequestBox.Target)
					{
						client.Character.RequestBox.Deny();
					}
					else
					{
						client.Character.RequestBox.Cancel();
					}
				}
			}
		}
        [WorldHandler(GameRolePlayAttackMonsterRequestMessage.Id)]
        public static void HandleGameRolePlayAttackMonsterRequestMessage(WorldClient client, GameRolePlayAttackMonsterRequestMessage message)
        {
            if (!client.Character.IsFighting())
            {
                var actor = client.Character.Map.GetActor<MonsterGroup>(message.monsterGroupId);
                if (actor != null)
                {
                    if (actor.Position.Cell == client.Character.Cell)
                    {
                        actor.FightWith(client.Character);
                    }
                }
            }
        }
        [WorldHandler(GameFightOptionToggleMessage.Id)]
		public static void HandleGameFightOptionToggleMessage(WorldClient client, GameFightOptionToggleMessage message)
		{
			if (client.Character.IsFighting() && client.Character.Fighter.IsTeamLeader())
			{
				if (!client.Character.Fight.IsStarted)
				{
					client.Character.Team.ToggleOption((FightOptionsEnum)message.option);
				}
				else
				{
					if (message.option == 0)
					{
						client.Character.Fight.ToggleSpectatorClosed(!client.Character.Fight.SpectatorClosed);
					}
				}
			}
		}
        [WorldHandler(GameFightJoinRequestMessage.Id)]
		public static void HandleGameFightJoinRequestMessage(WorldClient client, GameFightJoinRequestMessage message)
		{
			if (!client.Character.IsFighting())
			{
				Fight fight = Singleton<FightManager>.Instance.GetFight(message.fightId);
				if (fight == null)
				{
					ContextHandler.SendChallengeFightJoinRefusedMessage(client, client.Character, FighterRefusedReasonEnum.TOO_LATE);
				}
				else
				{
					if (fight.Map != client.Character.Map)
					{
						ContextHandler.SendChallengeFightJoinRefusedMessage(client, client.Character, FighterRefusedReasonEnum.WRONG_MAP);
					}
					else
					{
						if (fight.IsStarted)
						{
							if (message.fighterId == 0 && fight.CanSpectatorJoin(client.Character))
							{
								fight.AddSpectator(client.Character.CreateSpectator(fight));
							}
						}
						else
						{
							FightTeam fightTeam;
							if (fight.RedTeam.Leader.Id == message.fighterId)
							{
								fightTeam = fight.RedTeam;
							}
							else
							{
								if (fight.BlueTeam.Leader.Id != message.fighterId)
								{
									ContextHandler.SendChallengeFightJoinRefusedMessage(client, client.Character, FighterRefusedReasonEnum.WRONG_MAP);
									return;
								}
								fightTeam = fight.BlueTeam;
							}
							FighterRefusedReasonEnum reason;
							if ((reason = fightTeam.CanJoin(client.Character)) != FighterRefusedReasonEnum.FIGHTER_ACCEPTED)
							{
								ContextHandler.SendChallengeFightJoinRefusedMessage(client, client.Character, reason);
							}
							else
							{
								fightTeam.AddFighter(client.Character.CreateFighter(fightTeam));
							}
						}
					}
				}
			}
		}
        [WorldHandler(GameContextKickMessage.Id)]
		public static void HandleGameContextKickMessage(WorldClient client, GameContextKickMessage message)
		{
			if (client.Character.IsFighting() && client.Character.Fighter.IsTeamLeader())
			{
				CharacterFighter oneFighter = client.Character.Fight.GetOneFighter<CharacterFighter>(message.targetId);
				if (oneFighter != null && oneFighter.Character.IsFighting() && client.Character.Fight == oneFighter.Character.Fight)
				{
					client.Character.Fight.KickFighter(oneFighter);
				}
			}
		}
        [WorldHandler(GameContextCreateRequestMessage.Id)]
		public static void HandleGameContextCreateRequestMessage(WorldClient client, GameContextCreateRequestMessage message)
		{
			if (client.Character.IsInWorld)
			{
				client.Character.SendServerMessage("You are already Logged !");
			}
			else
			{
				ContextHandler.SendGameContextDestroyMessage(client);
				ContextHandler.SendGameContextCreateMessage(client, (sbyte)(client.Character.IsInFight() ? GameContextEnum.FIGHT : GameContextEnum.ROLE_PLAY));
				client.Character.RefreshStats();
				client.Character.LogIn();
			}
		}
        [WorldHandler(GameContextReadyMessage.Id)]
        public static void HandleGameContextReadyMessage(WorldClient client, GameContextReadyMessage message)
        {
            if (client.Character.IsFighting())
            {
                client.Character.Fighter.Map.Area.AddMessage(delegate
                {
                    client.Character.Fighter.Fight.Reconnect(client.Character.Fighter);
                });
            }
        }
        [WorldHandler(GameMapChangeOrientationRequestMessage.Id)]
		public static void HandleGameMapChangeOrientationRequestMessage(WorldClient client, GameMapChangeOrientationRequestMessage message)
		{
			client.Character.Direction = (DirectionsEnum)message.direction;
			ContextHandler.SendGameMapChangeOrientationMessage(client.Character.CharacterContainer.Clients, client.Character);
		}
        [WorldHandler(GameMapMovementRequestMessage.Id)]
		public static void HandleGameMapMovementRequestMessage(WorldClient client, GameMapMovementRequestMessage message)
		{
			if (!client.Character.CanMove())
			{
				ContextHandler.SendGameMapNoMovementMessage(client);
			}
			Path movementPath = Path.BuildFromCompressedPath(client.Character.Map, message.keyMovements);
			client.Character.StartMove(movementPath);
		}
        [WorldHandler(GameMapMovementConfirmMessage.Id)]
		public static void HandleGameMapMovementConfirmMessage(WorldClient client, GameMapMovementConfirmMessage message)
		{
			client.Character.StopMove();

            BasicHandler.SendBasicNoOperationMessage(client);
		}
        [WorldHandler(GameMapMovementCancelMessage.Id)]
		public static void HandleGameMapMovementCancelMessage(WorldClient client, GameMapMovementCancelMessage message)
		{
            client.Character.StopMove(new ObjectPosition(client.Character.Map, (short)message.cellId, client.Character.Position.Direction));

            BasicHandler.SendBasicNoOperationMessage(client);
		}
        [WorldHandler(ShowCellRequestMessage.Id)]
		public static void HandleShowCellRequestMessage(WorldClient client, ShowCellRequestMessage message)
		{
			if (client.Character.IsFighting())
			{
				client.Character.Fighter.ShowCell(client.Character.Map.Cells[(int)message.cellId], true);
			}
			else
			{
				if (client.Character.IsSpectator())
				{
					client.Character.Spectator.ShowCell(client.Character.Map.Cells[(int)message.cellId]);
				}
			}
		}
        [WorldHandler(ChallengeTargetsListRequestMessage.Id)]
        public static void HandleChallengeTargetsListRequestMessage(WorldClient client, ChallengeTargetsListRequestMessage message)
        {
            if (client.Character.Fight != null)
            {
                var challenge = client.Character.Fight.GetChallenge(message.challengeId);
                if (challenge != null)
                {
                    var target = client.Character.Fight.GetOneFighter(challenge.TargetId);
                    if (target != null && target.IsVisibleFor(client.Character))
                    {
                        ContextHandler.SendChallengeTargetsListMessage(client, new int[1] { target.Id }, new short[1] { target.Cell.Id });
                    }
                    else
                    {
                        ContextHandler.SendChallengeTargetsListMessage(client, new int[0], new short[0]);
                    }
                }
            }
        }

        public static void SendGameFightStartMessage(IPacketReceiver client)
        {
            client.Send(new GameFightStartMessage(new Idol[0]));
        }
        public static void SendGameFightStartingMessage(IPacketReceiver client, FightTypeEnum fightTypeEnum)
        {
            client.Send(new GameFightStartingMessage((sbyte)fightTypeEnum, 0, 0));
        }
        public static void SendGameRolePlayShowChallengeMessage(IPacketReceiver client, Fight fight)
        {
            client.Send(new GameRolePlayShowChallengeMessage(fight.GetFightCommonInformations()));
        }
        public static void SendGameRolePlayRemoveChallengeMessage(IPacketReceiver client, Fight fight)
        {
            client.Send(new GameRolePlayRemoveChallengeMessage(fight.Id));
        }
        public static void SendGameFightEndMessage(IPacketReceiver client, Fight fight)
        {
            client.Send(new GameFightEndMessage(fight.GetFightDuration(), fight.AgeBonus, 0, new FightResultListEntry[0], Enumerable.Empty<NamedPartyTeamWithOutcome>()));
        }
        public static void SendGameFightEndMessage(IPacketReceiver client, Fight fight, System.Collections.Generic.IEnumerable<FightResultListEntry> results)
        {
            client.Send(new GameFightEndMessage(fight.GetFightDuration(), fight.AgeBonus, 0, results, Enumerable.Empty<NamedPartyTeamWithOutcome>()));
        }
        public static void SendGameFightJoinMessage(IPacketReceiver client, bool canBeCancelled, bool canSayReady, bool isSpectator, bool isFightStarted, int timeMaxBeforeFightStart, FightTypeEnum fightTypeEnum)
        {
            client.Send(new GameFightJoinMessage(canBeCancelled, canSayReady, isFightStarted, (short)timeMaxBeforeFightStart, (sbyte)fightTypeEnum));
        }
        public static void SendGameFightSpectateMessage(IPacketReceiver client, Fight fight)
        {
            client.Send(new GameFightSpectateMessage(
                from entry in fight.GetBuffs()
                select entry.GetFightDispellableEffectExtendedInformations(), fight.GetTriggers().Select((MarkTrigger entry) => entry.GetHiddenGameActionMark()), (ushort)fight.TimeLine.RoundNumber, fight.GetFightDuration(), Enumerable.Empty<Idol>()));
        }
        public static void SendGameFightTurnResumeMessage(IPacketReceiver client, FightActor playingTurn, int waitTime)
        {
            client.Send(new GameFightTurnResumeMessage(playingTurn.Id, (uint)waitTime, 0));
        }
        public static void SendChallengeFightJoinRefusedMessage(IPacketReceiver client, Character character, FighterRefusedReasonEnum reason)
        {
            client.Send(new ChallengeFightJoinRefusedMessage((uint)character.Id, (sbyte)reason));
        }
        public static void SendGameFightHumanReadyStateMessage(IPacketReceiver client, FightActor fighter)
        {
            client.Send(new GameFightHumanReadyStateMessage((uint)fighter.Id, fighter.IsReady));
        }
        public static void SendGameFightSynchronizeMessage(WorldClient client, Fight fight)
        {
            client.Send(new GameFightSynchronizeMessage(
                from entry in fight.GetAllFighters()
                select entry.GetGameFightFighterInformations(client)));
        }
        public static void SendGameFightNewRoundMessage(IPacketReceiver client, int roundNumber)
        {
            client.Send(new GameFightNewRoundMessage((uint)roundNumber));
        }
        public static void SendGameFightTurnListMessage(IPacketReceiver client, Fight fight)
        {
            client.Send(new GameFightTurnListMessage(fight.GetAliveFightersIds(), fight.GetDeadFightersIds()));
        }
        public static void SendGameFightTurnStartMessage(IPacketReceiver client, int id, uint waitTime)
        {
            client.Send(new GameFightTurnStartMessage(id, waitTime / 100));
        }
        public static void SendGameFightTurnFinishMessage(IPacketReceiver client)
        {
            client.Send(new GameFightTurnFinishMessage());
        }
        public static void SendGameFightTurnEndMessage(IPacketReceiver client, FightActor fighter)
        {
            client.Send(new GameFightTurnEndMessage(fighter.Id));
        }
        public static void SendGameFightUpdateTeamMessage(IPacketReceiver client, Fight fight, FightTeam team)
        {
            client.Send(new GameFightUpdateTeamMessage((short)fight.Id, team.GetFightTeamInformations()));
        }
        public static void SendGameFightShowFighterMessage(WorldClient client, FightActor fighter)
        {
            client.Send(new GameFightShowFighterMessage(fighter.GetGameFightFighterInformations(client)));
        }
        public static void SendGameFightRefreshFighterMessage(WorldClient client, FightActor fighter)
        {
            client.Send(new GameFightRefreshFighterMessage(fighter.GetGameFightFighterInformations(client)));
        }
        public static void SendGameFightRemoveTeamMemberMessage(IPacketReceiver client, FightActor fighter)
        {
            client.Send(new GameFightRemoveTeamMemberMessage((short)fighter.Fight.Id, fighter.Team.Id, fighter.Id));
        }
        public static void SendGameFightLeaveMessage(IPacketReceiver client, FightActor fighter)
        {
            client.Send(new GameFightLeaveMessage(fighter.Id));
        }
        public static void SendGameFightPlacementPossiblePositionsMessage(IPacketReceiver client, Fight fight, sbyte team)
        {
            client.Send(new GameFightPlacementPossiblePositionsMessage(
                from entry in fight.RedTeam.PlacementCells
                select (ushort)entry.Id, fight.BlueTeam.PlacementCells.Select((Cell entry) => (ushort)entry.Id), team));
        }
        public static void SendGameFightOptionStateUpdateMessage(IPacketReceiver client, FightTeam team, FightOptionsEnum option, bool state)
        {
            client.Send(new GameFightOptionStateUpdateMessage((short)team.Fight.Id, team.Id, (sbyte)option, state));
        }
        public static void SendGameActionFightSpellCastMessage(IPacketReceiver client, ActionsEnum actionId, FightActor caster, FightActor target, Cell cell, FightSpellCastCriticalEnum critical, bool silentCast, Spell spell)
        {
            client.Send(new GameActionFightSpellCastMessage((ushort)actionId, caster.Id, (target == null) ? 0 : target.Id, cell.Id, (sbyte)critical, silentCast, (ushort)spell.Id, (sbyte)spell.CurrentLevel, Enumerable.Empty<short>()));
        }
        public static void SendGameActionFightSpellCastMessage(IPacketReceiver client, ActionsEnum actionId, FightActor caster, FightActor target, Cell cell, FightSpellCastCriticalEnum critical, bool silentCast, short spellId, sbyte spellLevel)
        {
            client.Send(new GameActionFightSpellCastMessage((ushort)actionId, caster.Id, (target == null) ? 0 : target.Id, cell.Id, (sbyte)critical, silentCast, (ushort)spellId, spellLevel, Enumerable.Empty<short>()));
        }
        public static void SendGameActionFightNoSpellCastMessage(IPacketReceiver client, Spell spell)
        {
            client.Send(new GameActionFightNoSpellCastMessage((uint)spell.Id));
        }
        public static void SendGameActionFightModifyEffectsDurationMessage(IPacketReceiver client, FightActor source, FightActor target, short delta)
        {
            client.Send(new GameActionFightModifyEffectsDurationMessage(515, source.Id, target.Id, delta));
        }
        public static void SendGameActionFightDispellableEffectMessage(IPacketReceiver client, Buff buff, bool update = false)
        {
            client.Send(new GameActionFightDispellableEffectMessage((ushort)Convert.ToInt16(update ? 515 : buff.GetActionId()), buff.Caster.Id, buff.GetAbstractFightDispellableEffect()));
        }
        public static void SendGameActionFightDispellableEffectMessage(IPacketReceiver client, ushort actionId, FightActor caster, AbstractFightDispellableEffect effect, bool update = false)
        {
            client.Send(new GameActionFightDispellableEffectMessage(Convert.ToUInt16(update ? 515 : actionId), caster.Id, effect));
        }
        public static void SendGameActionFightMarkCellsMessage(IPacketReceiver client, MarkTrigger trigger, bool visible = true)
        {
            ActionsEnum actionsEnum = (trigger.Type == GameActionMarkTypeEnum.GLYPH) ? ActionsEnum.ACTION_FIGHT_ADD_GLYPH_CASTING_SPELL : ActionsEnum.ACTION_FIGHT_ADD_TRAP_CASTING_SPELL;
            client.Send(new GameActionFightMarkCellsMessage((ushort)actionsEnum, trigger.Caster.Id, visible ? trigger.GetGameActionMark() : trigger.GetHiddenGameActionMark()));
        }
        public static void SendGameActionFightUnmarkCellsMessage(IPacketReceiver client, MarkTrigger trigger)
        {
            client.Send(new GameActionFightUnmarkCellsMessage(310, trigger.Caster.Id, trigger.Id));
        }
        public static void SendGameActionFightTriggerGlyphTrapMessage(IPacketReceiver client, MarkTrigger trigger, FightActor target, Spell triggeredSpell)
        {
            ActionsEnum actionsEnum = (trigger.Type == GameActionMarkTypeEnum.GLYPH) ? ActionsEnum.ACTION_FIGHT_TRIGGER_GLYPH : ActionsEnum.ACTION_FIGHT_TRIGGER_TRAP;
            client.Send(new GameActionFightTriggerGlyphTrapMessage((ushort)actionsEnum, trigger.Caster.Id, trigger.Id, target.Id, (ushort)triggeredSpell.Id));
        }
        public static void SendGameFightTurnReadyRequestMessage(IPacketReceiver client, FightActor current)
        {
            client.Send(new GameFightTurnReadyRequestMessage(current.Id));
        }
		public static void SendGameMapNoMovementMessage(IPacketReceiver client)
		{
			client.Send(new GameMapNoMovementMessage());
		}
		public static void SendGameContextCreateMessage(IPacketReceiver client, sbyte context)
		{
			client.Send(new GameContextCreateMessage(context));
		}
		public static void SendGameContextDestroyMessage(IPacketReceiver client)
		{
			client.Send(new GameContextDestroyMessage());
		}
		public static void SendGameMapChangeOrientationMessage(IPacketReceiver client, ContextActor actor)
		{
			client.Send(new GameMapChangeOrientationMessage(new ActorOrientation(actor.Id, (sbyte)actor.Position.Direction)));
		}
		public static void SendGameContextRemoveElementMessage(IPacketReceiver client, ContextActor actor)
		{
			client.Send(new GameContextRemoveElementMessage(actor.Id));
		}
		public static void SendShowCellSpectatorMessage(IPacketReceiver client, FightSpectator spectator, Cell cell)
		{
            client.Send(new ShowCellSpectatorMessage(spectator.Character.Id, (ushort)cell.Id, spectator.Character.Name));
		}
		public static void SendShowCellMessage(IPacketReceiver client, ContextActor source, Cell cell)
		{
            client.Send(new ShowCellMessage(source.Id, (ushort)cell.Id));
		}
		public static void SendGameContextRefreshEntityLookMessage(IPacketReceiver client, ContextActor actor)
		{
			client.Send(new GameContextRefreshEntityLookMessage(actor.Id, actor.Look.GetEntityLook()));
		}
		public static void SendGameMapMovementMessage(IPacketReceiver client, System.Collections.Generic.IEnumerable<short> movementsKey, ContextActor actor)
		{
			client.Send(new GameMapMovementMessage(movementsKey, actor.Id));
		}
		public static void SendGameEntitiesDispositionMessage(IPacketReceiver client, System.Collections.Generic.IEnumerable<ContextActor> actors)
		{
			client.Send(new GameEntitiesDispositionMessage(
				from entry in actors
				select entry.GetIdentifiedEntityDispositionInformations()));
		}
		public static void SendNotificationListMessage(IPacketReceiver client, System.Collections.Generic.IEnumerable<int> notifications)
		{
			client.Send(new NotificationListMessage(notifications));
		}
        public static void SendIdolFightPreparationUpdateMessage(IPacketReceiver client)
        {
            client.Send(new IdolFightPreparationUpdateMessage(0, new Idol[0]));
        }
        public static void SendFighterStatsListMessage(IPacketReceiver client, CharacterFighter fighter)
        {
            client.Send(new FighterStatsListMessage(new CharacterCharacteristicsInformations((ulong)fighter.Character.Experience,
                (ulong)fighter.Character.LowerBoundExperience,
                (ulong)fighter.Character.UpperBoundExperience,
                fighter.Character.Kamas,
                (ushort)fighter.Character.StatsPoints,
                0,
                fighter.Character.SpellsPoints,
                fighter.Character.GetActorAlignmentExtendInformations(),
                (uint)fighter.Character.Stats.Health.Total,
                (uint)fighter.Character.Stats.Health.TotalMax,
                (ushort)fighter.Character.Energy,
                (ushort)fighter.Character.EnergyMax,
                (short)fighter.Stats[PlayerFields.AP].Total,
                (short)fighter.Stats[PlayerFields.MP].Total,
                fighter.Stats[PlayerFields.Initiative],
                fighter.Stats[PlayerFields.Prospecting],
                fighter.Stats[PlayerFields.AP],
                fighter.Stats[PlayerFields.MP],
                fighter.Stats[PlayerFields.Strength],
                fighter.Stats[PlayerFields.Vitality],
                fighter.Stats[PlayerFields.Wisdom],
                fighter.Stats[PlayerFields.Chance],
                fighter.Stats[PlayerFields.Agility],
                fighter.Stats[PlayerFields.Intelligence],
                fighter.Stats[PlayerFields.Range],
                fighter.Stats[PlayerFields.SummonLimit],
                fighter.Stats[PlayerFields.DamageReflection],
                fighter.Stats[PlayerFields.CriticalHit], 
                (ushort)fighter.Character.Inventory.WeaponCriticalHit, 
                fighter.Stats[PlayerFields.CriticalMiss],
                fighter.Stats[PlayerFields.HealBonus],
                fighter.Stats[PlayerFields.DamageBonus],
                fighter.Stats[PlayerFields.WeaponDamageBonus],
                fighter.Stats[PlayerFields.DamageBonusPercent],
                fighter.Stats[PlayerFields.TrapBonus],
                fighter.Stats[PlayerFields.TrapBonusPercent],
                fighter.Stats[PlayerFields.GlyphBonusPercent],
                fighter.Stats[PlayerFields.PermanentDamagePercent],
                fighter.Stats[PlayerFields.TackleBlock],
                fighter.Stats[PlayerFields.TackleEvade],
                fighter.Stats[PlayerFields.APAttack],
                fighter.Stats[PlayerFields.MPAttack],
                fighter.Stats[PlayerFields.PushDamageBonus],
                fighter.Stats[PlayerFields.CriticalDamageBonus],
                fighter.Stats[PlayerFields.NeutralDamageBonus],
                fighter.Stats[PlayerFields.EarthDamageBonus],
                fighter.Stats[PlayerFields.WaterDamageBonus],
                fighter.Stats[PlayerFields.AirDamageBonus],
                fighter.Stats[PlayerFields.FireDamageBonus],
                fighter.Stats[PlayerFields.DodgeAPProbability],
                fighter.Stats[PlayerFields.DodgeMPProbability],
                fighter.Stats[PlayerFields.NeutralResistPercent],
                fighter.Stats[PlayerFields.EarthResistPercent],
                fighter.Stats[PlayerFields.WaterResistPercent],
                fighter.Stats[PlayerFields.AirResistPercent],
                fighter.Stats[PlayerFields.FireResistPercent],
                fighter.Stats[PlayerFields.NeutralElementReduction],
                fighter.Stats[PlayerFields.EarthElementReduction],
                fighter.Stats[PlayerFields.WaterElementReduction],
                fighter.Stats[PlayerFields.AirElementReduction],
                fighter.Stats[PlayerFields.FireElementReduction],
                fighter.Stats[PlayerFields.PushDamageReduction],
                fighter.Stats[PlayerFields.CriticalDamageReduction],
                fighter.Stats[PlayerFields.PvpNeutralResistPercent],
                fighter.Stats[PlayerFields.PvpEarthResistPercent],
                fighter.Stats[PlayerFields.PvpWaterResistPercent],
                fighter.Stats[PlayerFields.PvpAirResistPercent],
                fighter.Stats[PlayerFields.PvpFireResistPercent],
                fighter.Stats[PlayerFields.PvpNeutralElementReduction],
                fighter.Stats[PlayerFields.PvpEarthElementReduction],
                fighter.Stats[PlayerFields.PvpWaterElementReduction],
                fighter.Stats[PlayerFields.PvpAirElementReduction],
                fighter.Stats[PlayerFields.PvpFireElementReduction], 
                new List<CharacterSpellModification>(), 0)));
        }
        public static void SendChallengeInfoMessage(IPacketReceiver client, ushort challengeId, int targetId, uint bonus)
        {
            client.Send(new ChallengeInfoMessage(challengeId, targetId, bonus, bonus));
        }
        public static void SendChallengeResultMessage(IPacketReceiver client, ushort challengeId, bool success)
        {
            client.Send(new ChallengeResultMessage(challengeId, success));
        }
        public static void SendChallengeTargetsListMessage(IPacketReceiver client, IEnumerable<int> targetIds, IEnumerable<short> targetCells)
        {
            client.Send(new ChallengeTargetsListMessage(targetIds, targetCells));
        }
        public static void SendGameActionFightChangeLookMessage(IPacketReceiver client, FightActor source, FightActor target)
        {
            client.Send(new GameActionFightChangeLookMessage((ushort)ActionsEnum.ACTION_CHARACTER_CHANGE_LOOK, source.Id, target.Id, target.Look.GetEntityLook()));
        }
        public static void SendGameFightResumeMessage(IPacketReceiver client, Fight fight, FightActor actor)
        {
            client.Send(new GameFightResumeMessage(
                from entry in fight.GetBuffs() select entry.GetFightDispellableEffectExtendedInformations(), 
                fight.GetTriggers().Select((MarkTrigger entry) => entry.GetHiddenGameActionMark()), 
                (ushort)fight.TimeLine.RoundNumber, 
                fight.GetFightDuration(), 
                new Idol[0],
                new GameFightSpellCooldown[0],
                (sbyte)actor.SummonedCount,
                (sbyte)actor.BombsCount));
        }
        public static void SendSlaveSwitchContextMessage(IPacketReceiver client, FightActor source, FightActor target)
        {
            var spells = (target as SummonedMonster).Monster.Spells.Select(x => x.GetSpellItem());
            List<Shortcut> shortcuts = new List<Shortcut>();
            for (sbyte i = 0; i < spells.Count(); i++)
            {
                shortcuts.Add(new ShortcutSpell(i, (ushort)spells.ElementAt(i).spellId));
            }
            client.Send(new SlaveSwitchContextMessage(source.Id, target.Id, spells, target.GetSlaveStats(source), shortcuts));
        }
    }
}
