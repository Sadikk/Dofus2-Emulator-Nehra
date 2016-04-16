using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Items.Player;

namespace Stump.Server.WorldServer.Game.Effects.Handlers.Usables
{
	[EffectHandler(EffectsEnum.Effect_LearnSpell)]
	public class LearnSpell : UsableEffectHandler
	{
		public LearnSpell(EffectBase effect, Character target, BasePlayerItem item) : base(effect, target, item)
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
				base.UsedItems = base.NumberOfUses;
				base.Target.Spells.LearnSpell((int)effectInteger.Value);
				result = true;
			}
			return result;
		}
	}
}
