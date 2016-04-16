using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Spells;
using System;
namespace Stump.Server.WorldServer.Game.Fights.Buffs.Customs
{
	public class InvisibilityBuff : Buff
	{
		public InvisibilityBuff(int id, FightActor target, FightActor caster, EffectBase effect, Spell spell, bool critical, bool dispelable) : base(id, target, caster, effect, spell, critical, dispelable)
		{
		}
		public InvisibilityBuff(int id, FightActor target, FightActor caster, EffectBase effect, Spell spell, bool critical, bool dispelable, short customActionId) : base(id, target, caster, effect, spell, critical, dispelable, customActionId)
		{
		}
		public override void Apply()
		{
			base.Target.SetInvisibilityState(GameActionFightInvisibilityStateEnum.INVISIBLE);
		}
		public override void Dispell()
		{
			base.Target.SetInvisibilityState(GameActionFightInvisibilityStateEnum.VISIBLE);
		}
		public override AbstractFightDispellableEffect GetAbstractFightDispellableEffect()
		{
			return new FightTemporaryBoostEffect((uint)base.Id, base.Target.Id, base.Duration, Convert.ToSByte(base.Dispellable ? 1 : 0), (ushort)base.Spell.Id, 0, 0, 1);
		}
	}
}
