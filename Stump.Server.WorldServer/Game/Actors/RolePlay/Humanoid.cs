using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Types;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Actors.RolePlay
{
	public abstract class Humanoid : NamedActor
	{
		private readonly System.Collections.Generic.List<RolePlayActor> m_followingCharacters = new System.Collections.Generic.List<RolePlayActor>();
		public System.Collections.Generic.IEnumerable<RolePlayActor> FollowingCharacters
		{
			get
			{
				return this.m_followingCharacters;
			}
		}
		public virtual SexTypeEnum Sex
		{
			get;
			protected set;
		}
		public void AddFollowingCharacter(RolePlayActor actor)
		{
			this.m_followingCharacters.Add(actor);
		}
		public void RemoveFollowingCharacter(RolePlayActor actor)
		{
			this.m_followingCharacters.Remove(actor);
		}
		public virtual HumanInformations GetHumanInformations()
		{
			return new HumanInformations(new ActorRestrictionsInformations(), this.Sex == SexTypeEnum.SEX_FEMALE, Enumerable.Empty<HumanOption>());
		}
	}
}
