using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Database.Accounts;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using System;
namespace Stump.Server.WorldServer.Game.Social
{
	public class Friend
	{
		public WorldAccount Account
		{
			get;
			private set;
		}
		public Character Character
		{
			get;
			private set;
		}
		public AccountRelation Relation
		{
			get;
			private set;
		}
		public Friend(AccountRelation relation, WorldAccount account)
		{
			this.Relation = relation;
			this.Account = account;
		}
		public Friend(AccountRelation relation, WorldAccount account, Character character)
		{
			this.Relation = relation;
			this.Account = account;
			this.Character = character;
		}
		public void SetOnline(Character character)
		{
			if (character.Client.WorldAccount.Id == this.Account.Id)
			{
				this.Character = character;
			}
		}
		public void SetOffline()
		{
			this.Character = null;
		}
		public bool IsOnline()
		{
			return this.Character != null;
		}
		public FriendInformations GetFriendInformations()
		{
			FriendInformations result;
			if (this.IsOnline())
			{
                result = new FriendOnlineInformations(this.Account.Id, this.Account.Nickname, Convert.ToSByte(this.Character.IsFighting() ? 2 : 1), (ushort)this.Account.LastConnectionTimeStamp, 0, (uint)Character.Id, this.Character.Name, (byte)this.Character.Level, (sbyte)this.Character.AlignmentSide, (sbyte)this.Character.Breed.Id, this.Character.Sex == SexTypeEnum.SEX_FEMALE, (this.Character.GuildMember == null) ? new BasicGuildInformations(0, "") : this.Character.GuildMember.Guild.GetBasicGuildInformations(), -1, new PlayerStatus());
			}
			else
			{
                result = new FriendInformations(this.Account.Id, this.Account.Nickname, 0, (ushort)this.Account.LastConnectionTimeStamp, 0);
			}
			return result;
		}
	}
}
