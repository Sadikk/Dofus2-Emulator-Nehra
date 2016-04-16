using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Spells;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Effects.Spells;
using Stump.Server.WorldServer.Game.Maps.Cells;
using Stump.Server.WorldServer.Game.Maps.Cells.Shapes;
using Stump.Server.WorldServer.Game.Spells.Casts;

namespace Stump.Server.WorldServer.Game.Fights.Triggers
{
    public class BombWall
    {
        // FIELDS
        public const int MAX_BOMB_DISTANCE = 8;

        private List<BombWallTrigger> m_wall;

        // PROPERTIES
        public Fight Fight { get; private set; }
        public FightActor Caster { get; private set; }
        public Spell Spell { get; private set; }

        public BombFighter From { get; private set; }
        public BombFighter To { get; private set; }

        // CONSTRUCTORS
        public BombWall(Fight fight, FightActor caster, Spell castedSpell, BombFighter from, BombFighter to)
        {
            this.m_wall = new List<BombWallTrigger>();

            this.Fight = fight;
            this.Caster = caster;
            this.Spell = castedSpell;

            this.From = from;
            this.To = to;
        }

        // METHODS
        public void MarkWall()
        {
            var cells = this.From.Position.Point.GetCellsOnLineBetween(this.To.Position.Point);
            foreach (var cell in cells)
            {
                var trigger = new BombWallTrigger((short)this.Fight.PopNextTriggerId(), this.Caster, this.Spell,
                    this.Fight.Map.GetCell(cell.CellId), this.Spell.CurrentSpellLevel.Effects[0]);

                this.Fight.AddTrigger(trigger);
                this.m_wall.Add(trigger);
            }

            this.Activate();
        }

        public void UnmarkWall()
        {
            this.m_wall.ForEach(entry => entry.Remove());
        }

        public void Activate()
        {
            foreach (var bombWallTrigger in this.m_wall)
            {
                var fighter = this.Fight.GetOneFighter(bombWallTrigger.CenterCell);
                if (fighter != null)
                {
                    bombWallTrigger.Trigger(fighter);
                }
            }
        }

        public void Detonate()
        {
            this.Activate();

            this.UnmarkWall();
        }
    }

    public class BombWallTrigger : MarkTrigger
    {
        // FIELDS

        // PROPERTIES
        public override GameActionMarkTypeEnum Type
        {
            get { return GameActionMarkTypeEnum.WALL; }
        }
        public override TriggerTypeFlag TriggerType
        {
            get { return TriggerTypeFlag.MOVE | TriggerTypeFlag.TURN_BEGIN | TriggerTypeFlag.TURN_END; }
        }

        // CONSTRUCTORS
        public BombWallTrigger(short id, FightActor caster, Spell castedSpell, Cell centerCell, EffectDice originEffect)
            : base(id, caster, castedSpell, centerCell, originEffect, new MarkShape[]
            {
                new MarkShape(caster.Fight, centerCell, GameActionMarkCellsTypeEnum.CELLS_CIRCLE, 0, Color.Brown)
            })
        {
            
        }

        // METHODS
        public override void Trigger(FightActor trigger)
        {
            base.NotifyTriggered(trigger, this.CastedSpell);
            MarkShape[] shapes = base.Shapes;
            for (int i = 0; i < shapes.Length; i++)
            {
                MarkShape markShape = shapes[i];
                SpellCastHandler spellCastHandler = Singleton<SpellManager>.Instance.GetSpellCastHandler(base.Caster, this.CastedSpell, markShape.Cell, false);
                spellCastHandler.MarkTrigger = this;
                spellCastHandler.Initialize();
                foreach (SpellEffectHandler current in spellCastHandler.GetEffectHandlers())
                {
                    current.EffectZone = new Zone(current.Effect.ZoneShape, markShape.Size);
                }
                spellCastHandler.Execute();
            }
        }

        public override GameActionMark GetGameActionMark()
        {
            return new GameActionMark(base.Caster.Id, base.Caster.Team.Id, base.CastedSpell.Id,
                (sbyte) base.CastedSpell.CurrentLevel, base.Id, (sbyte) this.Type, 2,
                from entry in base.Shapes
                select entry.GetGameActionMarkedCell(), true);
        }

        public override GameActionMark GetHiddenGameActionMark()
        {
            return this.GetGameActionMark();
        }

        public override bool DoesSeeTrigger(FightActor fighter)
        {
            return true;
        }

        public override bool DecrementDuration()
        {
            return false;
        }
    }
}
