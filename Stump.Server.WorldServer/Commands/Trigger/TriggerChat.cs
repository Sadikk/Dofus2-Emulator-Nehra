using Stump.Core.Attributes;
using Stump.Core.IO;
using Stump.Server.BaseServer.Commands;
using Stump.Server.BaseServer.Network;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Handlers.Chat;
using System.Drawing;
namespace Stump.Server.WorldServer.Commands.Trigger
{
	public class TriggerChat : GameTrigger
	{
		private static string m_htmlErrorColor = ColorTranslator.ToHtml(System.Drawing.Color.Red);
		private static Color m_errorColor = System.Drawing.Color.Red;
		[Variable(true)]
		public static string HtmlErrorColor
		{
			get
			{
				return TriggerChat.m_htmlErrorColor;
			}
			set
			{
				TriggerChat.m_htmlErrorColor = value;
				TriggerChat.m_errorColor = ColorTranslator.FromHtml(value);
			}
		}
		public static Color ErrorColor
		{
			get
			{
				return TriggerChat.m_errorColor;
			}
			set
			{
				TriggerChat.m_htmlErrorColor = ColorTranslator.ToHtml(value);
				TriggerChat.m_errorColor = value;
			}
		}
		public override ICommandsUser User
		{
			get
			{
				return base.Character;
			}
		}
		public TriggerChat(StringStream args, Character character) : base(args, character)
		{
		}
		public TriggerChat(string args, Character character) : base(args, character)
		{
		}
		public override void Reply(string text)
		{
			ChatHandler.SendChatServerMessage(base.Character.Client, text);
		}
		public override void ReplyError(string message)
		{
			this.Reply(base.Color(base.Bold("(Error)") + " " + message, TriggerChat.ErrorColor));
		}
		public override BaseClient GetSource()
		{
			return (base.Character != null) ? base.Character.Client : null;
		}
	}
}
