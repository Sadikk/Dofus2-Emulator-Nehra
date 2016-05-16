using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stump.Server.WorldServer.Game.Effects.Spells.TargetMask
{
    class TargetMaskMonsterHandlers
    {
        [TargetMaskHandler('f')]
        public static bool HandleIsNotMonster(FightActor source, FightActor target, EffectBase effect, int param)
        {
            return true;
            //todo how to get GID ?
            //return (target is MonsterFighter && (target as MonsterFighter).
        }

        [TargetMaskHandler('F')]
        public static bool HandleIsMonster(FightActor source, FightActor target, EffectBase effect, int param)
        {
            return true;    
        }
    }
}
