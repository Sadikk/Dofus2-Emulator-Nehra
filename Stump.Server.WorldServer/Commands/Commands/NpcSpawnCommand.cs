using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.WorldServer.Commands.Trigger;
using Stump.Server.WorldServer.Database.Npcs;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Npcs;
using Stump.Server.WorldServer.Game.Maps;
using Stump.Server.WorldServer.Game.Maps.Cells;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class NpcSpawnCommand : SubCommand
	{
		public NpcSpawnCommand()
		{
			base.Aliases = new string[]
			{
				"spawn"
			};
			base.RequiredRole = RoleEnum.GameMaster;
			base.Description = "Spawn a npc on the current location";
			base.ParentCommand = typeof(NpcsCommands);
			base.AddParameter<NpcTemplate>("npc", "npc", "Npc Template id", null, false, ParametersConverter.NpcTemplateConverter);
			base.AddParameter<Map>("map", "map", "Map id", null, true, ParametersConverter.MapConverter);
			base.AddParameter<short>("cell", "cell", "Cell id", 0, true, null);
			base.AddParameter<DirectionsEnum>("direction", "dir", "Direction", DirectionsEnum.DIRECTION_EAST, true, ParametersConverter.GetEnumConverter<DirectionsEnum>());
		}
		public override void Execute(TriggerBase trigger)
		{
			NpcTemplate npcTemplate = trigger.Get<NpcTemplate>("npc");
			ObjectPosition objectPosition = null;
			if (trigger.IsArgumentDefined("map") && trigger.IsArgumentDefined("cell") && trigger.IsArgumentDefined("direction"))
			{
				Map map = trigger.Get<Map>("map");
				short cellId = trigger.Get<short>("cell");
				DirectionsEnum direction = trigger.Get<DirectionsEnum>("direction");
				objectPosition = new ObjectPosition(map, cellId, direction);
			}
			else
			{
				if (trigger is GameTrigger)
				{
					objectPosition = (trigger as GameTrigger).Character.Position;
				}
			}
			if (objectPosition == null)
			{
				trigger.ReplyError("Position of npc is not defined");
			}
			else
			{
				Npc npc = objectPosition.Map.SpawnNpc(npcTemplate, objectPosition, npcTemplate.Look);
				trigger.Reply("Npc {0} spawned", new object[]
				{
					npc.Id
				});
			}
		}
	}
}
