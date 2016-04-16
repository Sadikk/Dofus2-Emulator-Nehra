using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.WorldServer.Commands.Commands.Patterns;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Maps;
using Stump.Server.WorldServer.Game.Maps.Cells;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class GoCommand : TargetCommand
	{
		public GoCommand()
		{
			base.Aliases = new string[]
			{
				"go",
				"teleport",
				"tp"
			};
			base.RequiredRole = RoleEnum.GameMaster_Padawan;
			base.Description = "Teleport the target given map id";
			base.AddParameter<Map>("map", "map", "Map destination", null, false, ParametersConverter.MapConverter);
			base.AddTargetParameter(true, "Defined target");
			base.AddParameter<short>("cellid", "cell", "Cell destination", 0, true, null);
		}
		public override void Execute(TriggerBase trigger)
		{
			Character[] targets = base.GetTargets(trigger);
			for (int i = 0; i < targets.Length; i++)
			{
				Character character = targets[i];
				Map map = trigger.Get<Map>("map");
				if (map == null)
				{
					trigger.ReplyError("Map '{0}' doesn't exist", new object[]
					{
						trigger.Get<int>("mapid")
					});
				}
				else
				{
					Cell cell = trigger.IsArgumentDefined("cell") ? map.Cells[(int)trigger.Get<short>("cell")] : character.Cell;
					character.Teleport(new ObjectPosition(map, cell, character.Direction), true);
					trigger.Reply("Teleported.");
				}
			}
		}
	}
}
