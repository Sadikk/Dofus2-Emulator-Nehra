using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Enums.HomeMade;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Maps.Cells;
using Stump.Server.WorldServer.Game.Spells.Casts;
using Stump.Server.WorldServer.Handlers.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stump.Server.WorldServer.Game.Spells.Pandawa
{
    [SpellCastHandler(SpellIdEnum.TemporalParadox)]
    public class TemporalParadoxHandler : DefaultSpellCastHandler
    {
        public TemporalParadoxHandler(FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(caster, spell, targetedCell, critical)
        {
        }

        //CONST
        private const int RANGE = 4;

        public override void Execute()
        {

            var affectedMapPoints = new MapPoint(this.TargetedCell).GetAllCellsInRange(1, RANGE, false, null).OrderBy((x) => new MapPoint(this.TargetedCell));
            var affectedCells = from mapPoint
                                 in affectedMapPoints
                                select this.Fight.Cells.Where((x) => x.Id == mapPoint.CellId).First();
            var affectedActors = this.Fight.GetAllFighters(affectedCells).Where(x => x != base.Caster);

            for (int i = 0; i < this.Handlers.Length; i++)
            {
                var handler = this.Handlers[i];
                handler.SetAffectedActors(affectedActors);
                handler.Apply();
            }
        }
    }
}