using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Game.Actors.Interfaces;
using Stump.Server.WorldServer.Game.Fights.Teams;
using Stump.Server.WorldServer.Handlers.Chat;

namespace Stump.Server.WorldServer.Game.Actors.Fight
{
	public abstract class NamedFighter : FightActor, INamedActor
	{
		public abstract string Name
		{
			get;
		}
		protected NamedFighter(FightTeam team) : base(team)
		{
		}
		public void Say(string msg)
		{
			ChatHandler.SendChatServerMessage(base.Fight.Clients, this, ChatActivableChannelsEnum.CHANNEL_GLOBAL, msg);
		}
		public override string GetMapRunningFighterName()
		{
			return this.Name;
		}
	}
}
