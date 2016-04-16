using System;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Spells;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Steals
{
	[EffectHandler(EffectsEnum.Effect_StealMP_441)]
	public class MPSteal : SpellEffectHandler
	{
		public MPSteal(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
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
				base.AddStatBuff(current, Convert.ToInt16(-effectInteger.Value), PlayerFields.MP, true, 169);
				if (this.Effect.Duration > 0)
				{
					base.AddStatBuff(base.Caster, effectInteger.Value, PlayerFields.MP, true, 128);
				}
				else
				{
					base.Caster.RegainMP(effectInteger.Value);
				}
			}
			result = true;
			return result;
		}
	}
}
