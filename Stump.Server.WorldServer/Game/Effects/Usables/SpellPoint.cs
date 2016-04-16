using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Items.Player;

namespace Stump.Server.WorldServer.Game.Effects.Handlers.Usables
{
	[EffectHandler(EffectsEnum.Effect_AddSpellPoints)]
	public class SpellPoint : UsableEffectHandler
	{
		public SpellPoint(EffectBase effect, Character target, BasePlayerItem item) : base(effect, target, item)
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
				if (effectInteger.Value < 1)
				{
					result = false;
				}
				else
				{
					base.UsedItems = base.NumberOfUses;
					Character expr_47 = base.Target;
					expr_47.SpellsPoints += (ushort)((long)effectInteger.Value * (long)((ulong)base.NumberOfUses));
					base.Target.RefreshStats();
					result = true;
				}
			}
			return result;
		}
	}
}
