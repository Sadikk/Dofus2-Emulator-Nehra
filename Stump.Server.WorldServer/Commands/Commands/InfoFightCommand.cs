using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.BaseServer.Commands.Commands;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Fights;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class InfoFightCommand : SubCommand
	{
		public InfoFightCommand()
		{
			base.Aliases = new string[]
			{
				"fight"
			};
			base.RequiredRole = RoleEnum.GameMaster;
			base.Description = "Give informations about a fight";
			base.ParentCommand = typeof(InfoCommand);
			base.AddParameter<Fight>("fight", "f", "Gives informations about the given fight", null, false, ParametersConverter.FightConverter);
		}
		public override void Execute(TriggerBase trigger)
		{
			Fight fight = trigger.Get<Fight>("fight");
			trigger.ReplyBold("Fight {0}", new object[]
			{
				fight.Id
			});
			trigger.ReplyBold("State : {0} Started Since : {1}", new object[]
			{
				fight.State,
				fight.IsStarted ? (System.DateTime.Now - fight.StartTime).ToString("m\\mss\\s") : "not"
			});
			trigger.ReplyBold("Blue team ({0}) :", new object[]
			{
				fight.BlueTeam.Fighters.Count
			});
			foreach (FightActor current in fight.BlueTeam.Fighters)
			{
				trigger.ReplyBold(" - {0} : {1} Level : {2}{3}", new object[]
				{
					current.GetType().Name,
					current,
					current.Level,
					current.IsDead() ? " DEAD" : string.Empty
				});
			}
			trigger.ReplyBold("Red team ({0}) :", new object[]
			{
				fight.RedTeam.Fighters.Count
			});
			foreach (FightActor current in fight.RedTeam.Fighters)
			{
				trigger.ReplyBold(" - {0} : {1} Level : {2}{3}", new object[]
				{
					current.GetType().Name,
					current,
					current.Level,
					current.IsDead() ? " DEAD" : string.Empty
				});
			}
			trigger.ReplyBold("Spectators ({0}) :", new object[]
			{
				fight.Spectators.Count
			});
			foreach (FightSpectator current2 in fight.Spectators)
			{
				trigger.ReplyBold(" - {0}", new object[]
				{
					current2.Character
				});
			}
		}
	}
}
