using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Spells;
using System;
namespace Stump.Server.WorldServer.Game.Fights.Buffs.Customs
{
	public class SkipTurnBuff : Buff
	{
		public SkipTurnBuff(int id, FightActor target, FightActor caster, EffectBase effect, Spell spell, bool critical, bool dispelable) : base(id, target, caster, effect, spell, critical, dispelable)
		{
		}
		public SkipTurnBuff(int id, FightActor target, FightActor caster, EffectBase effect, Spell spell, bool critical, bool dispelable, short customActionId) : base(id, target, caster, effect, spell, critical, dispelable, customActionId)
		{
		}
		public override void Apply()
		{
		}
		public override void Dispell()
		{
		}
		public override AbstractFightDispellableEffect GetAbstractFightDispellableEffect()
		{
			return new FightTemporaryBoostEffect((uint)base.Id, base.Target.Id, base.Duration, Convert.ToSByte(base.Dispellable ? 0 : 1), (ushort)base.Spell.Id, 0, 0, 0);
		}
	}
}
