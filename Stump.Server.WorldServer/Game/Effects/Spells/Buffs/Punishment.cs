using System.Linq;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Fights.Buffs;
using Stump.Server.WorldServer.Game.Spells;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Buffs
{
	[EffectHandler(EffectsEnum.Effect_Punishment)]
	public class Punishment : SpellEffectHandler
	{
		public Punishment(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
		{
		}
		public override bool Apply()
		{
			foreach (FightActor current in base.GetAffectedActors())
			{
				base.AddTriggerBuff(current, true, BuffTriggerType.AFTER_ATTACKED, new TriggerBuffApplyHandler(this.OnActorAttacked));
			}
			return true;
		}
		private void OnActorAttacked(TriggerBuff buff, BuffTriggerType trigger, object token)
		{
			System.Collections.Generic.IEnumerable<StatBuff> source = buff.Target.GetBuffs((Buff entry) => entry.Spell == base.Spell).OfType<StatBuff>();
			int num = (
				from entry in source
				where (int)entry.Duration == this.Effect.Duration
				select entry).Sum((StatBuff entry) => (int)entry.Value);
			short diceFace = base.Dice.DiceFace;
			if (num < (int)diceFace)
			{
                Fights.Damage damage = (Fights.Damage)token;
				int num2 = damage.Amount;
				if (num2 + num > (int)diceFace)
				{
					num2 = (int)((short)((int)diceFace - num));
				}
				PlayerFields punishmentBoostType = Punishment.GetPunishmentBoostType(base.Dice.DiceNum);
				StatBuff buff2 = new StatBuff(buff.Target.PopNextBuffId(), buff.Target, base.Caster, base.Dice, base.Spell, (short)num2, punishmentBoostType, false, true, (short)Punishment.GetBuffEffectId(punishmentBoostType))
				{
					Duration = base.Dice.Value
				};
				buff.Target.AddAndApplyBuff(buff2, true);
			}
		}
		private static PlayerFields GetPunishmentBoostType(short punishementAction)
		{
			PlayerFields result;
			switch (punishementAction)
			{
			case 118:
				result = PlayerFields.Strength;
				return result;
			case 119:
				result = PlayerFields.Agility;
				return result;
			case 120:
			case 121:
			case 122:
				break;
			case 123:
				result = PlayerFields.Chance;
				return result;
			case 124:
				result = PlayerFields.Wisdom;
				return result;
			case 125:
				goto IL_61;
			case 126:
				result = PlayerFields.Intelligence;
				return result;
			default:
				if (punishementAction == 407)
				{
					goto IL_61;
				}
				break;
			}
			throw new System.Exception(string.Format("PunishmentBoostType not found for action {0}", punishementAction));
			IL_61:
			result = PlayerFields.Vitality;
			return result;
		}
		private static EffectsEnum GetBuffEffectId(PlayerFields caracteristic)
		{
			EffectsEnum result;
			switch (caracteristic)
			{
			case PlayerFields.Strength:
				result = EffectsEnum.Effect_AddStrength;
				break;
			case PlayerFields.Vitality:
				result = EffectsEnum.Effect_AddVitality;
				break;
			case PlayerFields.Wisdom:
				result = EffectsEnum.Effect_AddWisdom;
				break;
			case PlayerFields.Chance:
				result = EffectsEnum.Effect_AddChance;
				break;
			case PlayerFields.Agility:
				result = EffectsEnum.Effect_AddAgility;
				break;
			case PlayerFields.Intelligence:
				result = EffectsEnum.Effect_AddIntelligence;
				break;
			default:
				throw new System.Exception(string.Format("Buff Effect not found for caracteristic {0}", caracteristic));
			}
			return result;
		}
	}
}
