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

                var damages = this.Handlers.Where((x) => x.Effect.EffectId == EffectsEnum.Effect_DamageFire).First();
                var revealsInvisible = this.Handlers.Where((x) => x.Effect.EffectId == EffectsEnum.Effect_RevealsInvisible).First();
                var pull = this.Handlers.Where((x) => x.Effect.EffectId == EffectsEnum.Effect_PullForward).First();

                damages.SetAffectedActors(affectedActors);
                revealsInvisible.SetAffectedActors(affectedActors);
                pull.SetAffectedActors(affectedActors);

                //foreach (var actor in affectedActors)
                //{
                //    if (actor != null)
                //    {
                //        var orientation = actor.Position.Point.OrientationTo(tree.Position.Point).GetPullableDirection();

                //        var newCellId = actor.Position.Point.GetNearestCellInDirection(orientation).CellId;
                //        var newCell = this.Fight.Cells.Where(cell => cell.Id == newCellId).First();

                //        if (this.Fight.IsCellFree(newCell) && actor.CanBeMove())
                //        {
                //            ActionsHandler.SendGameActionFightSlideMessage(this.Fight.Clients, tree, actor, actor.Cell.Id, newCellId);
                //            actor.Cell = newCell;
                //        }

                //        damages.Apply();
                //        revealsInvisible.Apply();
                //    }
                //}

                damages.Apply();
                revealsInvisible.Apply();
                pull.Apply();
            }
        }
    }
}
