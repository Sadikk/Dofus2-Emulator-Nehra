using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.WorldServer.Commands.Trigger;
using Stump.Server.WorldServer.Game.Maps;
using Stump.Server.WorldServer.Game.Maps.Spawns;
using System.Linq;
namespace Stump.Server.WorldServer.Commands.Commands
{
	public class MonsterSpawnNextCommand : SubCommand
	{
		public MonsterSpawnNextCommand()
		{
			base.Aliases = new string[]
			{
				"spawnnext"
			};
			base.RequiredRole = RoleEnum.GameMaster;
			base.Description = "Spawn the next monster of the spawning pool";
			base.ParentCommand = typeof(MonsterCommands);
			base.AddParameter<Map>("map", "m", "Map", null, true, ParametersConverter.MapConverter);
			base.AddParameter<SubArea>("subarea", "subarea", "If defined spawn a monster on each map", null, true, ParametersConverter.SubAreaConverter);
		}
		public override void Execute(TriggerBase trigger)
		{
			Map map = null;
			SubArea subArea = null;
			if (!trigger.IsArgumentDefined("map") && !trigger.IsArgumentDefined("subarea"))
			{
				if (!(trigger is GameTrigger))
				{
					trigger.ReplyError("You have to define a map or a subarea if your are not ingame");
					return;
				}
				map = (trigger as GameTrigger).Character.Map;
			}
			else
			{
				if (trigger.IsArgumentDefined("map"))
				{
					map = trigger.Get<Map>("map");
				}
				else
				{
					if (trigger.IsArgumentDefined("subarea"))
					{
						subArea = trigger.Get<SubArea>("subarea");
					}
				}
			}
			if (map != null)
			{
				ClassicalSpawningPool classicalSpawningPool = map.SpawningPools.OfType<ClassicalSpawningPool>().FirstOrDefault<ClassicalSpawningPool>();
				if (classicalSpawningPool == null)
				{
					trigger.ReplyError("No spawning pool on the map");
				}
				else
				{
					if (classicalSpawningPool.SpawnNextGroup())
					{
						trigger.Reply("Next group spawned");
					}
					else
					{
						trigger.ReplyError("Spawns limit reached");
					}
				}
			}
			else
			{
				if (subArea != null)
				{
					int num = (
						from subMap in subArea.Maps
						select subMap.SpawningPools.OfType<ClassicalSpawningPool>().FirstOrDefault<ClassicalSpawningPool>() into pool
						where pool != null
						select pool).Count((ClassicalSpawningPool pool) => pool.SpawnNextGroup());
					trigger.Reply("{0} groups spawned", new object[]
					{
						num
					});
				}
			}
		}
	}
}
