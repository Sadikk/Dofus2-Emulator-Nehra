using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Spells;
using System;
namespace Stump.Server.WorldServer.Game.Fights.Buffs.Customs
{
	public class DodgeBuff : Buff
	{
		public int DodgePercent
		{
			get;
			set;
		}
		public int BackCellsCount
		{
			get;
			set;
		}
		public DodgeBuff(int id, FightActor target, FightActor caster, EffectBase effect, Spell spell, bool critical, bool dispelable, int dodgePercent, int backCellsCount) : base(id, target, caster, effect, spell, critical, dispelable)
		{
			this.DodgePercent = dodgePercent;
			this.BackCellsCount = backCellsCount;
		}
		public DodgeBuff(int id, FightActor target, FightActor caster, EffectBase effect, Spell spell, bool critical, bool dispelable, short customActionId, int dodgePercent, int backCellsCount) : base(id, target, caster, effect, spell, critical, dispelable, customActionId)
		{
			this.DodgePercent = dodgePercent;
			this.BackCellsCount = backCellsCount;
		}
		public override void Apply()
		{
		}
		public override void Dispell()
		{
		}
		public override AbstractFightDispellableEffect GetAbstractFightDispellableEffect()
		{
			object[] values = base.Effect.GetValues();
            return new FightTriggeredEffect((uint)base.Id, base.Target.Id, base.Duration, Convert.ToSByte(base.Dispellable ? 0 : 1), (ushort)base.Spell.Id, 0, 0, (int)((short)values[0]), (int)((short)values[1]), (int)((short)values[2]), 0);
		}
	}
}
