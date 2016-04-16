using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.BaseServer.Commands.Commands;
using Stump.Server.WorldServer.Commands.Trigger;
using Stump.Server.WorldServer.Game.Actors.RolePlay;
using Stump.Server.WorldServer.Game.Maps;
using Stump.Server.WorldServer.Game.Maps.Spawns;
using System.Linq;
namespace Stump.Server.WorldServer.Commands.Commands
{
	public class InfoMapCommand : SubCommand
	{
		public InfoMapCommand()
		{
			base.Aliases = new string[]
			{
				"map"
			};
			base.RequiredRole = RoleEnum.GameMaster;
			base.Description = "Give informations about a map";
			base.ParentCommand = typeof(InfoCommand);
			base.AddParameter<Map>("map", "map", "Target map", null, true, ParametersConverter.MapConverter);
		}
		public override void Execute(TriggerBase trigger)
		{
			Map map = null;
			if (trigger.IsArgumentDefined("map"))
			{
				map = trigger.Get<Map>("map");
			}
			else
			{
				if (trigger is GameTrigger)
				{
					map = (trigger as GameTrigger).Character.Map;
				}
			}
			if (map == null)
			{
				trigger.ReplyError("Map not defined");
			}
			else
			{
				trigger.ReplyBold("Map {0} (relative : {1})", new object[]
				{
					map.Id,
					map.RelativeId
				});
				trigger.ReplyBold("X:{0}, Y:{1}", new object[]
				{
					map.Position.X,
					map.Position.Y
				});
				trigger.ReplyBold("SubArea:{0}, Area:{1}, SuperArea:{2}", new object[]
				{
					map.SubArea.Id,
					map.Area.Id,
					map.SuperArea.Id
				});
				RolePlayActor[] array = map.GetActors<RolePlayActor>().ToArray<RolePlayActor>();
				trigger.ReplyBold("Actors ({0}) :", new object[]
				{
					array.Length
				});
				RolePlayActor[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					RolePlayActor rolePlayActor = array2[i];
					trigger.ReplyBold("- {0} : {1}", new object[]
					{
						rolePlayActor.GetType().Name,
						rolePlayActor
					});
				}
				trigger.ReplyBold("SpawningPools ({0}) :", new object[]
				{
					map.SpawningPools.Count
				});
				foreach (SpawningPoolBase current in map.SpawningPools)
				{
					trigger.ReplyBold("- {0} : State : {1}, Next : {2}s", new object[]
					{
						current.GetType().Name,
						current.State,
						current.RemainingTime / 1000
					});
				}
			}
		}
	}
}
