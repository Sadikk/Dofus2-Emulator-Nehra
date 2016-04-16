using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Spells;
using System;

namespace Stump.Server.WorldServer.Game.Fights.Buffs
{
	public class StatBuff : Buff
	{
		public short Value
		{
			get;
			private set;
		}
		public PlayerFields Caracteristic
		{
			get;
			set;
		}
		public StatBuff(int id, FightActor target, FightActor caster, EffectBase effect, Spell spell, short value, PlayerFields caracteristic, bool critical, bool dispelable) : base(id, target, caster, effect, spell, critical, dispelable)
		{
			this.Value = value;
			this.Caracteristic = caracteristic;
		}
		public StatBuff(int id, FightActor target, FightActor caster, EffectBase effect, Spell spell, short value, PlayerFields caracteristic, bool critical, bool dispelable, short customActionId) : base(id, target, caster, effect, spell, critical, dispelable, customActionId)
		{
			this.Value = value;
			this.Caracteristic = caracteristic;
		}
		public override void Apply()
		{
			base.Target.Stats[this.Caracteristic].Context += (int)this.Value;
		}
		public override void Dispell()
		{
			base.Target.Stats[this.Caracteristic].Context -= (int)this.Value;
		}
		public override AbstractFightDispellableEffect GetAbstractFightDispellableEffect()
		{
            return new FightTemporaryBoostEffect((uint)base.Id, base.Target.Id, base.Duration, Convert.ToSByte(base.Dispellable ? 0 : 1), (ushort)base.Spell.Id, 0, 0, System.Math.Abs(this.Value));
		}
	}
}
