using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Items.Player;

namespace Stump.Server.WorldServer.Game.Effects.Handlers.Usables
{
	[EffectHandler(EffectsEnum.Effect_GiveKamas)]
	public class GiveKamas : UsableEffectHandler
	{
		public GiveKamas(EffectBase effect, Character target, BasePlayerItem item) : base(effect, target, item)
		{
		}
		public override bool Apply()
		{
			EffectInteger effectInteger = this.Effect.GenerateEffect(EffectGenerationContext.Item, EffectGenerationType.Normal) as EffectInteger;
			bool result;
			if (effectInteger == null)
			{
				result = false;
			}
			else
			{
				int amount = (int)((long)effectInteger.Value * (long)((ulong)base.NumberOfUses));
				base.UsedItems = base.NumberOfUses;
				base.Target.Inventory.AddKamas(amount);
				result = true;
			}
			return result;
		}
	}
}
