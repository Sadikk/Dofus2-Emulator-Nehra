using System.Linq;
using NLog;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.Spells;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Fights;
using Stump.Server.WorldServer.Game.Fights.Buffs;
using Stump.Server.WorldServer.Game.Fights.Triggers;
using Stump.Server.WorldServer.Game.Maps;
using Stump.Server.WorldServer.Game.Maps.Cells;
using Stump.Server.WorldServer.Game.Maps.Cells.Shapes;
using Stump.Server.WorldServer.Game.Spells;
using Stump.DofusProtocol.Enums.HomeMade;
using System.Collections.Generic;
using System;
using Stump.Core.Reflection;

namespace Stump.Server.WorldServer.Game.Effects.Spells
{
	public abstract class SpellEffectHandler : EffectHandler
	{
        private static Logger logger = LogManager.GetCurrentClassLogger();

		private FightActor[] m_customAffectedActors;
		private Cell[] m_affectedCells;
		private MapPoint m_castPoint;
		private Zone m_effectZone;
		public EffectDice Dice
		{
			get;
			private set;
		}
		public FightActor Caster
		{
			get;
			private set;
		}
		public Spell Spell
		{
			get;
			private set;
		}
		public Cell TargetedCell
		{
			get;
			private set;
		}
		public MapPoint TargetedPoint
		{
			get;
			private set;
		}
		public bool Critical
		{
			get;
			private set;
		}
		public MarkTrigger MarkTrigger
		{
			get;
			set;
		}
		public Cell CastCell
		{
			get
			{
				return (this.MarkTrigger == null || this.MarkTrigger.Shapes.Length <= 0) ? this.Caster.Cell : this.MarkTrigger.Shapes[0].Cell;
			}
		}
		public MapPoint CastPoint
		{
			get
			{
				MapPoint arg_1E_0;
				if ((arg_1E_0 = this.m_castPoint) == null)
				{
					arg_1E_0 = (this.m_castPoint = new MapPoint(this.CastCell));
				}
				return arg_1E_0;
			}
			set
			{
				this.m_castPoint = value;
			}
		}
		public Zone EffectZone
		{
			get
			{
				Zone arg_41_0;
				if ((arg_41_0 = this.m_effectZone) == null)
				{
					arg_41_0 = (this.m_effectZone = new Zone(this.Effect.ZoneShape, (byte)this.Effect.ZoneSize, this.CastPoint.OrientationTo(this.TargetedPoint, true)));
				}
				return arg_41_0;
			}
			set
			{
				this.m_effectZone = value;
				this.RefreshZone();
			}
		}
		public SpellTargetType Targets
		{
			get;
			set;
		}
		public Cell[] AffectedCells
		{
			get
			{
				Cell[] arg_2A_0;
				if ((arg_2A_0 = this.m_affectedCells) == null)
				{
					arg_2A_0 = (this.m_affectedCells = this.EffectZone.GetCells(this.TargetedCell, this.Map));
				}
				return arg_2A_0;
			}
			private set
			{
				this.m_affectedCells = value;
			}
		}
		public Fight Fight
		{
			get
			{
				return this.Caster.Fight;
			}
		}
		public Map Map
		{
			get
			{
				return this.Fight.Map;
			}
		}

		protected SpellEffectHandler(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect)
		{
			this.Dice = effect;
			this.Caster = caster;
			this.Spell = spell;
			this.TargetedCell = targetedCell;
			this.TargetedPoint = new MapPoint(this.TargetedCell);
			this.Critical = critical;
			this.Targets = effect.Targets;

            SpellEffectHandler.logger.Debug<EffectsEnum, SpellTemplate>("Handle '{0}' effect for the spell '{1}'.", effect.EffectId, spell.Template);
		}

        public bool IsValidTarget(FightActor actor)
        {
            bool result;
            if (this.Targets == SpellTargetType.NONE)
            {
                result = true;
            }
            else
            {
                if (this.Targets == SpellTargetType.ALL)
                {
                    result = true;
                }
                else
                {
                    if (this.Caster == actor && this.Targets.HasFlag(SpellTargetType.SELF))
                    {
                        result = true;
                    }
                    else
                    {
                        if (this.Targets.HasFlag(SpellTargetType.ONLY_SELF) && actor != this.Caster)
                        {
                            result = false;
                        }
                        else
                        {
                            if (this.Caster.IsFriendlyWith(actor) && this.Caster != actor)
                            {
                                if ((this.Targets.HasFlag(SpellTargetType.ALLY_1) || this.Targets.HasFlag(SpellTargetType.ALLY_2) || this.Targets.HasFlag(SpellTargetType.ALLY_3) || this.Targets.HasFlag(SpellTargetType.ALLY_4) || this.Targets.HasFlag(SpellTargetType.ALLY_5)) && !(actor is SummonedFighter))
                                {
                                    result = true;
                                    return result;
                                }
                                if ((this.Targets.HasFlag(SpellTargetType.ALLY_SUMMONS) || this.Targets.HasFlag(SpellTargetType.ALLY_STATIC_SUMMONS)) && actor is SummonedFighter)
                                {
                                    result = true;
                                    return result;
                                }
                            }
                            if (this.Caster.IsEnnemyWith(actor))
                            {
                                if ((this.Targets.HasFlag(SpellTargetType.ENNEMY_1) || this.Targets.HasFlag(SpellTargetType.ENNEMY_2) || this.Targets.HasFlag(SpellTargetType.ENNEMY_3) || this.Targets.HasFlag(SpellTargetType.ENNEMY_4) || this.Targets.HasFlag(SpellTargetType.ENNEMY_5)) && !(actor is SummonedFighter))
                                {
                                    result = true;
                                    return result;
                                }
                                if ((this.Targets.HasFlag(SpellTargetType.ENNEMY_SUMMONS) || this.Targets.HasFlag(SpellTargetType.ENNEMY_STATIC_SUMMONS)) && actor is SummonedFighter)
                                {
                                    result = true;
                                    return result;
                                }
                            }
                            result = false;
                        }
                    }
                }
            }
            if (this.Effect != null && this.Effect.ParsedTargetMask != null)
            {
                foreach (KeyValuePair<char, object> pair in this.Effect.ParsedTargetMask)
                {
                    foreach (var handler in Singleton<EffectManager>.Instance.GetTargetMaskHandlers(pair.Key, Caster).ToArray())
                    {
                        if (handler.Func(handler.Container, Caster, actor, Effect, pair.Value))
                        {
                            return true;
                        }
                    }
                }
            }
            return result;
		}
		public void RefreshZone()
		{
			this.AffectedCells = this.EffectZone.GetCells(this.TargetedCell, this.Map);
		}
		public System.Collections.Generic.IEnumerable<FightActor> GetAffectedActors()
		{
            
			System.Collections.Generic.IEnumerable<FightActor> result;
			if (this.m_customAffectedActors != null)
			{
				result = this.m_customAffectedActors;
			}
			else
			{
				if (this.Effect.Targets.HasFlag(SpellTargetType.ONLY_SELF))
				{
					result = new FightActor[]
					{
						this.Caster
					};
				}
				else
				{
					result = (
						from entry in this.Fight.GetAllFighters(this.AffectedCells)
						where !entry.IsDead() && this.IsValidTarget(entry)
						select entry).ToArray<FightActor>();
				}
			}
			return result;
		}
		public System.Collections.Generic.IEnumerable<FightActor> GetAffectedActors(System.Predicate<FightActor> predicate)
		{
			System.Collections.Generic.IEnumerable<FightActor> result;
			if (this.m_customAffectedActors != null)
			{
				result = this.m_customAffectedActors;
			}
			else
			{
				if (this.Effect.Targets.HasFlag(SpellTargetType.ONLY_SELF) && predicate(this.Caster))
				{
					result = new FightActor[]
					{
						this.Caster
					};
				}
				else
				{
					if (this.Effect.Targets.HasFlag(SpellTargetType.ONLY_SELF))
					{
						result = new FightActor[0];
					}
					else
					{
						result = (
							from entry in this.GetAffectedActors()
							where predicate(entry)
							select entry).ToArray<FightActor>();
					}
				}
			}
			return result;
		}
		public EffectInteger GenerateEffect()
		{
			EffectInteger effectInteger = this.Effect.GenerateEffect(EffectGenerationContext.Spell, EffectGenerationType.Normal) as EffectInteger;
			if (effectInteger != null)
			{
				effectInteger.Value = (short)((double)effectInteger.Value * base.Efficiency);
			}
			return effectInteger;
		}
		public void SetAffectedActors(System.Collections.Generic.IEnumerable<FightActor> actors)
		{
			this.m_customAffectedActors = actors.ToArray<FightActor>();
		}
		public StatBuff AddStatBuff(FightActor target, short value, PlayerFields caracteritic, bool dispelable)
		{
			int id = target.PopNextBuffId();
			StatBuff statBuff = new StatBuff(id, target, this.Caster, this.Effect, this.Spell, value, caracteritic, this.Critical, dispelable);
			target.AddAndApplyBuff(statBuff, true);
			return statBuff;
		}
		public StatBuff AddStatBuff(FightActor target, short value, PlayerFields caracteritic, bool dispelable, short customActionId)
		{
			int id = target.PopNextBuffId();
			StatBuff statBuff = new StatBuff(id, target, this.Caster, this.Effect, this.Spell, value, caracteritic, this.Critical, dispelable, customActionId);
			target.AddAndApplyBuff(statBuff, true);
			return statBuff;
		}
        public StatBuff AddStatBuff(FightActor target, short value, PlayerFields caracteritic, bool dispelable, CustomActionsEnum customAction)
        {
            int id = target.PopNextBuffId();
            StatBuff statBuff = new StatBuff(id, target, this.Caster, this.Effect, this.Spell, value, caracteritic, this.Critical, dispelable, (short)customAction);
            target.AddAndApplyBuff(statBuff, true);
            return statBuff;
        }
        public TriggerBuff AddTriggerBuff(FightActor target, bool dispelable, BuffTriggerType trigger, TriggerBuffApplyHandler applyTrigger)
		{
			int id = target.PopNextBuffId();
			TriggerBuff triggerBuff = new TriggerBuff(id, target, this.Caster, this.Dice, this.Spell, this.Critical, dispelable, trigger, applyTrigger);
			target.AddAndApplyBuff(triggerBuff, true);
			return triggerBuff;
		}
		public TriggerBuff AddTriggerBuff(FightActor target, bool dispelable, BuffTriggerType trigger, TriggerBuffApplyHandler applyTrigger, TriggerBuffRemoveHandler removeTrigger)
		{
			int id = target.PopNextBuffId();
			TriggerBuff triggerBuff = new TriggerBuff(id, target, this.Caster, this.Dice, this.Spell, this.Critical, dispelable, trigger, applyTrigger, removeTrigger);
			target.AddAndApplyBuff(triggerBuff, true);
			return triggerBuff;
		}
		public StateBuff AddStateBuff(FightActor target, bool dispelable, SpellState state)
		{
			int id = target.PopNextBuffId();
			StateBuff stateBuff = new StateBuff(id, target, this.Caster, this.Dice, this.Spell, dispelable, state);
			target.AddAndApplyBuff(stateBuff, true);
			return stateBuff;
		}
        public bool RemoveStateBuff(FightActor target, int stateId)
        {
            var stateBuff = target.GetBuffs().Where(x => x is StateBuff && (x as StateBuff).State.Id == stateId).FirstOrDefault();
            if (stateBuff != null)
            {
                target.RemoveAndDispellBuff(stateBuff);
                return true;
            }
            return false;
        }
        public virtual bool RequireSilentCast()
		{
			return false;
		}
        private Dictionary<char, object> ParseTargetMask(string pattern)
        {
            Dictionary<char, object> result = new Dictionary<char, object>();
            var splitted = pattern.Replace(" ", "").Split(',');
            foreach (string mask in splitted)
            {
                result.Add(mask[0], mask.Remove(0, 1));
            }
            return result;
        }
	}
}
