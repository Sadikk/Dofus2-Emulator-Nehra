using System;
using System.Linq;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Fights.Buffs;
using Stump.Server.WorldServer.Game.Spells;
using Stump.Server.WorldServer.Handlers.Context;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Buffs
{
	[EffectHandler(EffectsEnum.Effect_ReduceEffectsDuration)]
	public class ReduceBuffDuration : SpellEffectHandler
	{
		public ReduceBuffDuration(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
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
				foreach (Buff current2 in 
					from buff in current.GetBuffs().ToArray<Buff>()
					where buff.Dispellable
					select buff)
				{
					Buff expr_71 = current2;
					expr_71.Duration -= effectInteger.Value;
					if (current2.Duration <= 0)
					{
						current.RemoveAndDispellBuff(current2);
					}
				}
				ContextHandler.SendGameActionFightModifyEffectsDurationMessage(base.Fight.Clients, base.Caster, current, Convert.ToInt16(-effectInteger.Value));
			}
			result = true;
			return result;
		}
	}
}
