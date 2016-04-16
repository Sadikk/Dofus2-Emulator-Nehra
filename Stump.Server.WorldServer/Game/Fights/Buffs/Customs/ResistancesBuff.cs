using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Spells;
using System;
namespace Stump.Server.WorldServer.Game.Fights.Buffs.Customs
{
	public class ResistancesBuff : Buff
	{
		public short Value
		{
			get;
			private set;
		}
		public ResistancesBuff(int id, FightActor target, FightActor caster, EffectBase effect, Spell spell, short value, bool critical, bool dispelable) : base(id, target, caster, effect, spell, critical, dispelable)
		{
			this.Value = value;
		}
		public override void Apply()
		{
			base.Target.Stats[PlayerFields.AirResistPercent].Context += ((base.Target.Stats[PlayerFields.AirResistPercent].Total + (int)this.Value > 50) ? (50 - base.Target.Stats[PlayerFields.AirResistPercent].Total) : ((int)this.Value));
			base.Target.Stats[PlayerFields.FireResistPercent].Context += ((base.Target.Stats[PlayerFields.FireResistPercent].Total + (int)this.Value > 50) ? (50 - base.Target.Stats[PlayerFields.FireResistPercent].Total) : ((int)this.Value));
			base.Target.Stats[PlayerFields.EarthResistPercent].Context += ((base.Target.Stats[PlayerFields.EarthResistPercent].Total + (int)this.Value > 50) ? (50 - base.Target.Stats[PlayerFields.EarthResistPercent].Total) : ((int)this.Value));
			base.Target.Stats[PlayerFields.NeutralResistPercent].Context += ((base.Target.Stats[PlayerFields.NeutralResistPercent].Total + (int)this.Value > 50) ? (50 - base.Target.Stats[PlayerFields.NeutralResistPercent].Total) : ((int)this.Value));
			base.Target.Stats[PlayerFields.WaterResistPercent].Context += ((base.Target.Stats[PlayerFields.WaterResistPercent].Total + (int)this.Value > 50) ? (50 - base.Target.Stats[PlayerFields.WaterResistPercent].Total) : ((int)this.Value));
		}
		public override void Dispell()
		{
			base.Target.Stats[PlayerFields.AirResistPercent].Context -= (int)this.Value;
			base.Target.Stats[PlayerFields.FireResistPercent].Context -= (int)this.Value;
			base.Target.Stats[PlayerFields.EarthResistPercent].Context -= (int)this.Value;
			base.Target.Stats[PlayerFields.NeutralResistPercent].Context -= (int)this.Value;
			base.Target.Stats[PlayerFields.WaterResistPercent].Context -= (int)this.Value;
		}
		public override AbstractFightDispellableEffect GetAbstractFightDispellableEffect()
		{
            return new FightTemporaryBoostEffect((uint)base.Id, base.Target.Id, base.Duration, Convert.ToSByte(base.Dispellable ? 0 : 1), (ushort)base.Spell.Id, 0, 0, this.Value);
		}
	}
}
