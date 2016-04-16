using System;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Spells;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Debuffs
{
	[EffectHandler(EffectsEnum.Effect_SubMP), EffectHandler(EffectsEnum.Effect_SubMP_1080)]
	public class MPDebuff : SpellEffectHandler
	{
		public MPDebuff(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
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
				if (this.Effect.Duration > 1)
				{
					base.AddStatBuff(current, Convert.ToInt16(-effectInteger.Value), PlayerFields.MP, true, 169);
				}
				else
				{
					current.LostMP(effectInteger.Value);
				}
			}
			result = true;
			return result;
		}
	}
}
