using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Actors.Look;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Fights.Buffs.Customs;
using Stump.Server.WorldServer.Game.Spells;

namespace Stump.Server.WorldServer.Game.Effects.Spells.States
{
	[EffectHandler(EffectsEnum.Effect_ChangeAppearance), EffectHandler(EffectsEnum.Effect_ChangeAppearance_335)]
	public class ChangeSkin : SpellEffectHandler
	{
        public ChangeSkin(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical)
            : base(effect, caster, spell, targetedCell, critical)
        {
        }

		public override bool Apply()
		{
			foreach (FightActor current in base.GetAffectedActors())
			{
				ActorLook actorLook = current.Look.Clone();
                actorLook.BonesID = base.Dice.Value;
				SkinBuff buff = new SkinBuff(current.PopNextBuffId(), current, base.Caster, base.Dice, actorLook, base.Spell, true);
				current.AddAndApplyBuff(buff, true);
			}
			return true;
		}
	}
}
