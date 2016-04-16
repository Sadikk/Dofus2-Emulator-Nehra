using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Fights.Buffs.Customs;
using Stump.Server.WorldServer.Game.Spells;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Armor
{
	[EffectHandler(EffectsEnum.Effect_ReflectSpell)]
	public class SpellReflection : SpellEffectHandler
	{
		public SpellReflection(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
		{
		}
		public override bool Apply()
		{
			bool result;
			foreach (FightActor current in base.GetAffectedActors())
			{
				if (this.Effect.Duration <= 0)
				{
					result = false;
					return result;
				}
				int id = current.PopNextBuffId();
				SpellReflectionBuff buff = new SpellReflectionBuff(id, current, base.Caster, base.Dice, base.Spell, base.Critical, true);
				current.AddAndApplyBuff(buff, true);
			}
			result = true;
			return result;
		}
	}
}
