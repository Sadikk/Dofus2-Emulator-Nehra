using Stump.Core.Attributes;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.WorldServer.Commands.Trigger;
using Stump.Server.WorldServer.Game;
using System.Drawing;
namespace Stump.Server.WorldServer.Commands.Commands
{
	public class AnnounceCommand : CommandBase
	{
		[Variable(true)]
		public static string AnnounceColor = ColorTranslator.ToHtml(Color.Red);
		public AnnounceCommand()
		{
			base.Aliases = new string[]
			{
				"announce",
				"a"
			};
			base.Description = "Display an announce to all players";
			base.RequiredRole = RoleEnum.GameMaster;
			base.AddParameter<string>("message", "msg", "The announce", null, false, null);
		}
		public override void Execute(TriggerBase trigger)
		{
			Color color = ColorTranslator.FromHtml(AnnounceCommand.AnnounceColor);
			string text = trigger.Get<string>("msg");
			string announce = (trigger is GameTrigger) ? string.Format("(ANNOUNCE) {0} : {1}", ((GameTrigger)trigger).Character.Name, text) : string.Format("(ANNOUNCE) {0}", text);
			Singleton<World>.Instance.SendAnnounce(announce, color);
		}
	}
}
