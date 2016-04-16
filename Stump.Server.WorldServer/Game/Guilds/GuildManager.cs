using Stump.Core.Pool;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Types;
using Stump.Server.BaseServer;
using Stump.Server.BaseServer.Database;
using Stump.Server.BaseServer.Initialization;
using Stump.Server.WorldServer.Database;
using Stump.Server.WorldServer.Database.Alliances;
using Stump.Server.WorldServer.Database.Characters;
using Stump.Server.WorldServer.Database.Guilds;
using Stump.Server.WorldServer.Database.Symbols;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Items;
using Stump.Server.WorldServer.Game.Items.Player;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stump.Server.WorldServer.Game.Guilds
{
	public class GuildManager : DataManager<GuildManager>, ISaveable
	{
        public const int GUILDALOGEMME_ID = 1575;

		private UniqueIdProvider m_idProvider;
		private System.Collections.Generic.Dictionary<int, Guild> m_guilds;
		private System.Collections.Generic.Dictionary<int, EmblemRecord> m_emblems;
		private System.Collections.Generic.Dictionary<int, GuildMember> m_guildsMembers;
		private readonly Stack<Guild> m_guildsToDelete = new Stack<Guild>();
		private readonly object m_lock = new object();
		[Initialization(InitializationPass.Sixth)]
		public override void Initialize()
		{
			this.m_emblems = base.Database.Query<EmblemRecord>(EmblemRelator.FetchQuery, new object[0]).ToDictionary((EmblemRecord x) => x.Id);
			this.m_guilds = (
				from x in base.Database.Query<GuildRecord>(GuildRelator.FetchQuery, new object[0]).ToList<GuildRecord>()
				select new Guild(x, this.FindGuildMembers(x.Id))).ToDictionary((Guild x) => x.Id);
			this.m_guildsMembers = this.m_guilds.Values.SelectMany((Guild x) => x.Members).ToDictionary((GuildMember x) => x.Id);
			UniqueIdProvider arg_12F_1;
			if (!this.m_guilds.Any<System.Collections.Generic.KeyValuePair<int, Guild>>())
			{
				arg_12F_1 = new UniqueIdProvider(1);
			}
			else
			{
				arg_12F_1 = new UniqueIdProvider((
					from x in this.m_guilds
					select x.Value.Id).Max());
			}
			this.m_idProvider = arg_12F_1;
			Singleton<World>.Instance.RegisterSaveableInstance(this);
		}
		public bool DoesNameExist(string name)
		{
			return this.m_guilds.Any((System.Collections.Generic.KeyValuePair<int, Guild> x) => string.Equals(x.Value.Name, name, System.StringComparison.CurrentCultureIgnoreCase));
		}
		public bool DoesEmblemExist(Stump.DofusProtocol.Types.GuildEmblem emblem)
		{
			return this.m_guilds.Any((System.Collections.Generic.KeyValuePair<int, Guild> x) => x.Value.Emblem.DoesEmblemMatch(emblem));
		}
		public bool DoesEmblemExist(GuildEmblem emblem)
		{
			return this.m_guilds.Any((System.Collections.Generic.KeyValuePair<int, Guild> x) => x.Value.Emblem.DoesEmblemMatch(emblem));
		}
		public GuildMember[] FindGuildMembers(int guildId)
		{
			return (
				from x in base.Database.Fetch<GuildMemberRecord, CharacterRecord, GuildMemberRecord>(new Func<GuildMemberRecord, CharacterRecord, GuildMemberRecord>(new GuildMemberRelator().Map), string.Format(GuildMemberRelator.FetchByGuildId, guildId), new object[0])
				select new GuildMember(x)).ToArray<GuildMember>();
		}
		public Guild TryGetGuild(int id)
		{
			Guild result;
			lock (this.m_lock)
			{
				Guild guild;
				result = (this.m_guilds.TryGetValue(id, out guild) ? guild : null);
			}
			return result;
		}
		public Guild TryGetGuild(string name)
		{
			Guild value;
			lock (this.m_lock)
			{
				value = this.m_guilds.FirstOrDefault((System.Collections.Generic.KeyValuePair<int, Guild> x) => string.Equals(x.Value.Name, name, System.StringComparison.CurrentCultureIgnoreCase)).Value;
			}
			return value;
		}
		public EmblemRecord TryGetEmblem(int id)
		{
			EmblemRecord emblemRecord;
			return this.m_emblems.TryGetValue(id, out emblemRecord) ? emblemRecord : null;
		}
		public GuildMember TryGetGuildMember(int characterId)
		{
			GuildMember result;
			lock (this.m_lock)
			{
				GuildMember guildMember;
				result = (this.m_guildsMembers.TryGetValue(characterId, out guildMember) ? guildMember : null);
			}
			return result;
		}
		public Guild CreateGuild(string name)
		{
			Guild result;
			lock (this.m_lock)
			{
				Guild guild = new Guild(this.m_idProvider.Pop(), name);
				this.m_guilds.Add(guild.Id, guild);
				result = guild;
			}
			return result;
		}
		public GuildCreationResultEnum CreateGuild(Character character, string name, Stump.DofusProtocol.Types.GuildEmblem emblem)
		{
			GuildCreationResultEnum result;
			if (this.DoesNameExist(name))
			{
				result = GuildCreationResultEnum.GUILD_CREATE_ERROR_NAME_ALREADY_EXISTS;
			}
			else
			{
				if (this.DoesEmblemExist(emblem))
				{
					result = GuildCreationResultEnum.GUILD_CREATE_ERROR_EMBLEM_ALREADY_EXISTS;
				}
				else
				{
                    BasePlayerItem basePlayerItem = character.Inventory.TryGetItem(Singleton<ItemManager>.Instance.TryGetTemplate(GuildManager.GUILDALOGEMME_ID));
					if (basePlayerItem == null)
					{
						result = GuildCreationResultEnum.GUILD_CREATE_ERROR_REQUIREMENT_UNMET;
					}
					else
					{
						character.Inventory.RemoveItem(basePlayerItem, 1u, true);
						character.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_MESSAGE, 22, new object[]
						{
							1,
							basePlayerItem.Template.Id
						});
						Guild guild = this.CreateGuild(name);
						if (guild == null)
						{
							result = GuildCreationResultEnum.GUILD_CREATE_ERROR_CANCEL;
						}
						else
						{
							guild.Emblem.ChangeEmblem(emblem);
							GuildMember guildMember;
							if (!guild.TryAddMember(character, out guildMember))
							{
								this.DeleteGuild(guild);
								result = GuildCreationResultEnum.GUILD_CREATE_ERROR_CANCEL;
							}
							else
							{
								character.GuildMember = guildMember;
								character.RefreshActor();
								result = GuildCreationResultEnum.GUILD_CREATE_OK;
							}
						}
					}
				}
			}
			return result;
		}
		public bool DeleteGuild(Guild guild)
		{
			bool result;
			lock (this.m_lock)
			{
				guild.RemoveTaxCollectors();
				this.m_guilds.Remove(guild.Id);
				this.m_guildsToDelete.Push(guild);
				result = true;
			}
			return result;
		}
		public void RegisterGuildMember(GuildMember member)
		{
			ServerBase<WorldServer>.Instance.IOTaskPool.AddMessage(delegate
			{
				this.Database.Insert(member.Record);
			});
			lock (this.m_lock)
			{
				this.m_guildsMembers.Add(member.Id, member);
			}
		}
		public bool DeleteGuildMember(GuildMember member)
		{
			ServerBase<WorldServer>.Instance.IOTaskPool.AddMessage(delegate
			{
				this.Database.Delete(member.Record);
			});
			bool result;
			lock (this.m_lock)
			{
				this.m_guildsMembers.Remove(member.Id);
				result = true;
			}
			return result;
		}

        public IEnumerable<Guild> GetGuildsByAlliance(AllianceRecord alliance)
        {
            lock (this.m_lock)
            {
                return this.m_guilds.Values
                    .Where(entry => entry.Record.AllianceId == alliance.Id);
            }
        }

        public IEnumerable<GuildInformations> GetGuildsListInformations()
        {
            lock (this.m_lock)
            {
                return this.m_guilds.Values
                    .Select(entry => entry.GetGuildInformations());
            }
        }

        public IEnumerable<GuildVersatileInformations> GetGuildsVersatileInformations()
        {
            lock (this.m_lock)
            {
                return this.m_guilds.Values
                    .Select(entry => entry.GetGuildVersatileInformations());
            }
        }

		public void Save()
		{
			lock (this.m_lock)
			{
				Guild guild2;
				using (System.Collections.Generic.IEnumerator<Guild> enumerator = (
					from guild in this.m_guilds.Values
					where guild.IsDirty
					select guild).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						guild2 = enumerator.Current;
						guild2.Save(base.Database);
					}
					goto IL_8F;
				}
				IL_71:
				guild2 = this.m_guildsToDelete.Pop();
				base.Database.Delete(guild2.Record);
				IL_8F:
				if (this.m_guildsToDelete.Count > 0)
				{
					goto IL_71;
				}
			}
		}


    }
}
