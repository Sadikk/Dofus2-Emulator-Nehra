using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Spells;
using System.Linq;
namespace Stump.Server.WorldServer.AI.Fights.Spells
{
	public class SpellIdentifier
	{
		public static SpellCategory GetSpellCategories(Spell spell)
		{
			return spell.CurrentSpellLevel.Effects.Aggregate(SpellCategory.None, (SpellCategory current, EffectDice effect) => current | SpellIdentifier.GetEffectCategories(effect.EffectId));
		}
		public static SpellCategory GetEffectCategories(EffectsEnum effectId)
		{
			SpellCategory result;
			if (effectId <= EffectsEnum.Effect_279)
			{
				switch (effectId)
				{
				case EffectsEnum.Effect_Teleport:
					result = SpellCategory.Teleport;
					return result;
				case EffectsEnum.Effect_PushBack:
                        result = SpellCategory.Curse;
                        return result;
                    default:
					if (effectId == EffectsEnum.Effect_Dodge)
					{
						goto IL_496;
					}
					switch (effectId)
					{
					case EffectsEnum.Effect_StealMP_77:
					case EffectsEnum.Effect_RemoveAP:
					case EffectsEnum.Effect_LostMP:
					case EffectsEnum.Effect_StealKamas:
					case EffectsEnum.Effect_LoseHPByUsingAP:
					case EffectsEnum.Effect_LosingAP:
					case EffectsEnum.Effect_LosingMP:
					case EffectsEnum.Effect_SubRange_135:
					case EffectsEnum.Effect_SkipTurn:
					case EffectsEnum.Effect_SubDamageBonus:
					case EffectsEnum.Effect_SubChance:
					case EffectsEnum.Effect_SubVitality:
					case EffectsEnum.Effect_SubAgility:
					case EffectsEnum.Effect_SubIntelligence:
					case EffectsEnum.Effect_SubWisdom:
					case EffectsEnum.Effect_SubStrength:
					case EffectsEnum.Effect_SubDodgeAPProbability:
					case EffectsEnum.Effect_SubDodgeMPProbability:
					case EffectsEnum.Effect_SubAP:
					case EffectsEnum.Effect_SubMP:
					case EffectsEnum.Effect_SubCriticalHit:
					case EffectsEnum.Effect_SubMagicDamageReduction:
					case EffectsEnum.Effect_SubPhysicalDamageReduction:
					case EffectsEnum.Effect_SubInitiative:
					case EffectsEnum.Effect_SubProspecting:
					case EffectsEnum.Effect_SubHealBonus:
					case EffectsEnum.Effect_SubDamageBonusPercent:
					case EffectsEnum.Effect_197:
					case EffectsEnum.Effect_SubEarthResistPercent:
					case EffectsEnum.Effect_SubWaterResistPercent:
					case EffectsEnum.Effect_SubAirResistPercent:
					case EffectsEnum.Effect_SubFireResistPercent:
					case EffectsEnum.Effect_SubNeutralResistPercent:
					case EffectsEnum.Effect_SubEarthElementReduction:
					case EffectsEnum.Effect_SubWaterElementReduction:
					case EffectsEnum.Effect_SubAirElementReduction:
					case EffectsEnum.Effect_SubFireElementReduction:
					case EffectsEnum.Effect_SubNeutralElementReduction:
					case EffectsEnum.Effect_SubPvpEarthResistPercent:
					case EffectsEnum.Effect_SubPvpWaterResistPercent:
					case EffectsEnum.Effect_SubPvpAirResistPercent:
					case EffectsEnum.Effect_SubPvpFireResistPercent:
					case EffectsEnum.Effect_SubPvpNeutralResistPercent:
					case EffectsEnum.Effect_StealChance:
					case EffectsEnum.Effect_StealVitality:
					case EffectsEnum.Effect_StealAgility:
					case EffectsEnum.Effect_StealIntelligence:
					case EffectsEnum.Effect_StealWisdom:
					case EffectsEnum.Effect_StealStrength:
					case EffectsEnum.Effect_275:
					case EffectsEnum.Effect_276:
					case EffectsEnum.Effect_277:
					case EffectsEnum.Effect_278:
					case EffectsEnum.Effect_279:
                                result = SpellCategory.Curse;
                                return result;
                    case EffectsEnum.Effect_AddMP:
					case EffectsEnum.Effect_AddGlobalDamageReduction_105:
					case EffectsEnum.Effect_ReflectSpell:
					case EffectsEnum.Effect_AddDamageReflection:
					case EffectsEnum.Effect_AddHealth:
					case EffectsEnum.Effect_AddAP_111:
					case EffectsEnum.Effect_AddDamageBonus:
					case EffectsEnum.Effect_AddDamageMultiplicator:
					case EffectsEnum.Effect_AddCriticalHit:
					case EffectsEnum.Effect_AddRange:
					case EffectsEnum.Effect_AddStrength:
					case EffectsEnum.Effect_AddAgility:
					case EffectsEnum.Effect_RegainAP:
					case EffectsEnum.Effect_AddDamageBonus_121:
					case EffectsEnum.Effect_AddChance:
					case EffectsEnum.Effect_AddWisdom:
					case EffectsEnum.Effect_AddVitality:
					case EffectsEnum.Effect_AddIntelligence:
					case EffectsEnum.Effect_AddMP_128:
					case EffectsEnum.Effect_AddRange_136:
					case EffectsEnum.Effect_AddPhysicalDamage_137:
					case EffectsEnum.Effect_AddPhysicalDamage_142:
					case EffectsEnum.Effect_Invisibility:
					case EffectsEnum.Effect_AddGlobalDamageReduction:
					case EffectsEnum.Effect_AddDamageBonusPercent:
					case EffectsEnum.Effect_AddProspecting:
					case EffectsEnum.Effect_AddHealBonus:
					case EffectsEnum.Effect_AddSummonLimit:
					case EffectsEnum.Effect_AddPhysicalDamageReduction:
					case EffectsEnum.Effect_AddEarthResistPercent:
					case EffectsEnum.Effect_AddWaterResistPercent:
					case EffectsEnum.Effect_AddAirResistPercent:
					case EffectsEnum.Effect_AddFireResistPercent:
					case EffectsEnum.Effect_AddNeutralResistPercent:
					case EffectsEnum.Effect_AddEarthElementReduction:
					case EffectsEnum.Effect_AddWaterElementReduction:
					case EffectsEnum.Effect_AddAirElementReduction:
					case EffectsEnum.Effect_AddFireElementReduction:
					case EffectsEnum.Effect_AddNeutralElementReduction:
					case EffectsEnum.Effect_AddArmorDamageReduction:
						goto IL_496;
					case EffectsEnum.Effect_79:
					case (EffectsEnum)80:
					case EffectsEnum.Effect_StealHPFix:
					case (EffectsEnum)83:
					case EffectsEnum.Effect_StealAP_84:
					case EffectsEnum.Effect_DamagePercentWater:
					case EffectsEnum.Effect_DamagePercentEarth:
					case EffectsEnum.Effect_DamagePercentAir:
					case EffectsEnum.Effect_DamagePercentFire:
					case EffectsEnum.Effect_DamagePercentNeutral:
					case EffectsEnum.Effect_GiveHPPercent:
					case (EffectsEnum)102:
					case (EffectsEnum)103:
					case (EffectsEnum)104:
					case EffectsEnum.Effect_109:
					case EffectsEnum.Effect_DoubleDamageOrRestoreHP:
					case EffectsEnum.Effect_SubRange:
					case EffectsEnum.Effect_AddCriticalMiss:
					case (EffectsEnum)129:
					case EffectsEnum.Effect_DispelMagicEffects:
					case EffectsEnum.Effect_IncreaseDamage_138:
					case EffectsEnum.Effect_RestoreEnergyPoints:
					case EffectsEnum.Effect_DamageFix:
					case EffectsEnum.Effect_ChangesWords:
					case EffectsEnum.Effect_ReviveAlly:
					case EffectsEnum.Effect_Followed:
					case EffectsEnum.Effect_ChangeAppearance:
					case (EffectsEnum)151:
					case EffectsEnum.Effect_IncreaseWeight:
					case EffectsEnum.Effect_DecreaseWeight:
					case EffectsEnum.Effect_166:
					case (EffectsEnum)167:
					case (EffectsEnum)170:
					case EffectsEnum.Effect_AddInitiative:
					case EffectsEnum.Effect_AddMagicDamageReduction:
					case (EffectsEnum)187:
					case EffectsEnum.Effect_188:
					case (EffectsEnum)189:
					case (EffectsEnum)190:
					case (EffectsEnum)191:
					case (EffectsEnum)192:
					case (EffectsEnum)193:
					case EffectsEnum.Effect_GiveKamas:
					case (EffectsEnum)195:
					case (EffectsEnum)196:
					case (EffectsEnum)198:
					case (EffectsEnum)199:
					case (EffectsEnum)200:
					case EffectsEnum.Effect_201:
					case EffectsEnum.Effect_RevealsInvisible:
					case (EffectsEnum)203:
					case (EffectsEnum)204:
					case (EffectsEnum)205:
					case EffectsEnum.Effect_206:
					case (EffectsEnum)207:
					case (EffectsEnum)208:
					case (EffectsEnum)209:
					case (EffectsEnum)223:
					case (EffectsEnum)224:
					case (EffectsEnum)227:
					case (EffectsEnum)228:
					case (EffectsEnum)231:
					case (EffectsEnum)232:
					case (EffectsEnum)233:
					case (EffectsEnum)234:
					case (EffectsEnum)235:
					case (EffectsEnum)236:
					case (EffectsEnum)237:
					case (EffectsEnum)238:
					case EffectsEnum.Effect_239:
					case EffectsEnum.Effect_AddPvpEarthResistPercent:
					case EffectsEnum.Effect_AddPvpWaterResistPercent:
					case EffectsEnum.Effect_AddPvpAirResistPercent:
					case EffectsEnum.Effect_AddPvpFireResistPercent:
					case EffectsEnum.Effect_AddPvpNeutralResistPercent:
					case EffectsEnum.Effect_AddPvpEarthElementReduction:
					case EffectsEnum.Effect_AddPvpWaterElementReduction:
					case EffectsEnum.Effect_AddPvpAirElementReduction:
					case EffectsEnum.Effect_AddPvpFireElementReduction:
					case EffectsEnum.Effect_AddPvpNeutralElementReduction:
					case (EffectsEnum)272:
					case (EffectsEnum)273:
					case (EffectsEnum)274:
						goto IL_492;
					case EffectsEnum.Effect_HealHP_81:
					case EffectsEnum.Effect_HealHP_108:
					case EffectsEnum.Effect_HealHP_143:
						result = SpellCategory.Healing;
						return result;
					case EffectsEnum.Effect_StealHPWater:
						result = (SpellCategory.Healing | SpellCategory.DamagesWater);
						return result;
					case EffectsEnum.Effect_StealHPEarth:
						result = (SpellCategory.Healing | SpellCategory.DamagesEarth);
						return result;
					case EffectsEnum.Effect_StealHPAir:
						result = (SpellCategory.Healing | SpellCategory.DamagesAir);
						return result;
					case EffectsEnum.Effect_StealHPFire:
						result = (SpellCategory.Healing | SpellCategory.DamagesFire);
						return result;
					case EffectsEnum.Effect_StealHPNeutral:
						result = (SpellCategory.Healing | SpellCategory.DamagesNeutral);
						return result;
					case EffectsEnum.Effect_DamageWater:
						result = SpellCategory.DamagesWater;
						return result;
					case EffectsEnum.Effect_DamageEarth:
						result = SpellCategory.DamagesEarth;
						return result;
					case EffectsEnum.Effect_DamageAir:
						result = SpellCategory.DamagesAir;
						return result;
					case EffectsEnum.Effect_DamageFire:
						result = SpellCategory.DamagesFire;
						return result;
					case EffectsEnum.Effect_DamageNeutral:
						goto IL_49A;
					case EffectsEnum.Effect_Kill:
						result = SpellCategory.Damages;
						return result;
					case EffectsEnum.Effect_Double:
					case EffectsEnum.Effect_Summon:
					case EffectsEnum.Effect_185:
						break;
					default:
						goto IL_492;
					}
					break;
				}
			}
			else
			{
				if (effectId <= EffectsEnum.Effect_623)
				{
					switch (effectId)
					{
					case EffectsEnum.Effect_SubPushDamageReduction:
					case EffectsEnum.Effect_SubCriticalDamageBonus:
					case EffectsEnum.Effect_SubCriticalDamageReduction:
					case EffectsEnum.Effect_SubEarthDamageBonus:
					case EffectsEnum.Effect_SubFireDamageBonus:
					case EffectsEnum.Effect_SubWaterDamageBonus:
					case EffectsEnum.Effect_SubAirDamageBonus:
					case EffectsEnum.Effect_SubNeutralDamageBonus:
					case EffectsEnum.Effect_StealAP_440:
					case EffectsEnum.Effect_StealMP_441:
                            result = SpellCategory.Curse;
                            return result;
                    case EffectsEnum.Effect_SubPushDamageBonus:
					case (EffectsEnum)432:
					case (EffectsEnum)433:
					case (EffectsEnum)434:
					case (EffectsEnum)435:
					case (EffectsEnum)436:
					case (EffectsEnum)437:
					case (EffectsEnum)438:
					case (EffectsEnum)439:
						goto IL_492;
					case EffectsEnum.Effect_AddPushDamageBonus:
					case EffectsEnum.Effect_AddPushDamageReduction:
					case EffectsEnum.Effect_AddCriticalDamageBonus:
					case EffectsEnum.Effect_AddCriticalDamageReduction:
					case EffectsEnum.Effect_AddEarthDamageBonus:
					case EffectsEnum.Effect_AddFireDamageBonus:
					case EffectsEnum.Effect_AddWaterDamageBonus:
					case EffectsEnum.Effect_AddAirDamageBonus:
					case EffectsEnum.Effect_AddNeutralDamageBonus:
						goto IL_496;
					default:
						switch (effectId)
						{
						case EffectsEnum.Effect_621:
						case EffectsEnum.Effect_623:
							break;
						case EffectsEnum.Effect_622:
							goto IL_492;
						default:
							goto IL_492;
						}
						break;
					}
				}
				else
				{
					if (effectId == EffectsEnum.Effect_Punishment_Damage)
					{
						goto IL_49A;
					}
                    if(effectId == EffectsEnum.Effect_SubAPTelefrag)
                    {
                        result = SpellCategory.Curse;
                        return result;
                    }
					if (effectId != EffectsEnum.Effect_AddVitalityPercent)
					{
						goto IL_492;
					}
					goto IL_496;
				}
			}
			result = SpellCategory.Summoning;
			return result;
			IL_492:
			result = SpellCategory.None;
			return result;
			IL_496:
			result = SpellCategory.Buff;
			return result;
			IL_49A:
			result = SpellCategory.DamagesNeutral;
			return result;
		}
	}
}
