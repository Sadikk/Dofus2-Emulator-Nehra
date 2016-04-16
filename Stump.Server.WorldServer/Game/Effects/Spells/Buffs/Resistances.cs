using System;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Fights.Buffs.Customs;
using Stump.Server.WorldServer.Game.Spells;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Buffs
{
	[EffectHandler(EffectsEnum.Effect_AddResistances), EffectHandler(EffectsEnum.Effect_SubResistances)]
	public class Resistances : SpellEffectHandler
	{
		public Resistances(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
		{
		}
		public override bool Apply()
		{
			EffectInteger effectInteger = base.GenerateEffect();
			bool result;
			if (effectInteger == null)
			{
				result = false;
			}
			else
			{
				foreach (FightActor current in base.GetAffectedActors())
				{
					ResistancesBuff buff = new ResistancesBuff(current.PopNextBuffId(), current, base.Caster, effectInteger, base.Spell, Convert.ToInt16((this.Effect.EffectId == EffectsEnum.Effect_SubResistances) ? (-effectInteger.Value) : effectInteger.Value), false, true);
					current.AddAndApplyBuff(buff, true);
				}
				result = true;
			}
			return result;
		}
	}
}
