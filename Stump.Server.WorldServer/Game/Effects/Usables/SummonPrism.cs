using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Handlers.Usables;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Items.Player;
using Stump.Server.WorldServer.Game.Prisms;

namespace Stump.Server.WorldServer.Game.Effects.Usables
{
    [EffectHandler(EffectsEnum.Effect_Summon_Prism)]
    public class SummonPrism : UsableEffectHandler
    {
        public SummonPrism(EffectBase effect, Character target, BasePlayerItem item)
            : base(effect, target, item)
        {

        }

        public override bool Apply()
        {
            bool result;
            if (this.Target.Guild != null && this.Target.Guild.Alliance != null)
            {
                result = Singleton<PrismManager>.Instance.TryAddPrism(this.Target, true);
            }
            else
            {
                result = false;
            }
            return result;
        }
    }
}
