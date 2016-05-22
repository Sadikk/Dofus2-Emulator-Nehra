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
using Stump.Server.WorldServer.Game.Fights.Buffs;
using Stump.Server.WorldServer.Handlers.Context;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Summon
{
    [EffectHandler(EffectsEnum.Effect_KillAndSummon)]
    public class KillAndSummon : SpellEffectHandler
    {
        public KillAndSummon(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
        {
        }

        public override bool Apply()
        {
            var target = this.Caster.Team.GetOneTree(this.TargetedCell);
            if (target != null && target.Summoner.Id == this.Caster.Id && target.HasState((int)SpellStatesEnum.Leafy))
            {
                var monsterGrade = Singleton<MonsterManager>.Instance.GetMonsterGrade(base.Dice.DiceNum, this.Spell.CurrentLevel);
                var summonedMonster = new SummonedMonster(this.Fight.GetNextContextualId(), this.Caster.Team, this.Caster, monsterGrade, target.Cell, true, true);
                target.Die();

                this.Caster.AddSummon(summonedMonster);
                this.Caster.Team.AddFighter(summonedMonster);

                ActionsHandler.SendGameActionFightSummonMessage(base.Fight.Clients, summonedMonster);
                base.AddTriggerBuff(base.Caster, true, BuffTriggerType.TURN_END, new TriggerBuffApplyHandler(KillAndSummon.ApplySwitchBuff));
                return true;
            }     
            return false;
        }
        public static void ApplySwitchBuff(TriggerBuff buff, BuffTriggerType trigger, object token)
        {
            ContextHandler.SendSlaveSwitchContextMessage(buff.Target.Fight.Clients, buff.Target, buff.Target.GetSummons().Where(x => (x as SummonedMonster).Monster.Template.Id == 4010).FirstOrDefault());
        }
        
    }
}