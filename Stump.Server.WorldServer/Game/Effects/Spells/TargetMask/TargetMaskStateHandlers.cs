using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stump.Server.WorldServer.Game.Effects.Spells.TargetMask
{
    public class TargetMaskStateHandlers
    {
        [TargetMaskHandler('e')]
        public static bool HandleHasNotState(FightActor source, FightActor target, EffectBase effect, int param)
        {
            return !target.HasState(param);
        }

        [TargetMaskHandler('E')]
        public static bool HandleHasState(FightActor source, FightActor target, EffectBase effect, int param)
        {
            return target.HasState(param);
        }
    }
}
