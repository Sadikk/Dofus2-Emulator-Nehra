using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.WorldServer.Commands.Commands.Patterns;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Maps;
using Stump.Server.WorldServer.Game.Maps.Cells;
using System.Drawing;
using System.Linq;
namespace Stump.Server.WorldServer.Commands.Commands
{
	public class GoPosCommand : TargetCommand
	{
		public GoPosCommand()
		{
			base.Aliases = new string[]
			{
				"gopos",
				"teleporto"
			};
			base.RequiredRole = RoleEnum.GameMaster_Padawan;
			base.Description = "Teleport the target to the given map position (x/y)";
			base.AddParameter<int>("x", "", "", 0, false, null);
			base.AddParameter<int>("y", "", "", 0, false, null);
			base.AddTargetParameter(true, "Defined target");
			base.AddParameter<short>("cellid", "cell", "Cell destination", 0, true, null);
			base.AddParameter<SuperArea>("superarea", "area", "Super area containing the map (e.g 0 is continent, 3 is incarnam)", null, true, ParametersConverter.SuperAreaConverter);
			base.AddParameter<int>("outdoor", "out", "", 0, true, null);
		}
		public override void Execute(TriggerBase trigger)
		{
			Point position = new Point(trigger.Get<int>("x"), trigger.Get<int>("y"));
			bool flag = trigger.IsArgumentDefined("outdoor");
			int index = trigger.Get<int>("outdoor");
			Character[] targets = base.GetTargets(trigger);
			for (int i = 0; i < targets.Length; i++)
			{
				Character character = targets[i];
				Map map = character.Map;
				Map map2;
				if (trigger.IsArgumentDefined("superarea"))
				{
					SuperArea superArea = trigger.Get<SuperArea>("superarea");
					map2 = (flag ? superArea.GetMaps(position, flag).FirstOrDefault<Map>() : superArea.GetMaps(position).ElementAtOrDefault(index));
				}
				else
				{
					map2 = (flag ? Singleton<World>.Instance.GetMaps(map, position.X, position.Y).ElementAtOrDefault(index) : Singleton<World>.Instance.GetMaps(map, position.X, position.Y, flag).FirstOrDefault<Map>());
				}
				if (map2 == null)
				{
					trigger.ReplyError("Map x:{0} y:{1} not found", new object[]
					{
						position.X,
						position.Y
					});
				}
				else
				{
					Cell cell = trigger.IsArgumentDefined("cell") ? map2.Cells[(int)trigger.Get<short>("cell")] : character.Cell;
					character.Teleport(new ObjectPosition(map2, cell, character.Direction), true);
					trigger.Reply("Teleported to {0} {1} ({2}).", new object[]
					{
						position.X,
						position.Y,
						map2.Id
					});
				}
			}
		}
	}
}
