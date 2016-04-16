using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Spells;
using System;

namespace Stump.Server.WorldServer.Game.Fights.Buffs
{
	public class SpellBuff : Buff
	{
		public Spell BoostedSpell
		{
			get;
			private set;
		}
		public short Boost
		{
			get;
			private set;
		}

        public SpellBuff(int id, FightActor target, FightActor caster, EffectBase effect, Spell spell, Spell boostedSpell, short boost, bool critical, bool dispelable)
            : base(id, target, caster, effect, spell, critical, dispelable)
        {
            this.BoostedSpell = boostedSpell;
            this.Boost = boost;
        }
        public SpellBuff(int id, FightActor target, FightActor caster, EffectBase effect, Spell spell, Spell boostedSpell, short boost, bool critical, bool dispelable, short customActionId)
            : base(id, target, caster, effect, spell, critical, dispelable, customActionId)
        {
            this.BoostedSpell = boostedSpell;
            this.Boost = boost;
        }

		public override void Apply()
		{
			base.Target.BuffSpell(this.BoostedSpell, this.Boost);
		}
		public override void Dispell()
		{
			base.Target.UnBuffSpell(this.BoostedSpell, this.Boost);
		}
		public override AbstractFightDispellableEffect GetAbstractFightDispellableEffect()
		{
            return new FightTemporarySpellBoostEffect((uint)base.Id, base.Target.Id, base.Duration, Convert.ToSByte(base.Dispellable ? 1 : 0), (ushort)base.Spell.Id, 0, 0, this.Boost, (ushort)this.BoostedSpell.Id);
		}
	}
}
