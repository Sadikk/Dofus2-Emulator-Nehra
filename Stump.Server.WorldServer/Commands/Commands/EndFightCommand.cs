using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.WorldServer.Commands.Trigger;
using Stump.Server.WorldServer.Game.Fights;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class EndFightCommand : SubCommand
	{
		public EndFightCommand()
		{
			base.Aliases = new string[]
			{
				"end",
				"stop"
			};
			base.Description = "Ends a fight";
			base.ParentCommand = typeof(FightCommands);
			base.RequiredRole = RoleEnum.GameMaster;
			base.AddParameter<Fight>("fight", "f", "The fight to end", null, true, ParametersConverter.FightConverter);
		}
		public override void Execute(TriggerBase trigger)
		{
			Fight fight;
			if (trigger.IsArgumentDefined("fight"))
			{
				fight = trigger.Get<Fight>("fight");
			}
			else
			{
				if (!(trigger is GameTrigger) || !(trigger as GameTrigger).Character.IsInFight())
				{
					trigger.ReplyError("Define a fight");
					return;
				}
				fight = (trigger as GameTrigger).Character.Fight;
			}
			fight.EndFight();
			trigger.ReplyBold("Fight {0} ended", new object[]
			{
				fight.Id
			});
		}
	}
}
