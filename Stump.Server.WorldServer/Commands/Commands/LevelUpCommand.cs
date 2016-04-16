using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.WorldServer.Commands.Commands.Patterns;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class LevelUpCommand : TargetCommand
	{
		public LevelUpCommand()
		{
			base.Aliases = new string[]
			{
				"levelup"
			};
			base.RequiredRole = RoleEnum.GameMaster;
			base.Description = "Gives some levels to the target";
			base.AddParameter<short>("amount", "amount", "Amount of levels to add", 1, false, null);
			base.AddTargetParameter(true, "Character who will level up");
		}
		public override void Execute(TriggerBase trigger)
		{
			Character[] targets = base.GetTargets(trigger);
			for (int i = 0; i < targets.Length; i++)
			{
				Character character = targets[i];
				short num = trigger.Get<short>("amount");
				if (num > 0 && num <= 255)
				{
					byte b = (byte)num;
					character.LevelUp(b);
					trigger.Reply("Added " + trigger.Bold("{0}") + " levels to '{1}'.", new object[]
					{
						b,
						character.Name
					});
				}
				else
				{
					if (num < 0 && -num <= 255)
					{
						byte b = (byte)(-(byte)num);
						character.LevelDown(b);
						trigger.Reply("Removed " + trigger.Bold("{0}") + " levels from '{1}'.", new object[]
						{
							b,
							character.Name
						});
					}
					else
					{
						trigger.ReplyError("Invalid level given. Must be greater then -255 and lesser than 255");
					}
				}
			}
		}
	}
}
