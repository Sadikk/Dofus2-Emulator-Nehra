using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Conditions.Criterions
{
    public class AllianceRightsCriterion : Criterion
    {
        public override bool Eval(Character character)
        {
            return true;
        }

        public override void Build()
        { }
    }
}
