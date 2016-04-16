using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Fights.Buffs.Customs;
using Stump.Server.WorldServer.Game.Spells;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Debuffs
{
	[EffectHandler(EffectsEnum.Effect_SkipTurn)]
	public class SkipTurn : SpellEffectHandler
	{
		public SkipTurn(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
		{
		}
		public override bool Apply()
		{
			foreach (FightActor current in base.GetAffectedActors())
			{
				SkipTurnBuff buff = new SkipTurnBuff(current.PopNextBuffId(), current, base.Caster, base.Dice, base.Spell, false, true);
				current.AddAndApplyBuff(buff, true);
			}
			return true;
		}
	}
}
