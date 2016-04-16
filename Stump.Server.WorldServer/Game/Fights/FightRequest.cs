using Stump.Core.Reflection;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Dialogs;
using Stump.Server.WorldServer.Handlers.Context.RolePlay;

namespace Stump.Server.WorldServer.Game.Fights
{
	public class FightRequest : RequestBox
	{
		public FightRequest(Character source, Character target) : base(source, target)
		{
		}
		protected override void OnOpen()
		{
			ContextRoleplayHandler.SendGameRolePlayPlayerFightFriendlyRequestedMessage(base.Source.Client, base.Target, base.Source, base.Target);
			ContextRoleplayHandler.SendGameRolePlayPlayerFightFriendlyRequestedMessage(base.Target.Client, base.Source, base.Source, base.Target);
		}
		protected override void OnAccept()
		{
			ContextRoleplayHandler.SendGameRolePlayPlayerFightFriendlyAnsweredMessage(base.Source.Client, base.Target, base.Source, base.Target, true);
			Fight fight = Singleton<FightManager>.Instance.CreateDuel(base.Source.Map);
			fight.BlueTeam.AddFighter(base.Source.CreateFighter(fight.BlueTeam));
			fight.RedTeam.AddFighter(base.Target.CreateFighter(fight.RedTeam));
			fight.StartPlacement();
		}
		protected override void OnDeny()
		{
			ContextRoleplayHandler.SendGameRolePlayPlayerFightFriendlyAnsweredMessage(base.Source.Client, base.Target, base.Source, base.Target, false);
		}
		protected override void OnCancel()
		{
			ContextRoleplayHandler.SendGameRolePlayPlayerFightFriendlyAnsweredMessage(base.Target.Client, base.Source, base.Source, base.Target, false);
		}
	}
}
