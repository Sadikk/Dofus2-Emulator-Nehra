using System;
using System.Drawing;
using System.Linq;
using NLog;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.Spells;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Monsters;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Fights.Triggers;
using Stump.Server.WorldServer.Game.Spells;
using Stump.Server.WorldServer.Handlers.Actions;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Summon
{

    [EffectHandler(EffectsEnum.Effect_SummonBomb)]
    public class SummonBomb : SpellEffectHandler
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public SummonBomb(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical)
            : base(effect, caster, spell, targetedCell, critical)
        {
        }

        public override bool Apply()
        {
            bool result;
            var monsterGrade = Singleton<MonsterManager>.Instance.GetMonsterGrade((int)base.Dice.DiceNum, (int)base.Dice.DiceFace);

            if (monsterGrade == null)
            {
                SummonBomb.logger.Error<short, short>("Cannot summon monster {0} grade {1} (not found)", base.Dice.DiceNum, base.Dice.DiceFace);
                result = false;
            }
            else
            {
                var spellBombTemplate = Singleton<SpellManager>.Instance.GetSpellBombTemplate(base.Dice.DiceNum);
                if (!this.Fight.IsCellFree(base.TargetedCell))
                {
                    if (spellBombTemplate != null)
                    {
                        var instantSpell = this.GetInstantSpell(spellBombTemplate);
                        if (instantSpell != null)
                        {
                            base.Caster.ForceCastSpell(instantSpell, base.TargetedCell);
                        }
                    }

                    result = false;
                }
                else
                {
                    if (!this.Caster.CanPutBomb())
                    {
                        result = false;
                    }
                    else
                    {
                        var bomb = new BombFighter(base.Fight.GetNextContextualId(), base.Caster.Team, base.Caster, monsterGrade, base.TargetedCell);
                        ActionsHandler.SendGameActionFightSummonMessage(base.Fight.Clients, bomb);

                        var bombs = base.Caster.BombsOfType(monsterGrade.Template)
                            .Where((BombFighter entry) => entry.Position.Point.X == bomb.Position.Point.X || entry.Position.Point.Y == bomb.Position.Point.Y);

                        if (bombs.Any() && spellBombTemplate != null)
                        {
                            var instantSpell = this.GetInstantSpell(spellBombTemplate);
                            if (instantSpell != null)
                            {
                                foreach (var bombFighter in bombs)
                                {
                                    var distance = bombFighter.Position.Point.DistanceToCell(bomb.Position.Point);

                                    if (distance < BombWall.MAX_BOMB_DISTANCE)
                                    {
                                        var trigger = new BombWall(base.Fight, base.Caster, instantSpell, bombFighter, bomb);

                                        // TODO 

                                        trigger.MarkWall();
                                    }
                                }
                            }
                        }

                        base.Caster.AddBomb(bomb);
                        base.Caster.Team.AddFighter(bomb);
                        result = true;
                    }
                }
            }

            return result;
        }

        private Spell GetInstantSpell(SpellBombTemplate spellTemplate)
        {
            var instantSpell = Singleton<SpellManager>.Instance.GetSpellTemplate(spellTemplate.InstantSpellId);
            if (instantSpell != null)
            {
                return new Spell(instantSpell, (byte)base.Dice.DiceFace);
            }

            return null;
        }
    }
}
