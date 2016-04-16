using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.WorldServer.Commands.Trigger;
using Stump.Server.WorldServer.Game.Fights;
using Stump.Server.WorldServer.Game.Maps;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class ListFightCommand : SubCommand
	{
		public ListFightCommand()
		{
			base.Aliases = new string[]
			{
				"list"
			};
			base.Description = "List fights on the map";
			base.ParentCommand = typeof(FightCommands);
			base.RequiredRole = RoleEnum.GameMaster;
			base.AddParameter<Map>("map", "m", "List fights of that map", null, true, ParametersConverter.MapConverter);
		}
		public override void Execute(TriggerBase trigger)
		{
			Map map;
			if (!trigger.IsArgumentDefined("map"))
			{
				if (!(trigger is GameTrigger))
				{
					trigger.ReplyError("Map not defined");
					return;
				}
				map = (trigger as GameTrigger).Character.Map;
			}
			else
			{
				map = trigger.Get<Map>("map");
			}
			foreach (Fight current in map.Fights)
			{
				trigger.ReplyBold(" - {0} (red:{1}, blue{2}){3}", new object[]
				{
					current.Id,
					current.BlueTeam.Fighters.Count,
					current.RedTeam.Fighters.Count,
					(current.State == FightState.Placement) ? " Placement phase" : string.Empty
				});
			}
		}
	}
}
