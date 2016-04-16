using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Spells;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Buffs
{
	[EffectHandler(EffectsEnum.Effect_RegainAP), EffectHandler(EffectsEnum.Effect_AddAP_111)]
	public class APBuff : SpellEffectHandler
	{
		public APBuff(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
		{
		}
		public override bool Apply()
		{
			bool result;
			foreach (FightActor current in base.GetAffectedActors())
			{
				EffectInteger effectInteger = base.GenerateEffect();
				if (effectInteger == null)
				{
					result = false;
					return result;
				}
				if (this.Effect.Duration > 0)
				{
					base.AddStatBuff(current, effectInteger.Value, PlayerFields.AP, true);
				}
				else
				{
					current.RegainAP(effectInteger.Value);
				}
			}
			result = true;
			return result;
		}
	}
}
