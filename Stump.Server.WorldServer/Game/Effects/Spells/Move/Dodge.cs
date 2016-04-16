using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Fights.Buffs.Customs;
using Stump.Server.WorldServer.Game.Spells;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Move
{
	[EffectHandler(EffectsEnum.Effect_Dodge)]
	public class Dodge : SpellEffectHandler
	{
		public Dodge(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
		{
		}
		public override bool Apply()
		{
			foreach (FightActor current in base.GetAffectedActors())
			{
				DodgeBuff buff = new DodgeBuff(current.PopNextBuffId(), current, base.Caster, base.Dice, base.Spell, base.Critical, true, (int)base.Dice.DiceNum, (int)base.Dice.DiceFace);
				current.AddAndApplyBuff(buff, true);
			}
			return true;
		}
	}
}
