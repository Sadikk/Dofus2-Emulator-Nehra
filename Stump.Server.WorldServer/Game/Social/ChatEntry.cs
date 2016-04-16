using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Social
{
	public class ChatEntry
	{
		public string Message
		{
			get;
			set;
		}
		public ChatActivableChannelsEnum Channel
		{
			get;
			set;
		}
		public System.DateTime Date
		{
			get;
			set;
		}
		public Character Destinatory
		{
			get;
			set;
		}
		public ChatEntry(string message, ChatActivableChannelsEnum channel, System.DateTime date)
		{
			this.Message = message;
			this.Channel = channel;
			this.Date = date;
		}
	}
}
