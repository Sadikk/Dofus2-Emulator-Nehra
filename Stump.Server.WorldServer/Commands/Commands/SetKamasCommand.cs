using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.WorldServer.Commands.Commands.Patterns;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class SetKamasCommand : TargetCommand
	{
		public SetKamasCommand()
		{
			base.Aliases = new string[]
			{
				"kamas"
			};
			base.RequiredRole = RoleEnum.GameMaster;
			base.Description = "Set the amount kamas of target's inventory";
			base.AddParameter<int>("amount", "amount", "Amount of kamas to set", 0, false, null);
			base.AddTargetParameter(true, "Defined target");
		}
		public override void Execute(TriggerBase trigger)
		{
			Character[] targets = base.GetTargets(trigger);
			for (int i = 0; i < targets.Length; i++)
			{
				Character character = targets[i];
				int num = trigger.Get<int>("amount");
				character.Inventory.SetKamas(num);
				trigger.ReplyBold("{0} has now {1} kamas", new object[]
				{
					character,
					num
				});
			}
		}
	}
}
