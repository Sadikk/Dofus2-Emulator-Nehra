using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Fights.Buffs.Customs;
using Stump.Server.WorldServer.Game.Spells;

namespace Stump.Server.WorldServer.Game.Effects.Spells.States
{
	[EffectHandler(EffectsEnum.Effect_Invisibility)]
	public class Invisibility : SpellEffectHandler
	{
		public Invisibility(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
		{
		}
		public override bool Apply()
		{
			foreach (FightActor current in base.GetAffectedActors())
			{
				InvisibilityBuff buff = new InvisibilityBuff(current.PopNextBuffId(), current, base.Caster, base.Dice, base.Spell, false, true);
				current.AddAndApplyBuff(buff, true);
			}
			return true;
		}
	}
}
