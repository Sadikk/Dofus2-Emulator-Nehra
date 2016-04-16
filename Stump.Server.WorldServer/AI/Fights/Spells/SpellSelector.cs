using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.AI.Fights.Brain;
using Stump.Server.WorldServer.Database.Spells;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Maps.Cells;
using Stump.Server.WorldServer.Game.Maps.Pathfinding;
using Stump.Server.WorldServer.Game.Spells;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Stump.Server.WorldServer.AI.Fights.Spells
{
	public class SpellSelector
	{
		private readonly EnvironmentAnalyser m_environment;
		public AIFighter Fighter
		{
			get;
			private set;
		}
		public System.Collections.Generic.List<SpellCastInformations> Possibilities
		{
			get;
			private set;
		}
		public System.Collections.Generic.Dictionary<SpellCategory, int> Priorities
		{
			get;
			set;
		}
		public SpellSelector(AIFighter fighter, EnvironmentAnalyser environment)
		{
			this.m_environment = environment;
			this.Fighter = fighter;
			this.Possibilities = new System.Collections.Generic.List<SpellCastInformations>();
			this.Priorities = new System.Collections.Generic.Dictionary<SpellCategory, int>
			{

				{
					SpellCategory.Summoning,
					5
				},

				{
					SpellCategory.Buff,
					4
				},

				{
					SpellCategory.Damages,
					3
				},

				{
					SpellCategory.Healing,
					2
				},

				{
					SpellCategory.Curse,
					1
				}
			};
		}
		public bool CanReach(FightActor fighter, Spell spell, out int mpToUse)
		{
			bool result;
			if (!spell.CurrentSpellLevel.CastTestLos || fighter.Fight.CanBeSeen(fighter.Cell, this.Fighter.Cell, false))
			{
				long num = (long)this.Fighter.GetSpellRange(spell.CurrentSpellLevel) - (long)((ulong)fighter.Position.Point.DistanceToCell(this.Fighter.Position.Point));
				if (num > 0L)
				{
					mpToUse = 0;
					result = true;
				}
				else
				{
					if (num <= (long)this.Fighter.MP)
					{
						mpToUse = (int)num;
						result = true;
					}
					else
					{
						mpToUse = (int)num;
						result = false;
					}
				}
			}
			else
			{
				mpToUse = 0;
				result = false;
			}
			return result;
		}
		public void AnalysePossibilities()
		{
			this.Possibilities = new System.Collections.Generic.List<SpellCastInformations>();
			foreach (Spell current in this.Fighter.Spells.Values)
			{
				SpellCategory spellCategories = SpellIdentifier.GetSpellCategories(current);
				SpellLevelTemplate currentSpellLevel = current.CurrentSpellLevel;
				SpellCastInformations spellCastInformations = new SpellCastInformations(current);
				if ((long)this.Fighter.AP >= (long)((ulong)currentSpellLevel.ApCost) && !currentSpellLevel.StatesForbidden.Any(new Func<int, bool>(this.Fighter.HasState)))
				{
					if (!currentSpellLevel.StatesRequired.Any((int state) => !this.Fighter.HasState(state)) && this.Fighter.SpellHistory.CanCastSpell(current.CurrentSpellLevel))
					{
						if ((spellCategories & SpellCategory.Summoning) != SpellCategory.None && this.Fighter.CanSummon())
						{
							Cell freeAdjacentCell = this.m_environment.GetFreeAdjacentCell();
							if (freeAdjacentCell == null)
							{
								continue;
							}
							spellCastInformations.IsSummoningSpell = true;
							spellCastInformations.SummonCell = freeAdjacentCell;
						}
						else
						{
							foreach (FightActor current2 in 
								from fighter in this.Fighter.Fight.Fighters
								where fighter.IsAlive() && fighter.IsVisibleFor(this.Fighter)
								select fighter)
							{
								int mPToUse;
								if (this.CanReach(current2, current, out mPToUse) && this.Fighter.SpellHistory.CanCastSpell(current.CurrentSpellLevel, current2.Cell))
								{
									spellCastInformations.MPToUse = mPToUse;
									SpellTarget spellTarget = this.ComputeSpellImpact(current, current2);
									if (spellTarget != null)
									{
										spellTarget.Target = current2;
										if (spellTarget.Damage >= 0.0)
										{
											spellCastInformations.Impacts.Add(spellTarget);
										}
									}
								}
							}
						}
						this.Possibilities.Add(spellCastInformations);
					}
				}
			}
			if (Brain.Brain.DebugMode)
			{
				Debug.WriteLine(this.Fighter.Name);
				using (System.Collections.Generic.Dictionary<int, Spell>.ValueCollection.Enumerator enumerator = this.Fighter.Spells.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Spell spell = enumerator.Current;
						Debug.WriteLine("Spell {0} ({1}) :: {2}", new object[]
						{
							spell.Template.Name,
							spell.Id,
							SpellIdentifier.GetSpellCategories(spell)
						});
						System.Collections.Generic.IEnumerable<SpellCastInformations> arg_2C4_0 = this.Possibilities;
						Func<SpellCastInformations, bool> predicate = (SpellCastInformations x) => x.Spell == spell;
						SpellCastInformations spellCastInformations2 = arg_2C4_0.FirstOrDefault(predicate);
						if (spellCastInformations2 != null)
						{
							if (spellCastInformations2.IsSummoningSpell)
							{
								Debug.WriteLine("\tSummon Spell");
							}
							else
							{
								ObjectDumper objectDumper = new ObjectDumper(8);
								objectDumper.MemberPredicate = ((System.Reflection.MemberInfo member) => !member.Name.Contains("Target"));
								ObjectDumper objectDumper2 = objectDumper;
								Debug.WriteLine("\t{0} Targets", new object[]
								{
									spellCastInformations2.Impacts.Count
								});
								foreach (SpellTarget spellTarget in spellCastInformations2.Impacts)
								{
									Debug.Write(objectDumper2.DumpElement(spellTarget));
									if (spellTarget.Target != null)
									{
                                        Debug.WriteLine("\t\tTarget = " + spellTarget.Target + spellTarget.Target.Id.ToString());
									}
								}
							}
						}
					}
				}
				Debug.WriteLine("");
			}
		}

        public IEnumerable<SpellCast> EnumerateSpellsCast()
        {
            Func<MapPoint, uint> keySelector = null;
            foreach (KeyValuePair<SpellCategory, int> iteratorVariable0 in from x in this.Priorities
                                                                           orderby x.Value descending
                                                                           select x)
            {
                SpellImpactComparer comparer = new SpellImpactComparer(this, iteratorVariable0.Key);
                foreach (SpellCastInformations iteratorVariable2 in this.Possibilities.OrderBy<SpellCastInformations, SpellCastInformations>(x => x, new SpellCastComparer(this, iteratorVariable0.Key)))
                {
                    SpellCategory spellCategories = SpellIdentifier.GetSpellCategories(iteratorVariable2.Spell);
                    if ((spellCategories & ((SpellCategory)iteratorVariable0.Key)) != SpellCategory.None)
                    {
                        if (this.Fighter.AP == 0)
                        {
                            break;
                        }
                        if (iteratorVariable2.MPToUse <= this.Fighter.MP)
                        {
                            if (iteratorVariable2.IsSummoningSpell)
                            {
                                yield return new SpellCast(iteratorVariable2.Spell, iteratorVariable2.SummonCell);
                            }
                            else
                            {
                                foreach (SpellTarget iteratorVariable4 in iteratorVariable2.Impacts.OrderByDescending<SpellTarget, SpellTarget>(x => x, comparer))
                                {
                                    int iteratorVariable5;
                                    if (this.CanReach(iteratorVariable4.Target, iteratorVariable2.Spell, out iteratorVariable5))
                                    {
                                        SpellCast iteratorVariable6 = new SpellCast(iteratorVariable2.Spell, iteratorVariable4.Target.Cell);
                                        if (iteratorVariable5 == 0)
                                        {
                                            yield return iteratorVariable6;
                                        }
                                        else if (iteratorVariable5 <= this.Fighter.MP)
                                        {
                                            if (keySelector == null)
                                            {
                                                keySelector = entry => entry.DistanceToCell(this.Fighter.Position.Point);
                                            }
                                            MapPoint point = iteratorVariable4.Target.Position.Point.GetAdjacentCells(new Func<short, bool>(this.m_environment.CellInformationProvider.IsCellWalkable)).OrderBy<MapPoint, uint>(keySelector).FirstOrDefault<MapPoint>();
                                            if (point == null)
                                            {
                                                point = iteratorVariable4.Target.Position.Point;
                                            }
                                            Path iteratorVariable9 = new Pathfinder(this.m_environment.CellInformationProvider).FindPath(this.Fighter.Position.Cell.Id, point.CellId, false, this.Fighter.MP);
                                            if (!iteratorVariable9.IsEmpty() && (iteratorVariable9.MPCost <= this.Fighter.MP))
                                            {
                                                iteratorVariable6.MoveBefore = iteratorVariable9;
                                                yield return iteratorVariable6;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

		public SpellTarget ComputeSpellImpact(Spell spell, FightActor target)
		{
			SpellTarget result = null;
			foreach (EffectDice current in spell.CurrentSpellLevel.Effects)
			{
				this.CumulEffects(current, ref result, target, spell);
			}
			return result;
		}
		private void CumulEffects(EffectDice effect, ref SpellTarget spellImpact, FightActor target, Spell spell)
		{
			bool flag = this.Fighter.Team.Id == target.Team.Id;
			SpellTarget spellTarget = new SpellTarget();
			SpellTargetType arg_2A_0 = effect.Targets;
			SpellCategory effectCategories = SpellIdentifier.GetEffectCategories(effect.EffectId);
			if (effectCategories != SpellCategory.None)
			{
				double num = 1.0;
				if (effect.Random > 0)
				{
					num = (double)effect.Random / 100.0;
				}
				if (target is SummonedFighter)
				{
					num /= 2.0;
				}
				uint damage = (uint)System.Math.Min(effect.DiceNum, effect.DiceFace);
				uint damage2 = (uint)System.Math.Max(effect.DiceNum, effect.DiceFace);
				if ((effectCategories & SpellCategory.DamagesNeutral) > SpellCategory.None)
				{
					SpellSelector.AdjustDamage(spellTarget, damage, damage2, SpellCategory.DamagesNeutral, num, this.Fighter.Stats.GetTotal(PlayerFields.NeutralDamageBonus) + this.Fighter.Stats.GetTotal(PlayerFields.DamageBonus) + this.Fighter.Stats.GetTotal(PlayerFields.PhysicalDamage), this.Fighter.Stats.GetTotal(PlayerFields.DamageBonusPercent) + this.Fighter.Stats.GetTotal(PlayerFields.Strength), target.Stats.GetTotal(PlayerFields.NeutralElementReduction), target.Stats.GetTotal(PlayerFields.NeutralResistPercent), flag);
				}
				if ((effectCategories & SpellCategory.DamagesFire) > SpellCategory.None)
				{
					SpellSelector.AdjustDamage(spellTarget, damage, damage2, SpellCategory.DamagesNeutral, num, this.Fighter.Stats.GetTotal(PlayerFields.FireDamageBonus) + this.Fighter.Stats.GetTotal(PlayerFields.DamageBonus) + this.Fighter.Stats.GetTotal(PlayerFields.MagicDamage), this.Fighter.Stats.GetTotal(PlayerFields.DamageBonusPercent) + this.Fighter.Stats.GetTotal(PlayerFields.Intelligence), target.Stats.GetTotal(PlayerFields.FireElementReduction), target.Stats.GetTotal(PlayerFields.FireResistPercent), flag);
				}
				if ((effectCategories & SpellCategory.DamagesAir) > SpellCategory.None)
				{
					SpellSelector.AdjustDamage(spellTarget, damage, damage2, SpellCategory.DamagesNeutral, num, this.Fighter.Stats.GetTotal(PlayerFields.AirDamageBonus) + this.Fighter.Stats.GetTotal(PlayerFields.DamageBonus) + this.Fighter.Stats.GetTotal(PlayerFields.MagicDamage), this.Fighter.Stats.GetTotal(PlayerFields.DamageBonusPercent) + this.Fighter.Stats.GetTotal(PlayerFields.Agility), target.Stats.GetTotal(PlayerFields.AirElementReduction), target.Stats.GetTotal(PlayerFields.AirResistPercent), flag);
				}
				if ((effectCategories & SpellCategory.DamagesWater) > SpellCategory.None)
				{
					SpellSelector.AdjustDamage(spellTarget, damage, damage2, SpellCategory.DamagesNeutral, num, this.Fighter.Stats.GetTotal(PlayerFields.WaterDamageBonus) + this.Fighter.Stats.GetTotal(PlayerFields.DamageBonus) + this.Fighter.Stats.GetTotal(PlayerFields.MagicDamage), this.Fighter.Stats.GetTotal(PlayerFields.DamageBonusPercent) + this.Fighter.Stats.GetTotal(PlayerFields.Chance), target.Stats.GetTotal(PlayerFields.WaterElementReduction), target.Stats.GetTotal(PlayerFields.WaterResistPercent), flag);
				}
				if ((effectCategories & SpellCategory.DamagesEarth) > SpellCategory.None)
				{
					SpellSelector.AdjustDamage(spellTarget, damage, damage2, SpellCategory.DamagesNeutral, num, this.Fighter.Stats.GetTotal(PlayerFields.EarthDamageBonus) + this.Fighter.Stats.GetTotal(PlayerFields.DamageBonus) + this.Fighter.Stats.GetTotal(PlayerFields.PhysicalDamage), this.Fighter.Stats.GetTotal(PlayerFields.DamageBonusPercent) + this.Fighter.Stats.GetTotal(PlayerFields.Strength), target.Stats.GetTotal(PlayerFields.EarthElementReduction), target.Stats.GetTotal(PlayerFields.EarthResistPercent), flag);
				}
				if ((effectCategories & SpellCategory.Healing) > SpellCategory.None)
				{
					bool flag2;
					if (flag2 = ((effectCategories & SpellCategory.Damages) > SpellCategory.None))
					{
						target = this.Fighter;
					}
					uint num2 = (uint)System.Math.Max(0, target.MaxLifePoints - target.LifePoints);
					if (flag2)
					{
						spellTarget.MinHeal = System.Math.Min(num2, System.Math.Abs(spellTarget.MinDamage));
						spellTarget.MaxHeal = System.Math.Min(num2, System.Math.Abs(spellTarget.MaxDamage));
					}
					else
					{
						if (num2 > 0u)
						{
							SpellSelector.AdjustDamage(spellTarget, (uint)System.Math.Min((long)effect.DiceNum, (long)((ulong)num2)), (uint)System.Math.Min((long)effect.DiceFace, (long)((ulong)num2)), SpellCategory.Healing, num, this.Fighter.Stats.GetTotal(PlayerFields.HealBonus), this.Fighter.Stats.GetTotal(PlayerFields.Intelligence), 0, 0, !flag);
							if (spellTarget.Heal > num2)
							{
								if (flag)
								{
									spellTarget.MinHeal = (spellTarget.MaxHeal = num2);
								}
								else
								{
									spellTarget.MinHeal = (spellTarget.MaxHeal = (double)(-(double)((ulong)num2)));
								}
							}
						}
					}
				}
				if ((effectCategories & SpellCategory.Buff) > SpellCategory.None)
				{
					if (flag)
					{
						spellTarget.Boost += (double)spell.CurrentLevel * num;
					}
					else
					{
						spellTarget.Boost -= (double)spell.CurrentLevel * num;
					}
				}
				if ((effectCategories & SpellCategory.Curse) > SpellCategory.None)
				{
					double num3 = (double)spell.CurrentLevel * num;
					if (effect.EffectId == EffectsEnum.Effect_SkipTurn)
					{
						num3 = (double)(target.Level * 2) * num;
					}
					if (flag)
					{
						spellTarget.Curse -= 2.0 * num3;
					}
					else
					{
						spellTarget.Curse += num3;
					}
				}
				if (flag)
				{
					spellTarget.Add(spellTarget);
				}
				if (!flag && (effectCategories & SpellCategory.Damages) > SpellCategory.None && spellTarget.MinDamage > (double)target.LifePoints)
				{
					double num3 = System.Math.Max((double)target.MaxLifePoints / 2.0, (double)target.LifePoints) / spellTarget.MinDamage;
					spellTarget.Multiply(num3);
				}
				if (spellImpact != null)
				{
					spellImpact.Add(spellTarget);
				}
				else
				{
					spellImpact = spellTarget;
				}
			}
		}
		private static void AdjustDamage(SpellTarget damages, uint damage1, uint damage2, SpellCategory category, double chanceToHappen, int addDamage, int addDamagePercent, int reduceDamage, int reduceDamagePercent, bool negativ)
		{
			double num = damage1;
			double num2 = (damage1 >= damage2) ? damage1 : damage2;
			if (reduceDamagePercent < 100)
			{
				num = (num * (1.0 + (double)addDamagePercent / 100.0) + (double)addDamage - (double)reduceDamage) * (1.0 - (double)reduceDamagePercent / 100.0) * chanceToHappen;
				num2 = (num2 * (1.0 + (double)addDamagePercent / 100.0) + (double)addDamage - (double)reduceDamage) * (1.0 - (double)reduceDamagePercent / 100.0) * chanceToHappen;
				if (num < 0.0)
				{
					num = 0.0;
				}
				if (num2 < 0.0)
				{
					num2 = 0.0;
				}
				if (negativ)
				{
					num *= -1.5;
					num2 *= -1.5;
				}
				if (category <= SpellCategory.DamagesEarth)
				{
					if (category != SpellCategory.Healing)
					{
						if (category != SpellCategory.DamagesWater)
						{
							if (category == SpellCategory.DamagesEarth)
							{
								damages.MinEarth += num;
								damages.MaxEarth += num2;
							}
						}
						else
						{
							damages.MinWater += num;
							damages.MaxWater += num2;
						}
					}
					else
					{
						damages.MinHeal += num;
						damages.MaxHeal += num2;
					}
				}
				else
				{
					if (category != SpellCategory.DamagesAir)
					{
						if (category != SpellCategory.DamagesFire)
						{
							if (category == SpellCategory.DamagesNeutral)
							{
								damages.MinNeutral += num;
								damages.MaxNeutral += num2;
							}
						}
						else
						{
							damages.MinFire += num;
							damages.MaxAir += num2;
						}
					}
					else
					{
						damages.MinAir += num;
						damages.MaxAir += num2;
					}
				}
			}
		}
	}
}
