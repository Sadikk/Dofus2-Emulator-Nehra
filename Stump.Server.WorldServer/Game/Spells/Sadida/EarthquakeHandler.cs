using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Enums.Extensions;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Spells;
using Stump.Server.WorldServer.Game.Effects.Spells.Damage;
using Stump.Server.WorldServer.Game.Maps.Cells;
using Stump.Server.WorldServer.Game.Maps.Cells.Shapes;
using Stump.Server.WorldServer.Game.Spells.Casts;
using Stump.Server.WorldServer.Handlers.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stump.Server.WorldServer.Game.Spells.Sadida
{
    [SpellCastHandler(SpellIdEnum.Earthquake)]
    class EarthquakeHandler : DefaultSpellCastHandler
    {
        //CONST
        private const int RANGE = 3;
        
        public EarthquakeHandler(FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(caster, spell, targetedCell, critical)
        {
        }

        public override void Execute()
        {
            var trees = from entry
                        in this.Caster.GetSummons()
                        where (entry as SummonedMonster).IsLeafyTree
                        select entry;

            foreach (var tree in trees)
            {
                var affectedMapPoints = tree.Position.Point.GetAllCellsInRange(1, RANGE, false, null).OrderBy((x) => tree.Position.Point.DistanceTo(x));
                var affectedCells = from mapPoint
                                     in affectedMapPoints
                                    select this.Fight.Cells.Where((x) => x.Id == mapPoint.CellId).First();
                var affectedActors = this.Fight.GetAllFighters(affectedCells);

                for(int i = 0; i < this.Handlers.Length; i++)
                {
                    var handler = this.Handlers[i];
                    handler.SetAffectedActors(affectedActors);
                    handler.Apply();
                }
            }
        }
    }
}
