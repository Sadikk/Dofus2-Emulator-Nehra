using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Fights.Buffs.Customs;
using Stump.Server.WorldServer.Game.Spells;
using Stump.Server.WorldServer.Handlers.Actions;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Damage
{
	[EffectHandler(EffectsEnum.Effect_Punishment_Damage)]
	public class PunishmentDamage : SpellEffectHandler
	{
		public PunishmentDamage(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
		{
		}
		public override bool Apply()
		{
			foreach (FightActor current in base.GetAffectedActors())
			{
                Fights.Damage damage = new Fights.Damage(base.Dice);
				damage.MarkTrigger = base.MarkTrigger;
				damage.GenerateDamages();
				double num = 0.0;
				double num2 = (double)base.Caster.LifePoints / (double)base.Caster.MaxLifePoints;
				if (num2 <= 0.5)
				{
					num = 2.0 * num2;
				}
				else
				{
					if (num2 > 0.5)
					{
						num = 1.0 + (num2 - 0.5) * -2.0;
					}
				}
				damage.Amount = (int)((double)base.Caster.LifePoints * num * (double)damage.Amount / 100.0);
				SpellReflectionBuff bestReflectionBuff = current.GetBestReflectionBuff();
				if (bestReflectionBuff != null && bestReflectionBuff.ReflectedLevel >= (int)base.Spell.CurrentLevel && base.Spell.Template.Id != 0)
				{
					this.NotifySpellReflected(current);
					damage.Source = current;
					damage.ReflectedDamages = true;
					damage.IgnoreDamageBoost = true;
					base.Caster.InflictDamage(damage);
					current.RemoveAndDispellBuff(bestReflectionBuff);
				}
				else
				{
					current.InflictDamage(damage);
				}
			}
			return true;
		}
		private void NotifySpellReflected(FightActor source)
		{
			ActionsHandler.SendGameActionFightReflectSpellMessage(base.Fight.Clients, source, base.Caster);
		}
	}
}
