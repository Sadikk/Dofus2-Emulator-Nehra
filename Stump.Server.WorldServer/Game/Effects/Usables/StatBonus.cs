using Stump.Core.Attributes;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Items.Player;
using System;
using System.Drawing;
namespace Stump.Server.WorldServer.Game.Effects.Handlers.Usables
{
	[EffectHandler(EffectsEnum.Effect_AddPermanentWisdom), EffectHandler(EffectsEnum.Effect_AddPermanentIntelligence), EffectHandler(EffectsEnum.Effect_AddPermanentStrength), EffectHandler(EffectsEnum.Effect_AddPermanentVitality), EffectHandler(EffectsEnum.Effect_AddPermanentChance), EffectHandler(EffectsEnum.Effect_AddPermanentAgility)]
	public class StatBonus : UsableEffectHandler
	{
		[Variable]
		public static short StatBonusLimit = 101;
		public StatBonus(EffectBase effect, Character target, BasePlayerItem item) : base(effect, target, item)
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
				short num = this.AdjustBonusStat((short)((long)effectInteger.Value * (long)((ulong)base.NumberOfUses)));
				if (num == 0)
				{
					base.Target.SendServerMessage(string.Format("Bonus limit reached : {0}", StatBonus.StatBonusLimit), Color.Red);
					result = false;
				}
				else
				{
					base.Target.Stats[StatBonus.GetEffectCharacteristic(this.Effect.EffectId)].Base += (int)num;
					base.UsedItems = (uint)System.Math.Ceiling((double)num / (double)effectInteger.Value);
					this.UpdatePermanentStatField(num);
					base.Target.RefreshStats();
					result = true;
				}
			}
			return result;
		}
		private static PlayerFields GetEffectCharacteristic(EffectsEnum effect)
		{
			PlayerFields result;
			switch (effect)
			{
			case EffectsEnum.Effect_AddPermanentWisdom:
				result = PlayerFields.Wisdom;
				break;
			case EffectsEnum.Effect_AddPermanentStrength:
				result = PlayerFields.Strength;
				break;
			case EffectsEnum.Effect_AddPermanentChance:
				result = PlayerFields.Chance;
				break;
			case EffectsEnum.Effect_AddPermanentAgility:
				result = PlayerFields.Agility;
				break;
			case EffectsEnum.Effect_AddPermanentVitality:
				result = PlayerFields.Vitality;
				break;
			case EffectsEnum.Effect_AddPermanentIntelligence:
				result = PlayerFields.Intelligence;
				break;
			default:
				throw new System.Exception(string.Format("Effect {0} has not associated Characteristic", effect));
			}
			return result;
		}
		private short AdjustBonusStat(short bonus)
		{
			short num;
			short result;
			switch (this.Effect.EffectId)
			{
			case EffectsEnum.Effect_AddPermanentWisdom:
				num = base.Target.PermanentAddedWisdom;
				break;
			case EffectsEnum.Effect_AddPermanentStrength:
				num = base.Target.PermanentAddedStrength;
				break;
			case EffectsEnum.Effect_AddPermanentChance:
				num = base.Target.PermanentAddedChance;
				break;
			case EffectsEnum.Effect_AddPermanentAgility:
				num = base.Target.PermanentAddedAgility;
				break;
			case EffectsEnum.Effect_AddPermanentVitality:
				num = base.Target.PermanentAddedVitality;
				break;
			case EffectsEnum.Effect_AddPermanentIntelligence:
				num = base.Target.PermanentAddedIntelligence;
				break;
			default:
				result = 0;
				return result;
			}
			if (num >= StatBonus.StatBonusLimit)
			{
				result = 0;
			}
			else
			{
				if (num + bonus > StatBonus.StatBonusLimit)
				{
					result =  Convert.ToInt16(num - StatBonus.StatBonusLimit);
				}
				else
				{
					result = bonus;
				}
			}
			return result;
		}
		private void UpdatePermanentStatField(short bonus)
		{
			switch (this.Effect.EffectId)
			{
			case EffectsEnum.Effect_AddPermanentWisdom:
			{
				Character expr_3B = base.Target;
				expr_3B.PermanentAddedWisdom += bonus;
				break;
			}
			case EffectsEnum.Effect_AddPermanentStrength:
			{
				Character expr_51 = base.Target;
				expr_51.PermanentAddedStrength += bonus;
				break;
			}
			case EffectsEnum.Effect_AddPermanentChance:
			{
				Character expr_67 = base.Target;
				expr_67.PermanentAddedChance += bonus;
				break;
			}
			case EffectsEnum.Effect_AddPermanentAgility:
			{
				Character expr_7D = base.Target;
				expr_7D.PermanentAddedAgility += bonus;
				break;
			}
			case EffectsEnum.Effect_AddPermanentVitality:
			{
				Character expr_93 = base.Target;
				expr_93.PermanentAddedVitality += bonus;
				break;
			}
			case EffectsEnum.Effect_AddPermanentIntelligence:
			{
				Character expr_A9 = base.Target;
				expr_A9.PermanentAddedIntelligence += bonus;
				break;
			}
			}
		}
	}
}
