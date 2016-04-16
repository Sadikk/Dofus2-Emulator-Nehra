using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.BaseServer.Commands.Commands;
using Stump.Server.WorldServer.Commands.Trigger;
using Stump.Server.WorldServer.Game.Maps;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class InfoAreaCommand : SubCommand
	{
		public InfoAreaCommand()
		{
			base.Aliases = new string[]
			{
				"area"
			};
			base.RequiredRole = RoleEnum.GameMaster;
			base.Description = "Give informations about an area";
			base.ParentCommand = typeof(InfoCommand);
			base.AddParameter<Area>("area", "area", "Target area", null, true, ParametersConverter.AreaConverter);
		}
		public override void Execute(TriggerBase trigger)
		{
			Area area = null;
			if (trigger.IsArgumentDefined("area"))
			{
				area = trigger.Get<Area>("area");
			}
			else
			{
				if (trigger is GameTrigger)
				{
					area = (trigger as GameTrigger).Character.Area;
				}
			}
			if (area == null)
			{
				trigger.ReplyError("Area not defined");
			}
			else
			{
				trigger.ReplyBold("Area {0} ({1})", new object[]
				{
					area.Name,
					area.Id
				});
				trigger.ReplyBold("Enabled : {0}", new object[]
				{
					area.IsRunning
				});
				trigger.ReplyBold("Objects : {0}", new object[]
				{
					area.ObjectCount
				});
				trigger.ReplyBold("Timers : {0}", new object[]
				{
					area.TimersCount
				});
				trigger.ReplyBold("Update interval : {0}ms", new object[]
				{
					area.UpdateDelay
				});
				trigger.ReplyBold("AvgUpdateTime : {0}ms", new object[]
				{
					area.AverageUpdateTime
				});
				trigger.ReplyBold("LastUpdate : {0}", new object[]
				{
					area.LastUpdateTime
				});
				trigger.ReplyBold("Is Updating : {0}", new object[]
				{
					area.IsUpdating
				});
				trigger.ReplyBold("Is Disposed : {0}", new object[]
				{
					area.IsDisposed
				});
				trigger.ReplyBold("Current Thread : {0}", new object[]
				{
					area.CurrentThreadId
				});
			}
		}
	}
}
