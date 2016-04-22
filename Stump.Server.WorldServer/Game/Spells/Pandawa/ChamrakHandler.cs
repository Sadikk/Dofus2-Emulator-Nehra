using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Spells.Casts;
using Stump.Server.WorldServer.Handlers.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stump.Server.WorldServer.Game.Spells.Pandawa
{
    [SpellCastHandler(SpellIdEnum.Chamrak)]
    class ChamrakHandler : DefaultSpellCastHandler
    {
        public ChamrakHandler(FightActor caster, Spell spell, Cell targetedCell, bool critical)  : base(caster, spell, targetedCell, critical)
        {
        }

        public override void Execute()
        {
            if (this.Caster.Carrier != null && this.Caster.Carrier == this.Caster && this.Caster.Carried != null)
            {
                var buff = this.Caster.GetBuffs().Where((x) => x.Spell.Id == (int)SpellIdEnum.Karcham).First();
                var target = this.Caster.Carried;

                this.Caster.RemoveAndDispellBuff(buff);
                target.RemoveAndDispellBuff(buff);
            }
            
            base.Execute();
        }
    }
}
