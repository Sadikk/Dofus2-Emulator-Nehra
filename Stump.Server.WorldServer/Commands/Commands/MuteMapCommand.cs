using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.WorldServer.Commands.Trigger;
using Stump.Server.WorldServer.Game.Maps;
using Stump.Server.WorldServer.Handlers.Basic;
using System.Drawing;
namespace Stump.Server.WorldServer.Commands.Commands
{
	public class MuteMapCommand : CommandBase
	{
		public MuteMapCommand()
		{
			base.Aliases = new string[]
			{
				"mutemap"
			};
			base.RequiredRole = RoleEnum.Moderator;
		}
		public override void Execute(TriggerBase trigger)
		{
			Map map = ((GameTrigger)trigger).Character.Map;
			string arg = map.ToggleMute() ? "La map est maintenant réduite au silence !" : "La map n'est plus réduite au silence !";
			BasicHandler.SendTextInformationMessage(map.Clients, TextInformationTypeEnum.TEXT_INFORMATION_MESSAGE, 0, new string[]
			{
				string.Format("<font color=\"#{0}\">{1}</font>", Color.Red.ToArgb().ToString("X"), arg)
			});
		}
	}
}
