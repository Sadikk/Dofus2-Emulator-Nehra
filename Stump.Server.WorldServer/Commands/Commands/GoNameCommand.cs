using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.WorldServer.Commands.Trigger;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class GoNameCommand : CommandBase
	{
		public GoNameCommand()
		{
			base.Aliases = new string[]
			{
				"goname",
				"tptoname"
			};
			base.RequiredRole = RoleEnum.Moderator;
			base.Description = "Teleport to the target";
			base.AddParameter<Character>("to", "to", "The character to rejoin", null, false, ParametersConverter.CharacterConverter);
			base.AddParameter<Character>("from", "from", "The character that teleport", null, true, ParametersConverter.CharacterConverter);
		}
		public override void Execute(TriggerBase trigger)
		{
			Character character = trigger.Get<Character>("to");
			Character character2;
			if (trigger.IsArgumentDefined("from"))
			{
				character2 = trigger.Get<Character>("from");
			}
			else
			{
				if (!(trigger is GameTrigger))
				{
					throw new System.Exception("Character to teleport not defined !");
				}
				character2 = (trigger as GameTrigger).Character;
			}
			character2.Teleport(character.Position, true);
		}
	}
}
