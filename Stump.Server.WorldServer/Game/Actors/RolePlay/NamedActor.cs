using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Game.Actors.Interfaces;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Handlers.Chat;

namespace Stump.Server.WorldServer.Game.Actors.RolePlay
{
	public abstract class NamedActor : RolePlayActor, INamedActor
	{
		public virtual string Name
		{
			get;
			protected set;
		}
		public override GameContextActorInformations GetGameContextActorInformations(Character character)
		{
			return new GameRolePlayNamedActorInformations(this.Id, this.Look.GetEntityLook(), this.GetEntityDispositionInformations(), this.Name);
		}
		public void Say(string msg)
		{
			ChatHandler.SendChatServerMessage(this.CharacterContainer.Clients, this, ChatActivableChannelsEnum.CHANNEL_GLOBAL, msg);
		}
	}
}
