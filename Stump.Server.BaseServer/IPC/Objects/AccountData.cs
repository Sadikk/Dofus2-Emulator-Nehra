using ProtoBuf;
using Stump.DofusProtocol.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Stump.Server.BaseServer.IPC.Objects
{
	[ProtoContract]
	public class AccountData
	{
		private List<PlayableBreedEnum> m_breeds;
		private IList<WorldCharacterData> m_characters = new List<WorldCharacterData>();
		[ProtoMember(1)]
		public int Id
		{
			get;
			set;
		}
		[ProtoMember(2)]
		public string Login
		{
			get;
			set;
		}
		[ProtoMember(3)]
		public string PasswordHash
		{
			get;
			set;
		}
		[ProtoMember(4)]
		public string Nickname
		{
			get;
			set;
		}
		[ProtoMember(5)]
		public int UserGroupId
		{
			get;
			set;
		}
		[ProtoMember(6)]
		public string Ticket
		{
			get;
			set;
		}
		[ProtoMember(7)]
		public string SecretQuestion
		{
			get;
			set;
		}
		[ProtoMember(8)]
		public string SecretAnswer
		{
			get;
			set;
		}
		[ProtoMember(9)]
		public string Lang
		{
			get;
			set;
		}
		[ProtoMember(10)]
		public string Email
		{
			get;
			set;
		}
		[ProtoMember(11)]
		public DateTime CreationDate
		{
			get;
			set;
		}
		[ProtoMember(12)]
        public ushort BreedFlags
		{
			get;
			set;
		}
		public List<PlayableBreedEnum> AvailableBreeds
		{
			get
			{
				List<PlayableBreedEnum> breeds;
				if (this.m_breeds != null)
				{
					breeds = this.m_breeds;
				}
				else
				{
					this.m_breeds = new List<PlayableBreedEnum>();
					this.m_breeds.AddRange(
						from PlayableBreedEnum breed in Enum.GetValues(typeof(PlayableBreedEnum))
						where this.CanUseBreed((int)breed)
						select breed);
					breeds = this.m_breeds;
				}
				return breeds;
			}
			set
			{
				this.BreedFlags = (ushort)value.Aggregate(0, (int current, PlayableBreedEnum breedEnum) => current | 1 << breedEnum - PlayableBreedEnum.Feca);
			}
		}
		[ProtoMember(13)]
		public IList<WorldCharacterData> Characters
		{
			get
			{
				return this.m_characters;
			}
			set
			{
				this.m_characters = value;
			}
		}
		[ProtoMember(14)]
		public int DeletedCharactersCount
		{
			get;
			set;
		}
		[ProtoMember(15)]
		public DateTime LastDeletedCharacterDate
		{
			get;
			set;
		}
		[ProtoMember(16)]
		public DateTime? LastConnection
		{
			get;
			set;
		}
		[ProtoMember(17)]
		public string LastConnectionIp
		{
			get;
			set;
		}
		public bool IsSubscribe
		{
			get
			{
				return this.SubscriptionEndDate > DateTime.Now;
			}
		}
		[ProtoMember(18)]
		public DateTime SubscriptionEndDate
		{
			get;
			set;
		}
		[ProtoMember(19)]
		public bool IsJailed
		{
			get;
			set;
		}
		[ProtoMember(24)]
		public bool IsBanned
		{
			get;
			set;
		}
		[ProtoMember(20)]
		public DateTime? BanEndDate
		{
			get;
			set;
		}
		[ProtoMember(21)]
		public string BanReason
		{
			get;
			set;
		}
		[ProtoMember(22)]
		public uint Tokens
		{
			get;
			set;
		}
		[ProtoMember(23)]
		public DateTime? LastVote
		{
			get;
			set;
		}
		public bool CanUseBreed(int breedId)
		{
            bool result;
            if (breedId <= 0)
            {
                result = false;
            }
            else
            {
                var num = (ushort)(1 << breedId - 1);
                result = (this.BreedFlags & num) == num;
            }
            return result;
		}
	}
}
