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
using Stump.Server.WorldServer.Handlers.Shortcuts;
using Stump.Server.WorldServer.Handlers.Inventory;

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
                summonedMonster.Frozen = true;
                ActionsHandler.SendGameActionFightSummonMessage(base.Fight.Clients, summonedMonster);
                base.AddTriggerBuff(base.Caster, true, BuffTriggerType.TURN_END, new TriggerBuffApplyHandler(KillAndSummon.ApplySwitchBuff));
                var buff = new TriggerBuff(summonedMonster.PopNextBuffId(), summonedMonster, base.Caster, new EffectDice() { Duration = 1 }, base.Spell, false, false, BuffTriggerType.TURN_END, new TriggerBuffApplyHandler(KillAndSummon.ApplySuicideBuff), (short)ActionsEnum.ACTION_FIGHT_KILL_AND_SUMMON);
                summonedMonster.AddAndApplyBuff(buff);
                return true;
            }     
            return false;
        }
        public static void ApplySwitchBuff(TriggerBuff buff, BuffTriggerType trigger, object token)
        {
            ContextHandler.SendSlaveSwitchContextMessage(buff.Target.Fight.Clients, buff.Target, buff.Target.GetSummons().Where(x => (x as SummonedMonster).Monster.Template.Id == 4010).FirstOrDefault());
        }
        public static void ApplySuicideBuff(TriggerBuff buff, BuffTriggerType trigger, object token)
        {
            var target = buff.Target as SummonedMonster;
            InventoryHandler.SendSpellListMessage(target.Summoner.CharacterContainer.Clients.FirstOrDefault(), true);
            ShortcutHandler.SendShortcutBarContentMessage(target.Summoner.CharacterContainer.Clients.FirstOrDefault(), ShortcutBarEnum.SPELL_SHORTCUT_BAR);
            //ContextHandler.SendSlaveSwitchContextMessage(buff.Target.s);
            target.Die();
        }

    }
}