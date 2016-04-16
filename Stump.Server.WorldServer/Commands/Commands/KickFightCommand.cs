using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.WorldServer.Commands.Commands.Patterns;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Fights;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class KickFightCommand : TargetSubCommand
	{
		public KickFightCommand()
		{
			base.Aliases = new string[]
			{
				"kick"
			};
			base.Description = "Kick the target";
			base.ParentCommand = typeof(FightCommands);
			base.RequiredRole = RoleEnum.GameMaster;
			base.AddTargetParameter(false, "Defined target");
		}
		public override void Execute(TriggerBase trigger)
		{
			Character[] targets = base.GetTargets(trigger);
			for (int i = 0; i < targets.Length; i++)
			{
				Character character = targets[i];
				if (!character.IsInFight())
				{
					trigger.ReplyError("{0} is not fighting", new object[]
					{
						character
					});
				}
				else
				{
					Fight fight = character.Fight;
					if (character.IsFighting())
					{
						character.Fighter.LeaveFight();
					}
					if (character.IsSpectator())
					{
						character.Spectator.Leave();
					}
					trigger.ReplyBold("{0} get kicked from fight {1}", new object[]
					{
						character,
						fight.Id
					});
				}
			}
		}
	}
}
