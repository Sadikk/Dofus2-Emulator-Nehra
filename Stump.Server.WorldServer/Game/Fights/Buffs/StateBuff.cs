using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Database.Spells;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Spells;
using System;
namespace Stump.Server.WorldServer.Game.Fights.Buffs
{
	public class StateBuff : Buff
	{
		public SpellState State
		{
			get;
			private set;
		}
		public StateBuff(int id, FightActor target, FightActor caster, EffectBase effect, Spell spell, bool dispelable, SpellState state) : base(id, target, caster, effect, spell, false, dispelable)
		{
			this.State = state;
		}
		public StateBuff(int id, FightActor target, FightActor caster, EffectBase effect, Spell spell, bool dispelable, short customActionId, SpellState state) : base(id, target, caster, effect, spell, false, dispelable, customActionId)
		{
			this.State = state;
		}
		public override void Apply()
		{
			base.Target.AddState(this.State);
		}
		public override void Dispell()
		{
			base.Target.RemoveState(this.State);
		}
		public override AbstractFightDispellableEffect GetAbstractFightDispellableEffect()
		{
            return new FightTemporaryBoostStateEffect((uint)base.Id, base.Target.Id, base.Duration, Convert.ToSByte(base.Dispellable ? 1 : 0), (ushort)base.Spell.Id, 0, 0, 1, (short)this.State.Id);
		}
	}
}
