using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Database.Accounts;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Social
{
	public class Ignored
	{
		public bool Session
		{
			get;
			private set;
		}
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
		public Ignored(AccountRelation relation, WorldAccount account, bool session)
		{
			this.Relation = relation;
			this.Session = session;
			this.Account = account;
		}
		public Ignored(AccountRelation relation, WorldAccount account, bool session, Character character)
		{
			this.Relation = relation;
			this.Session = session;
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
		public IgnoredInformations GetIgnoredInformations()
		{
			IgnoredInformations result;
			if (this.IsOnline())
			{
				result = new IgnoredOnlineInformations(this.Account.Id, this.Account.Nickname, (uint)Character.Id, this.Character.Name, (sbyte)this.Character.Breed.Id, this.Character.Sex == SexTypeEnum.SEX_FEMALE);
			}
			else
			{
				result = new IgnoredInformations(this.Account.Id, this.Account.Nickname);
			}
			return result;
		}
	}
}
