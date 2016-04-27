using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Enums.HomeMade;
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
    [SpellCastHandler(SpellIdEnum.LossofMotivation)]
    public class LossOfMotivationHandler : DefaultSpellCastHandler
    {
        //aiguillde
        public LossOfMotivationHandler(FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(caster, spell, targetedCell, critical)
        {
        }

        public override void Execute()
        {

            var target = this.Fight.GetOneFighter(this.TargetedCell);
            if (target != null && target.HasState((int)SpellStatesEnum.Telefrag))
            {
                var buff = target.GetBuffs().Where((x) => x.Id == (int)SpellStatesEnum.Telefrag).FirstOrDefault();
                if (buff != null)
                    target.RemoveAndDispellBuff(buff);
            }
            base.Execute();
            //todo : remove it also on caster ? idk
        }
    }
}