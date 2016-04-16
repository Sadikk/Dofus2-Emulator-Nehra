using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer;
using Stump.Server.BaseServer.Commands;
using Stump.Server.WorldServer.Game;
using System;
namespace Stump.Server.WorldServer.Commands.Commands
{
	public class SaveWorldCommand : SubCommand
	{
		public SaveWorldCommand()
		{
			base.Aliases = new string[]
			{
				"world"
			};
			base.Description = "Save world";
			base.RequiredRole = RoleEnum.GameMaster;
			base.ParentCommand = typeof(SaveCommand);
		}
		public override void Execute(TriggerBase trigger)
		{
			ServerBase<WorldServer>.Instance.IOTaskPool.AddMessage(new Action(Singleton<World>.Instance.Save));
		}
	}
}
