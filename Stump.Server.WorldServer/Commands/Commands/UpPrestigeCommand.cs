using Stump.Server.WorldServer.Commands.Commands.Patterns;

namespace Commands.Commands
{
    class PrestigeCommand : InGameCommand
    {
        public PrestigeCommand()
        {
            base.Aliases = new string[] { "prestige" };
            base.Description = "Passer au prestige suivant";
            base.RequiredRole = Stump.DofusProtocol.Enums.RoleEnum.Player;
        }

        public override void Execute(Stump.Server.WorldServer.Commands.Trigger.GameTrigger trigger)
        {
            //if (trigger.Character.Level == 200)
            //{
            //    if (trigger.Character.Record.Prestige == 10)
            //    {
            //        trigger.ReplyBold("Vous ne pouvez pas passer au prestige 11 pour le moment.");
            //    }
            //    else
            //    {
            //        trigger.Character.PrestigeUp();
            //        trigger.Character.PrestigeReward();
            //        trigger.Character.SaveLater();
            //    }
            //}



            //else
            //{
            //    trigger.ReplyBold("Vous n'avez pas le level nécessaire.");
            //}

            trigger.ReplyBold("Cette fonction est, pour le moment, desactivé.");
        }
    }
}
