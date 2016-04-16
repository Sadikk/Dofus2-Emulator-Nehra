using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.WorldServer.Commands.Commands.Patterns;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Fights;
using Stump.Server.WorldServer.Game.Fights.Teams;
using Stump.Server.WorldServer.Game.Maps.Cells;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class JoinFightCommand : TargetSubCommand
	{
		public JoinFightCommand()
		{
			base.Aliases = new string[]
			{
				"join"
			};
			base.Description = "Join a fight";
			base.ParentCommand = typeof(FightCommands);
			base.RequiredRole = RoleEnum.GameMaster;
			base.AddParameter<Fight>("fight", "f", "The fight to join", null, false, ParametersConverter.FightConverter);
			base.AddParameter<string>("team", "team", "Team to join (red or blue)", "red", false, null);
			base.AddTargetParameter(true, "The character that will join the fight");
		}
		public override void Execute(TriggerBase trigger)
		{
			Character target = base.GetTarget(trigger);
			Fight fight = trigger.Get<Fight>("fight");
			string text = trigger.Get<string>("team");
			if (!text.Equals("blue", System.StringComparison.InvariantCultureIgnoreCase) && !text.Equals("red", System.StringComparison.InvariantCultureIgnoreCase))
			{
				trigger.ReplyError("Specify a team (red or blue)");
			}
			else
			{
				bool flag = text.Equals("red", System.StringComparison.InvariantCultureIgnoreCase);
				if (target.IsFighting())
				{
					if (target.Fight == fight)
					{
						trigger.ReplyError("{0} is already in the given fight", new object[]
						{
							target
						});
						return;
					}
					target.Fighter.LeaveFight();
				}
				FightTeam fightTeam = flag ? fight.RedTeam : fight.BlueTeam;
				CharacterFighter characterFighter = target.CreateFighter(fightTeam);
				Cell cell;
				if (!fight.FindRandomFreeCell(characterFighter, out cell, true))
				{
					foreach (FightActor current in fightTeam.Fighters)
					{
						using (System.Collections.Generic.IEnumerator<MapPoint> enumerator2 = current.Position.Point.GetAdjacentCells((short x) => fight.IsCellFree(fight.Map.Cells[(int)x])).GetEnumerator())
						{
							if (enumerator2.MoveNext())
							{
								MapPoint current2 = enumerator2.Current;
								cell = fight.Map.GetCell((int)current2.CellId);
							}
						}
						if (cell != null)
						{
							break;
						}
					}
				}
				if (cell == null)
				{
					target.RejoinMap();
					trigger.ReplyError("{0} cannot join fight {1}, no free cell were found !", new object[]
					{
						target,
						fight.Id
					});
				}
				else
				{
					characterFighter.Cell = cell;
					fightTeam.AddFighter(characterFighter);
					trigger.ReplyBold("{0} joined fight {1}", new object[]
					{
						target,
						fight.Id
					});
				}
			}
		}
	}
}
