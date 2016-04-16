using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Spells;
using System;

namespace Stump.Server.WorldServer.Game.Fights.Buffs
{
	public class TriggerBuff : Buff
	{
        // FIELDS
        private int m_roundCreation;

        // PROPERTIES
        public BuffTriggerType Trigger
        {
            get;
            private set;
        }
        public EffectDice Dice
        {
            get;
            private set;
        }
        public TriggerBuffApplyHandler ApplyTrigger
        {
            get;
            private set;
        }
        public TriggerBuffRemoveHandler RemoveTrigger
        {
            get;
            private set;
        }

        // CONSTRUCTORS
        public TriggerBuff(int id, FightActor target, FightActor caster, EffectDice effect, Spell spell, bool critical, bool dispelable, BuffTriggerType trigger, TriggerBuffApplyHandler applyTrigger)
            : base(id, target, caster, effect, spell, critical, dispelable)
        {
            this.Trigger = trigger;
            this.Dice = effect;
            this.ApplyTrigger = applyTrigger;

            this.m_roundCreation = target.Fight.TimeLine.RoundNumber;
        }
        public TriggerBuff(int id, FightActor target, FightActor caster, EffectDice effect, Spell spell, bool critical, bool dispelable, BuffTriggerType trigger, TriggerBuffApplyHandler applyTrigger, TriggerBuffRemoveHandler removeTrigger)
            : base(id, target, caster, effect, spell, critical, dispelable)
        {
            this.Trigger = trigger;
            this.Dice = effect;
            this.ApplyTrigger = applyTrigger;
            this.RemoveTrigger = removeTrigger;

            this.m_roundCreation = target.Fight.TimeLine.RoundNumber;
        }
        public TriggerBuff(int id, FightActor target, FightActor caster, EffectDice effect, Spell spell, bool critical, bool dispelable, BuffTriggerType trigger, TriggerBuffApplyHandler applyTrigger, short customActionId)
            : base(id, target, caster, effect, spell, critical, dispelable, customActionId)
        {
            this.Trigger = trigger;
            this.Dice = effect;
            this.ApplyTrigger = applyTrigger;

            this.m_roundCreation = target.Fight.TimeLine.RoundNumber;
        }
        public TriggerBuff(int id, FightActor target, FightActor caster, EffectDice effect, Spell spell, bool critical, bool dispelable, BuffTriggerType trigger, TriggerBuffApplyHandler applyTrigger, TriggerBuffRemoveHandler removeTrigger, short customActionId)
            : base(id, target, caster, effect, spell, critical, dispelable, customActionId)
        {
            this.Trigger = trigger;
            this.Dice = effect;
            this.ApplyTrigger = applyTrigger;
            this.RemoveTrigger = removeTrigger;

            this.m_roundCreation = target.Fight.TimeLine.RoundNumber;
        }

        // METHODS
		public override void Apply()
		{
			if (this.ApplyTrigger != null)
			{
				this.ApplyTrigger(this, BuffTriggerType.UNKNOWN, null);
			}
		}
		public void Apply(BuffTriggerType trigger)
		{
			if (this.ApplyTrigger != null)
			{
				this.ApplyTrigger(this, trigger, null);
			}
		}
		public void Apply(BuffTriggerType trigger, object token)
		{
			if (this.ApplyTrigger != null)
			{
				this.ApplyTrigger(this, trigger, token);
			}
		}

        public override bool DecrementDuration()
        {
            if (Target.Fight.TimeLine.RoundNumber - this.m_roundCreation > this.Effect.Delay)
            {
                return base.DecrementDuration();
            }

            return false;
        }

        public bool IsReady()
        {
            return base.Effect.Delay == 0 || base.Target.Fight.TimeLine.RoundNumber - this.m_roundCreation >= this.Effect.Delay;
        }

		public override void Dispell()
		{
			if (this.RemoveTrigger != null)
			{
				this.RemoveTrigger(this);
			}
		}

		public override AbstractFightDispellableEffect GetAbstractFightDispellableEffect()
		{
			object[] values = base.Effect.GetValues();
            return new FightTriggeredEffect((uint)base.Id, base.Target.Id, base.Duration, Convert.ToSByte(base.Dispellable ? 0 : 1), (ushort)base.Spell.Id, 0, 0, (int)((short)values[0]), (int)((short)values[1]), (int)((short)values[2]), (short)base.Effect.Delay);
		}
	}
}
