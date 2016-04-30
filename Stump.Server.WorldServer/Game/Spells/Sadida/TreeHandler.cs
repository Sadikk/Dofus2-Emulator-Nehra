using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Fights.Buffs;
using Stump.Server.WorldServer.Game.Spells.Casts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stump.Server.WorldServer.Game.Spells.Sadida
{
    [SpellCastHandler(SpellIdEnum.Tree)]
    class TreeHandler : DefaultSpellCastHandler
    {
        //CONSTANTS
        // Need to find the using of them
        private const int arg1 = 5567;
        private const int arg2 = 2;
        private const int delay = 1;

        public TreeHandler(FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(caster, spell, targetedCell, critical)
        {
            
        }

        public override void Execute()
        {

        }
    }
}
