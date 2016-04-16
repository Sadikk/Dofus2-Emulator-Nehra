using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Database.Guilds;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using System;
namespace Stump.Server.WorldServer.Game.Guilds
{
	public class GuildMember
	{
		public event System.Action<GuildMember> Connected;
		public event Action<GuildMember, Character> Disconnected;
		public GuildMemberRecord Record
		{
			get;
			private set;
		}
		public int Id
		{
			get
			{
				return this.Record.CharacterId;
			}
		}
		public Character Character
		{
			get;
			private set;
		}
		public bool IsConnected
		{
			get
			{
				return this.Character != null;
			}
		}
		public Guild Guild
		{
			get;
			private set;
		}
		public long GivenExperience
		{
			get
			{
				return this.Record.GivenExperience;
			}
			set
			{
				this.Record.GivenExperience = value;
				this.IsDirty = true;
			}
		}
		public byte GivenPercent
		{
			get
			{
				return this.Record.GivenPercent;
			}
			set
			{
				this.Record.GivenPercent = value;
				this.IsDirty = true;
			}
		}
		public GuildRightsBitEnum Rights
		{
			get
			{
				return this.Record.Rights;
			}
			set
			{
				this.Record.Rights = value;
				this.IsDirty = true;
			}
		}
		public short RankId
		{
			get
			{
				return Convert.ToInt16((this.Record.RankId < 0 || this.Record.RankId > 35) ? 0 : this.Record.RankId);
			}
			set
			{
				this.Record.RankId = value;
				this.IsDirty = true;
			}
		}
		public bool IsBoss
		{
			get
			{
				return this.RankId == 1;
			}
		}
		public string Name
		{
			get
			{
				return this.Record.Name;
			}
		}
		public long Experience
		{
			get
			{
				return this.Record.Experience;
			}
		}
		public PlayableBreedEnum Breed
		{
			get
			{
				return this.Record.Breed;
			}
		}
		public SexTypeEnum Sex
		{
			get
			{
				return this.Record.Sex;
			}
		}
		public AlignmentSideEnum AlignementSide
		{
			get
			{
				return this.Record.AlignementSide;
			}
		}
		public System.DateTime? LastConnection
		{
			get
			{
				return this.Record.LastConnection;
			}
		}
		public bool IsDirty
		{
			get;
			protected set;
		}
		public GuildMember(GuildMemberRecord record)
		{
			this.Record = record;
		}
		public GuildMember(Guild guild, Character character)
		{
			this.Record = new GuildMemberRecord
			{
				CharacterId = character.Id,
				AccountId = character.Account.Id,
				Character = character.Record,
				GivenExperience = 0L,
				GivenPercent = 0,
				RankId = 0,
				GuildId = guild.Id,
				Rights = GuildRightsBitEnum.GUILD_RIGHT_NONE
			};
			this.Guild = guild;
			this.Character = character;
			this.IsDirty = true;
		}
		public Stump.DofusProtocol.Types.GuildMember GetNetworkGuildMember()
		{
			Stump.DofusProtocol.Types.GuildMember result;
			if (this.IsConnected)
			{
                result = new Stump.DofusProtocol.Types.GuildMember((uint)this.Id, Singleton<ExperienceManager>.Instance.GetCharacterLevel(this.Character.Experience), this.Character.Name, (sbyte)this.Character.Breed.Id, this.Character.Sex == SexTypeEnum.SEX_FEMALE, (ushort)this.RankId, (ulong)this.GivenExperience, (sbyte)this.GivenPercent, (uint)this.Rights, Convert.ToSByte(this.IsConnected ? 1 : 0), (sbyte)this.Character.AlignmentSide, (ushort)System.DateTime.Now.Hour, 0, this.Record.AccountId, 0, new PlayerStatus());
			}
			else
			{
                result = new Stump.DofusProtocol.Types.GuildMember((uint)this.Id, Singleton<ExperienceManager>.Instance.GetCharacterLevel(this.Experience), this.Name, (sbyte)this.Breed, this.Sex == SexTypeEnum.SEX_FEMALE, (ushort)this.RankId, (ulong)this.GivenExperience, (sbyte)this.GivenPercent, (uint)this.Rights, Convert.ToSByte(this.IsConnected ? 1 : 0), (sbyte)this.AlignementSide, Convert.ToUInt16(this.LastConnection.HasValue ? ((ushort)(System.DateTime.Now - this.LastConnection.Value).TotalHours) : 0), 0, this.Record.AccountId, 0, new PlayerStatus());
			}
			return result;
		}
		public bool HasRight(GuildRightsBitEnum right)
		{
			return this.Rights == GuildRightsBitEnum.GUILD_RIGHT_BOSS || this.Rights.HasFlag(right);
		}
		public void OnCharacterConnected(Character character)
		{
			if (character.Id != this.Record.CharacterId)
			{
				throw new System.Exception(string.Format("GuildMember.CharacterId ({0}) != characterid ({1})", this.Record.CharacterId, character.Id));
			}
			this.Character = character;
			System.Action<GuildMember> connected = this.Connected;
			if (connected != null)
			{
				connected(this);
			}
		}
		public void OnCharacterDisconnected(Character character)
		{
			this.IsDirty = true;
			this.Character = null;
			Action<GuildMember, Character> disconnected = this.Disconnected;
			if (disconnected != null)
			{
				disconnected(this, character);
			}
		}
		public void AddXP(long experience)
		{
			this.GivenExperience += experience;
			this.Guild.AddXP(experience);
		}
		public void BindGuild(Guild guild)
		{
			if (this.Guild != null)
			{
				throw new System.Exception(string.Format("Guild already bound to GuildMember {0}", this.Id));
			}
			this.Guild = guild;
		}
        public void Save(Stump.ORM.Database database)
		{
			database.Update(this.Record);
			this.IsDirty = false;
		}
	}
}
