using System;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Spells;
using Stump.Server.WorldServer.Handlers.Actions;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Debuffs
{
	[EffectHandler(EffectsEnum.Effect_LostMP), EffectHandler(EffectsEnum.Effect_LosingMP)]
	public class MPDebuffNonFix : SpellEffectHandler
	{
		public MPDebuffNonFix(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
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
				short num = effectInteger.Value;
				if (this.Effect.EffectId != EffectsEnum.Effect_LosingMP)
				{
					num = 0;
					int num2 = 0;
					while (num2 < (int)effectInteger.Value && (int)num < current.MP)
					{
						if (current.RollMPLose(base.Caster))
						{
							num += 1;
						}
						num2++;
					}
					short num3 =  Convert.ToInt16(effectInteger.Value - num);
					if (num3 > 0)
					{
						ActionsHandler.SendGameActionFightDodgePointLossMessage(base.Fight.Clients, ActionsEnum.ACTION_FIGHT_SPELL_DODGED_PM, base.Caster, current, num3);
					}
				}
				if (num <= 0)
				{
					result = false;
					return result;
				}
				if (this.Effect.Duration > 1)
				{
					base.AddStatBuff(current, Convert.ToInt16(-num), PlayerFields.MP, true);
				}
				else
				{
					current.LostMP(num);
				}
			}
			result = true;
			return result;
		}
	}
}
