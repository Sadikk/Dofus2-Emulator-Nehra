using Stump.Server.WorldServer.Database.Characters;

namespace Stump.Server.WorldServer.Database.Guilds
{
	public class GuildMemberRelator
	{
		public static string FetchQuery = "SELECT * FROM guild_members";
		public static string FetchByGuildId = "SELECT * FROM guild_members gm LEFT JOIN (SELECT Id, Name, Experience, Breed, Sex, AlignmentSide, LastUsage FROM characters) ch ON ch.Id = gm.CharacterId WHERE GuildId={0}";
		public static string FindByCharacterId = "SELECT * FROM guild_members gm LEFT JOIN (SELECT Id, Name, Experience, Breed, Sex, AlignmentSide, LastUsage FROM characters) ch ON ch.Id = gm.CharacterId WHERE CharacterId={0}";
		private GuildMemberRecord m_current;
		public GuildMemberRecord Map(GuildMemberRecord record, CharacterRecord character)
		{
			GuildMemberRecord result;
			if (record == null)
			{
				result = this.m_current;
			}
			else
			{
				if (this.m_current != null && this.m_current.CharacterId == record.CharacterId)
				{
					this.m_current.Character = character;
					result = null;
				}
				else
				{
					GuildMemberRecord current = this.m_current;
					this.m_current = record;
					this.m_current.Character = character;
					result = current;
				}
			}
			return result;
		}
	}
}
