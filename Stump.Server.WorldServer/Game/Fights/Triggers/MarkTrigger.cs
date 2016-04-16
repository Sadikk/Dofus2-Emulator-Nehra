using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Spells;
using System;
using System.Linq;

namespace Stump.Server.WorldServer.Game.Fights.Triggers
{
    public abstract class MarkTrigger
    {
        // FIELDS
        public event Action<MarkTrigger, FightActor, Spell> Triggered;

        // PROPERTIES
        public short Id { get; private set; }
        public FightActor Caster { get; private set; }
        public Spell CastedSpell { get; private set; }
        public Cell CenterCell { get; private set; }
        public EffectDice OriginEffect { get; set; }

        public Fight Fight
        {
            get { return this.Caster.Fight; }
        }

        public MarkShape[] Shapes { get; private set; }
        public abstract GameActionMarkTypeEnum Type { get; }
        public abstract TriggerTypeFlag TriggerType { get; }

        // CONSTRUCTORS
        protected MarkTrigger(short id, FightActor caster, Spell castedSpell, Cell centerCell, EffectDice originEffect, params MarkShape[] shapes)
        {
            this.Id = id;
            this.Caster = caster;
            this.CastedSpell = castedSpell;
            this.CenterCell = centerCell;
            this.OriginEffect = originEffect;
            this.Shapes = shapes;
        }

        // METHODS
        protected void NotifyTriggered(FightActor target, Spell triggeredSpell)
        {
            Action<MarkTrigger, FightActor, Spell> triggered = this.Triggered;
            if (triggered != null)
            {
                triggered(this, target, triggeredSpell);
            }
        }

        public bool ContainsCell(Cell cell)
        {
            return this.Shapes.Any((MarkShape entry) => entry.GetCells().Contains(cell));
        }

        public System.Collections.Generic.IEnumerable<Cell> GetCells()
        {
            return this.Shapes.SelectMany((MarkShape entry) => entry.GetCells());
        }

        public virtual void Remove()
        {
            this.Fight.RemoveTrigger(this);
        }

        public abstract void Trigger(FightActor trigger);
        
        public abstract GameActionMark GetGameActionMark();
        public abstract GameActionMark GetHiddenGameActionMark();
        
        public abstract bool DoesSeeTrigger(FightActor fighter);

        public abstract bool DecrementDuration();
    }
}
