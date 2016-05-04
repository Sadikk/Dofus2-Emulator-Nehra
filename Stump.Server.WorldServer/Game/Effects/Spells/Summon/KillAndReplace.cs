using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Database.Monsters;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Monsters;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Spells;
using Stump.Server.WorldServer.Handlers.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Stump.DofusProtocol.Enums.HomeMade;
using Stump.Core.Reflection;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Summon
{
    [EffectHandler(EffectsEnum.Effect_KillAndReplace)]
    public class KillAndReplace : SpellEffectHandler
    {
        public KillAndReplace(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
		{
        }

        public override bool Apply()
        {
            var target = this.Fight.GetOneFighter(this.TargetedCell) as SummonedMonster;
            if (target != null && target.Monster.Template.Id == (int)MonsterEnum.SADIDA_TREE)
            {
                if (this.Caster.Team.RemoveFighter(target))
                {
                    var monsterGrade = Singleton<MonsterManager>.Instance.GetMonsterGrade(base.Dice.DiceNum, this.Spell.CurrentLevel);
                    var summonedMonster = new SummonedMonster(this.Fight.GetNextContextualId(), this.Caster.Team, this.Caster, monsterGrade, target.Cell);
                    this.Caster.Team.AddFighter(summonedMonster);

                    return true;
                }
            }

            return false;
        }
    }
}
