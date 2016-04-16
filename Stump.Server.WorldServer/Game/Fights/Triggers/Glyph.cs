using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Spells;
using Stump.Server.WorldServer.Game.Spells.Casts;
using System.Drawing;
using System.Linq;
using Stump.Server.WorldServer.Database.World;

namespace Stump.Server.WorldServer.Game.Fights.Triggers
{
    public class Glyph : MarkTrigger
    {
        private static readonly int[] SPELLS_GLYPH_END_TURN = new int[]
        {
            13,
            2035
        };

        public Spell GlyphSpell { get; private set; }
        public int Duration { get; private set; }

        public override GameActionMarkTypeEnum Type
        {
            get { return GameActionMarkTypeEnum.GLYPH; }
        }

        public override TriggerTypeFlag TriggerType
        {
            get
            {
                return Glyph.SPELLS_GLYPH_END_TURN.Contains(base.CastedSpell.Id)
                    ? TriggerTypeFlag.TURN_END
                    : TriggerTypeFlag.TURN_BEGIN;
            }
        }

        public Glyph(short id, FightActor caster, Spell castedSpell, Cell centerCell, EffectDice originEffect,
            Spell glyphSpell, byte size, Color color)
            : base(id, caster, castedSpell, centerCell, originEffect, new MarkShape[]
            {
                new MarkShape(caster.Fight, centerCell, GameActionMarkCellsTypeEnum.CELLS_CIRCLE, size, color)
            })
        {
            this.GlyphSpell = glyphSpell;
            this.Duration = originEffect.Duration;
        }

        public Glyph(short id, FightActor caster, Spell castedSpell, Cell centerCell, EffectDice originEffect,
            Spell glyphSpell, GameActionMarkCellsTypeEnum type, byte size, Color color)
            : base(id, caster, castedSpell, centerCell, originEffect, new MarkShape[]
            {
                new MarkShape(caster.Fight, centerCell, type, size, color)
            })
        {
            this.GlyphSpell = glyphSpell;
            this.Duration = originEffect.Duration;
        }

        public override bool DecrementDuration()
        {
            return this.Duration-- <= 0;
        }

        public override void Trigger(FightActor trigger)
        {
            base.NotifyTriggered(trigger, this.GlyphSpell);
            SpellCastHandler spellCastHandler = Singleton<SpellManager>.Instance.GetSpellCastHandler(base.Caster,
                this.GlyphSpell, trigger.Cell, false);
            spellCastHandler.MarkTrigger = this;
            spellCastHandler.Initialize();
            spellCastHandler.Execute();
        }

        public override GameActionMark GetGameActionMark()
        {
            return new GameActionMark(base.Caster.Id, Caster.Team.Id, base.CastedSpell.Id, 1, base.Id, (sbyte) this.Type,
                1,
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
    }
}
