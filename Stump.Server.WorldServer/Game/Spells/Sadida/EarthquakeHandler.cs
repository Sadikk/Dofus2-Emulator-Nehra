using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Maps.Cells;
using Stump.Server.WorldServer.Game.Spells.Casts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stump.Server.WorldServer.Game.Spells.Sadida
{
    class EarthquakeHandler : DefaultSpellCastHandler
    {
        //CONST
        private const int LINE_RANGE = 3;
        private const int DIAGONAL_RANGE = 2;
        
        public EarthquakeHandler(FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(caster, spell, targetedCell, critical)
        {
        }

        public override void Execute()
        {
            var newHandlers = from handler
                              in this.Handlers
                              where handler.Effect.Id != 1
                              select handler;
            var trees = from tree
                        in this.Caster.GetTrees()
                        where tree.IsLeafyTree
                        select tree;

            foreach(var tree in trees)
            {
                
            }

            base.Execute();
        }
    }
}
