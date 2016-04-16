using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Spells;
using Stump.Server.WorldServer.Handlers.Actions;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Steals
{
	[EffectHandler(EffectsEnum.Effect_StealAP_84)]
	public class APStealNonFix : SpellEffectHandler
	{
		public APStealNonFix(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
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
				int num = 0;
				int num2 = 0;
				while (num2 < (int)effectInteger.Value && num < current.AP)
				{
					if (current.RollAPLose(base.Caster))
					{
						num++;
					}
					num2++;
				}
				short num3 = (short)((int)effectInteger.Value - num);
				if (num3 > 0)
				{
					ActionsHandler.SendGameActionFightDodgePointLossMessage(base.Fight.Clients, ActionsEnum.ACTION_FIGHT_SPELL_DODGED_PA, base.Caster, current, num3);
				}
				if (num <= 0)
				{
					result = false;
					return result;
				}
				base.AddStatBuff(current, (short)(-(short)num), PlayerFields.AP, true, 168);
				if (this.Effect.Duration > 0)
				{
					base.AddStatBuff(base.Caster, (short)num, PlayerFields.AP, true, 111);
				}
				else
				{
					base.Caster.RegainAP((short)num);
				}
			}
			result = true;
			return result;
		}
	}
}
