using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Enums.HomeMade;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Fights.Buffs.Customs;
using Stump.Server.WorldServer.Game.Spells.Casts;
using Stump.Server.WorldServer.Handlers.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stump.Server.WorldServer.Game.Spells.Sadida
{
    [SpellCastHandler(SpellIdEnum.Tear)]
    class TearHandler : DefaultSpellCastHandler
    {
        public TearHandler(FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(caster, spell, targetedCell, critical)
        {

        }
        public override void Execute()
        {
            var target = this.Fight.GetOneFighter(this.TargetedCell);
            if(target != null)
            {
                var summonedMonster = target as SummonedMonster;
                if (summonedMonster != null && summonedMonster.IsTree && !summonedMonster.IsLeafyTree)
                {
                    var look = summonedMonster.Look.Clone();
                    look.BonesID = TreeHandler.BONES_ID;
                    summonedMonster.Look = look;

                    ActionsHandler.SendGameActionFightChangeLookMessage(this.Fight.Clients, summonedMonster, summonedMonster, look);

                    var state = Singleton<SpellManager>.Instance.GetSpellState((uint)SpellStatesEnum.Leafy);
                    summonedMonster.AddState(state);

                    return;
                }
                base.Execute();
            }
        }
    }
}
