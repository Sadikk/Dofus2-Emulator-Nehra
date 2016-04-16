using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Actors.Look;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Spells;
using Stump.Server.WorldServer.Handlers.Actions;
using System;
namespace Stump.Server.WorldServer.Game.Fights.Buffs.Customs
{
	public class SkinBuff : Buff
	{
		public ActorLook Look
		{
			get;
			set;
		}
		public ActorLook OriginalLook
		{
			get;
			private set;
		}
		public SkinBuff(int id, FightActor target, FightActor caster, EffectBase effect, ActorLook look, Spell spell, bool dispelable) : base(id, target, caster, effect, spell, false, dispelable)
		{
			this.Look = look;
		}
		public SkinBuff(int id, FightActor target, FightActor caster, EffectBase effect, ActorLook look, Spell spell, bool dispelable, short customActionId) : base(id, target, caster, effect, spell, false, dispelable, customActionId)
		{
			this.Look = look;
		}
		public override void Apply()
		{
			this.OriginalLook = base.Target.Look.Clone();
			base.Target.Look = this.Look.Clone();
			ActionsHandler.SendGameActionFightChangeLookMessage(base.Target.Fight.Clients, base.Caster, base.Target, base.Target.Look);
		}
		public override void Dispell()
		{
			base.Target.Look = this.OriginalLook.Clone();
			ActionsHandler.SendGameActionFightChangeLookMessage(base.Target.Fight.Clients, base.Caster, base.Target, base.Target.Look);
		}
		public override AbstractFightDispellableEffect GetAbstractFightDispellableEffect()
		{
            return new FightTemporaryBoostEffect((uint)base.Id, base.Target.Id, base.Duration, Convert.ToSByte(base.Dispellable ? 1 : 0), (ushort)base.Spell.Id, 0, 0, 0);
		}
	}
}
