using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Actors.RolePlay.TaxCollectors;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Handlers.Usables;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Items.Player;

namespace Game.Effects.Usables
{
    [EffectHandler(EffectsEnum.Effect_Summon_TaxCollector)]
    public class SummonTaxCollector : UsableEffectHandler
    {
        public SummonTaxCollector(EffectBase effect, Character target, BasePlayerItem item)
            : base(effect, target, item)
        {

        }

        public override bool Apply()
        {
            bool result;
            if (this.Target.Guild != null)
            {
                result = Singleton<TaxCollectorManager>.Instance.TryAddTaxCollectorSpawn(this.Target, true);
            }
            else
            {
                result = false;
            }
            return result;
        }
    }
}
