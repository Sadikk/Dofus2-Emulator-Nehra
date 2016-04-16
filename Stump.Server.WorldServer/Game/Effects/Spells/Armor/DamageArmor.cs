using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Fights.Buffs;
using Stump.Server.WorldServer.Game.Spells;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Armor
{
	[EffectHandler(EffectsEnum.Effect_AddGlobalDamageReduction_105), EffectHandler(EffectsEnum.Effect_AddArmorDamageReduction)]
	public class DamageArmor : SpellEffectHandler
	{
		public DamageArmor(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
		{
		}
		public override bool Apply()
		{
			bool result;
			foreach (FightActor current in base.GetAffectedActors())
			{
				EffectInteger left = this.Effect.GenerateEffect(EffectGenerationContext.Spell, EffectGenerationType.Normal) as EffectInteger;
				if (left == null)
				{
					result = false;
					return result;
				}
				if (this.Effect.Duration <= 0)
				{
					result = false;
					return result;
				}
				base.AddTriggerBuff(current, true, BuffTriggerType.BUFF_ADDED, new TriggerBuffApplyHandler(DamageArmor.ApplyArmorBuff), new TriggerBuffRemoveHandler(DamageArmor.RemoveArmorBuff));
			}
			result = true;
			return result;
		}
		public static void ApplyArmorBuff(TriggerBuff buff, BuffTriggerType trigger, object token)
		{
			EffectInteger effectInteger = buff.GenerateEffect();
			if (!(effectInteger == null))
			{
				foreach (PlayerFields current in DamageArmor.GetAssociatedCaracteristics(buff.Spell.Id))
				{
					buff.Target.Stats[current].Context += buff.Target.CalculateArmorValue((int)effectInteger.Value);
				}
			}
		}
		public static void RemoveArmorBuff(TriggerBuff buff)
		{
			EffectInteger effectInteger = buff.GenerateEffect();
			if (!(effectInteger == null))
			{
				foreach (PlayerFields current in DamageArmor.GetAssociatedCaracteristics(buff.Spell.Id))
				{
					buff.Target.Stats[current].Context -= buff.Target.CalculateArmorValue((int)effectInteger.Value);
				}
			}
		}
		public static System.Collections.Generic.IEnumerable<PlayerFields> GetAssociatedCaracteristics(int spellId)
		{
			if (spellId > 6)
			{
				if (spellId != 14)
				{
					if (spellId != 18)
					{
						switch (spellId)
						{
						case 451:
							goto IL_AF;
						case 452:
							goto IL_86;
						case 453:
							goto IL_9C;
						case 454:
							goto IL_C2;
						}
						goto IL_70;
					}
					IL_AF:
					yield return PlayerFields.WaterDamageArmor;
					goto IL_11A;
				}
				IL_C2:
				yield return PlayerFields.AirDamageArmor;
				goto IL_11A;
			}
			if (spellId == 1)
			{
				goto IL_86;
			}
			if (spellId == 6)
			{
				goto IL_9C;
			}
			IL_70:
			yield return PlayerFields.GlobalDamageReduction;
			goto IL_11A;
			IL_86:
			yield return PlayerFields.FireDamageArmor;
			goto IL_11A;
			IL_9C:
			yield return PlayerFields.EarthDamageArmor;
			yield return PlayerFields.NeutralDamageArmor;
			IL_11A:
			yield break;
		}
	}
}
