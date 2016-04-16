using Stump.Core.Attributes;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Social
{
	public class ChatHistory
	{
		[Variable]
		public static int MaxChatEntries = 50;
		private System.Collections.Generic.List<ChatEntry> m_entries = new System.Collections.Generic.List<ChatEntry>();
		public Character Character
		{
			get;
			private set;
		}
		public System.Collections.ObjectModel.ReadOnlyCollection<ChatEntry> Entries
		{
			get
			{
				return this.m_entries.AsReadOnly();
			}
		}
		public ChatHistory(Character character)
		{
			this.Character = character;
		}
		public bool RegisterAndCheckFlood(ChatEntry entry)
		{
			this.m_entries.Insert(0, entry);
			while (this.m_entries.Count >= ChatHistory.MaxChatEntries)
			{
				this.m_entries.Remove(this.m_entries.Last<ChatEntry>());
			}
			bool result;
			if (this.Character.IsGameMaster())
			{
				result = true;
			}
			else
			{
				if (this.m_entries.Count > 1 && (this.m_entries[0].Date - this.m_entries[1].Date).TotalMilliseconds < (double)ChatManager.AntiFloodTimeBetweenTwoMessages)
				{
					result = false;
				}
				else
				{
					if (this.m_entries.Count > 1 && ChatManager.IsGlobalChannel(entry.Channel))
					{
						int num = this.m_entries.FindIndex(1, (ChatEntry x) => x.Channel == entry.Channel);
						if (num >= 0 && (entry.Date - this.m_entries[num].Date).TotalSeconds < (double)ChatManager.AntiFloodTimeBetweenTwoGlobalMessages)
						{
							this.Character.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_MESSAGE, 115, new object[]
							{
								ChatManager.AntiFloodTimeBetweenTwoGlobalMessages - (int)(entry.Date - this.m_entries[num].Date).TotalSeconds
							});
							result = false;
							return result;
						}
					}
					bool arg_1E2_0;
					if (this.m_entries.Count >= ChatManager.AntiFloodAllowedMessages)
					{
						arg_1E2_0 = !this.m_entries.Take(ChatManager.AntiFloodAllowedMessages).All((ChatEntry x) => (System.DateTime.Now - x.Date).TotalSeconds < (double)ChatManager.AntiFloodAllowedMessagesResetTime);
					}
					else
					{
						arg_1E2_0 = true;
					}
					if (!arg_1E2_0)
					{
						this.Character.Mute(System.TimeSpan.FromSeconds((double)ChatManager.AntiFloodMuteTime));
						result = false;
					}
					else
					{
						result = true;
					}
				}
			}
			return result;
		}
	}
}
