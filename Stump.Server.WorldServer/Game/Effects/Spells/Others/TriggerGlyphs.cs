
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Fights.Triggers;
using Stump.Server.WorldServer.Game.Spells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Others
{
    [EffectHandler(EffectsEnum.Effect_TriggerGlyphs)]
    public class TriggerGlyphs : SpellEffectHandler
    {
        public TriggerGlyphs(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
        {
        }
        public override bool Apply()
        {
            System.Collections.Generic.IEnumerable<Glyph> enumerable =
                from entry in base.Fight.GetTriggers().OfType<Glyph>()
                where base.Caster.IsEnnemyWith(entry.Caster) && entry.Shapes.Any((MarkShape subentry) => base.AffectedCells.Contains(subentry.Cell))
                select entry;
            foreach (Glyph current in enumerable)
            {
                //todo check if we trigger on all fighters or if we need to add a check if the fighter is in the trigger
                base.Fight.Fighters.ForEach(current.Trigger);
            }
            return true;
        }
    }
}
