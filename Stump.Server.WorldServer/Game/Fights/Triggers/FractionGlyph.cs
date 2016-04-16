using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Spells;
using System.Drawing;
using System.Linq;

namespace Stump.Server.WorldServer.Game.Fights.Triggers
{
    public class FractionGlyph : MarkTrigger
    {
        public int Duration { get; private set; }

        public override GameActionMarkTypeEnum Type
        {
            get { return GameActionMarkTypeEnum.GLYPH; }
        }

        public override TriggerTypeFlag TriggerType
        {
            get { return TriggerTypeFlag.NEVER; }
        }

        public FractionGlyph(short id, FightActor caster, Spell castedSpell, Cell centerCell, EffectDice originEffect,
            byte size, Color color) : base(id, caster, castedSpell, centerCell, originEffect, new MarkShape[]
            {
                new MarkShape(caster.Fight, centerCell, GameActionMarkCellsTypeEnum.CELLS_CIRCLE, size, color)
            })
        {
            this.Duration = originEffect.Duration;
        }

        public override void Trigger(FightActor trigger)
        {
            throw new System.NotImplementedException();
        }

        public override GameActionMark GetGameActionMark()
        {
            return new GameActionMark(base.Caster.Id, Caster.Team.Id, base.CastedSpell.Id, 1, base.Id, (sbyte) this.Type,
                1,
                from entry in base.Shapes
                select entry.GetGameActionMarkedCell(), true);
        }

        public override GameActionMark GetHiddenGameActionMark()
        {
            return this.GetGameActionMark();
        }

        public override bool DoesSeeTrigger(FightActor fighter)
        {
            return true;
        }

        public override bool DecrementDuration()
        {
            return this.Duration-- <= 0;
        }

        public int DispatchDamages(Damage damage)
        {
            FightActor[] array = (
                from x in base.GetCells()
                select base.Fight.GetFirstFighter<FightActor>(x)
                into x
                where x != null && !(x is SummonedFighter) && x.IsFriendlyWith(base.Caster)
                select x).ToArray<FightActor>();
            damage.GenerateDamages();
            int averagePercentResistance = this.GetAveragePercentResistance(array, damage.School, damage.PvP);
            int averageFixResistance = this.GetAverageFixResistance(array, damage.School, damage.PvP);
            damage.Amount =
                (int) ((1.0 - (double) averagePercentResistance/100.0)*(double) (damage.Amount - averageFixResistance));
            damage.Amount /= array.Length;
            damage.IgnoreDamageReduction = true;
            FightActor[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                FightActor fightActor = array2[i];
                Damage damage2 = new Damage(damage.Amount)
                {
                    Source = damage.Source,
                    School = damage.School,
                    Buff = damage.Buff,
                    MarkTrigger = this,
                    IgnoreDamageReduction = true,
                    EffectGenerationType = damage.EffectGenerationType
                };
                fightActor.InflictDamage(damage2);
            }
            return damage.Amount;
        }

        private int GetAveragePercentResistance(FightActor[] actors, EffectSchoolEnum type, bool pvp)
        {
            int result;
            switch (type)
            {
                case EffectSchoolEnum.Neutral:
                {
                    int arg_77_0 =
                        (int) actors.Average((FightActor x) => x.Stats[PlayerFields.NeutralResistPercent].Total);
                    int arg_77_1;
                    if (!pvp)
                    {
                        arg_77_1 = (int) 0.0;
                    }
                    else
                    {
                        arg_77_1 =
                            (int) actors.Average((FightActor x) => x.Stats[PlayerFields.PvpNeutralResistPercent].Total);
                    }
                    result = arg_77_0 + arg_77_1;
                    break;
                }
                case EffectSchoolEnum.Earth:
                {
                    int arg_D3_0 =
                        (int) actors.Average((FightActor x) => x.Stats[PlayerFields.EarthResistPercent].Total);
                    int arg_D3_1;
                    if (!pvp)
                    {
                        arg_D3_1 = (int) 0.0;
                    }
                    else
                    {
                        arg_D3_1 =
                            (int) actors.Average((FightActor x) => x.Stats[PlayerFields.PvpEarthResistPercent].Total);
                    }
                    result = arg_D3_0 + arg_D3_1;
                    break;
                }
                case EffectSchoolEnum.Water:
                {
                    int arg_12F_0 =
                        (int) actors.Average((FightActor x) => x.Stats[PlayerFields.WaterResistPercent].Total);
                    int arg_12F_1;
                    if (!pvp)
                    {
                        arg_12F_1 = (int) 0.0;
                    }
                    else
                    {
                        arg_12F_1 =
                            (int) actors.Average((FightActor x) => x.Stats[PlayerFields.PvpWaterResistPercent].Total);
                    }
                    result = arg_12F_0 + arg_12F_1;
                    break;
                }
                case EffectSchoolEnum.Air:
                {
                    int arg_18B_0 = (int) actors.Average((FightActor x) => x.Stats[PlayerFields.AirResistPercent].Total);
                    int arg_18B_1;
                    if (!pvp)
                    {
                        arg_18B_1 = (int) 0.0;
                    }
                    else
                    {
                        arg_18B_1 =
                            (int) actors.Average((FightActor x) => x.Stats[PlayerFields.PvpAirResistPercent].Total);
                    }
                    result = arg_18B_0 + arg_18B_1;
                    break;
                }
                case EffectSchoolEnum.Fire:
                {
                    int arg_1E4_0 =
                        (int) actors.Average((FightActor x) => x.Stats[PlayerFields.FireResistPercent].Total);
                    int arg_1E4_1;
                    if (!pvp)
                    {
                        arg_1E4_1 = (int) 0.0;
                    }
                    else
                    {
                        arg_1E4_1 =
                            (int) actors.Average((FightActor x) => x.Stats[PlayerFields.PvpFireResistPercent].Total);
                    }
                    result = arg_1E4_0 + arg_1E4_1;
                    break;
                }
                default:
                    result = 0;
                    break;
            }
            return result;
        }

        private int GetAverageFixResistance(FightActor[] actors, EffectSchoolEnum type, bool pvp)
        {
            int result;
            switch (type)
            {
                case EffectSchoolEnum.Neutral:
                {
                    double arg_77_0 =
                        actors.Average((FightActor x) => x.Stats[PlayerFields.NeutralElementReduction].Total);
                    double arg_77_1;
                    if (!pvp)
                    {
                        arg_77_1 = 0.0;
                    }
                    else
                    {
                        arg_77_1 =
                            actors.Average((FightActor x) => x.Stats[PlayerFields.PvpNeutralElementReduction].Total);
                    }
                    result =
                        (int)
                            (arg_77_0 + arg_77_1 +
                             actors.Average((FightActor x) => x.Stats[PlayerFields.PhysicalDamageReduction].Total));
                    break;
                }
                case EffectSchoolEnum.Earth:
                {
                    double arg_F7_0 = actors.Average((FightActor x) => x.Stats[PlayerFields.EarthElementReduction].Total);
                    double arg_F7_1;
                    if (!pvp)
                    {
                        arg_F7_1 = 0.0;
                    }
                    else
                    {
                        arg_F7_1 = actors.Average((FightActor x) => x.Stats[PlayerFields.PvpEarthElementReduction].Total);
                    }
                    result =
                        (int)
                            (arg_F7_0 + arg_F7_1 +
                             actors.Average((FightActor x) => x.Stats[PlayerFields.PhysicalDamageReduction].Total));
                    break;
                }
                case EffectSchoolEnum.Water:
                {
                    double arg_177_0 =
                        actors.Average((FightActor x) => x.Stats[PlayerFields.WaterElementReduction].Total);
                    double arg_177_1;
                    if (!pvp)
                    {
                        arg_177_1 = 0.0;
                    }
                    else
                    {
                        arg_177_1 =
                            actors.Average((FightActor x) => x.Stats[PlayerFields.PvpWaterElementReduction].Total);
                    }
                    result =
                        (int)
                            (arg_177_0 + arg_177_1 +
                             actors.Average((FightActor x) => x.Stats[PlayerFields.MagicDamageReduction].Total));
                    break;
                }
                case EffectSchoolEnum.Air:
                {
                    double arg_1F7_0 = actors.Average((FightActor x) => x.Stats[PlayerFields.AirElementReduction].Total);
                    double arg_1F7_1;
                    if (!pvp)
                    {
                        arg_1F7_1 = 0.0;
                    }
                    else
                    {
                        arg_1F7_1 = actors.Average((FightActor x) => x.Stats[PlayerFields.PvpAirElementReduction].Total);
                    }
                    result =
                        (int)
                            (arg_1F7_0 + arg_1F7_1 +
                             actors.Average((FightActor x) => x.Stats[PlayerFields.MagicDamageReduction].Total));
                    break;
                }
                case EffectSchoolEnum.Fire:
                {
                    double arg_274_0 = actors.Average((FightActor x) => x.Stats[PlayerFields.FireElementReduction].Total);
                    double arg_274_1;
                    if (!pvp)
                    {
                        arg_274_1 = 0.0;
                    }
                    else
                    {
                        arg_274_1 = actors.Average((FightActor x) => x.Stats[PlayerFields.PvpFireElementReduction].Total);
                    }
                    result =
                        (int)
                            (arg_274_0 + arg_274_1 +
                             actors.Average((FightActor x) => x.Stats[PlayerFields.MagicDamageReduction].Total));
                    break;
                }
                default:
                    result = 0;
                    break;
            }
            return result;
        }
    }
}
