using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Maps.Cells.Shapes;
using Stump.Server.WorldServer.Game.Spells;
using Stump.Server.WorldServer.Game.Spells.Casts;
using System.Drawing;
using System.Linq;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Effects.Spells;

namespace Stump.Server.WorldServer.Game.Fights.Triggers
{
	public class Trap : MarkTrigger
	{
        // FIELDS

        // PROPERTIES
		public Spell TrapSpell
		{
			get;
			private set;
		}
		public GameActionFightInvisibilityStateEnum VisibleState
		{
			get;
			set;
		}
		public override GameActionMarkTypeEnum Type
		{
			get
			{
				return GameActionMarkTypeEnum.TRAP;
			}
		}
		public override TriggerTypeFlag TriggerType
		{
			get
			{
				return TriggerTypeFlag.MOVE;
			}
		}

        // CONSTRUCTORS
		public Trap(short id, FightActor caster, Spell castedSpell, Cell centerCell, EffectDice originEffect, Spell glyphSpell, byte size) 
            : base(id, caster, castedSpell, centerCell, originEffect, new MarkShape[]
		{
			new MarkShape(caster.Fight, centerCell, GameActionMarkCellsTypeEnum.CELLS_CIRCLE, size, Color.Brown)
		})
		{
			this.TrapSpell = glyphSpell;
			this.VisibleState = GameActionFightInvisibilityStateEnum.INVISIBLE;
		}
        public Trap(short id, FightActor caster, Spell spell, Cell centerCell, EffectDice originEffect, Spell trapSpell, GameActionMarkCellsTypeEnum shape, byte size) 
            : base(id, caster, spell, centerCell, originEffect, new MarkShape[]
		{
			new MarkShape(caster.Fight, centerCell, shape, size, Color.Brown)
		})
		{
			this.TrapSpell = trapSpell;
			this.VisibleState = GameActionFightInvisibilityStateEnum.INVISIBLE;
		}

        // METHODS
		public override bool DoesSeeTrigger(FightActor fighter)
		{
			return this.VisibleState != GameActionFightInvisibilityStateEnum.INVISIBLE || fighter.IsFriendlyWith(base.Caster);
		}
		public override bool DecrementDuration()
		{
			return false;
		}
		public override void Trigger(FightActor trigger)
		{
			base.NotifyTriggered(trigger, this.TrapSpell);
			MarkShape[] shapes = base.Shapes;
			for (int i = 0; i < shapes.Length; i++)
			{
				MarkShape markShape = shapes[i];
				SpellCastHandler spellCastHandler = Singleton<SpellManager>.Instance.GetSpellCastHandler(base.Caster, this.TrapSpell, markShape.Cell, false);
				spellCastHandler.MarkTrigger = this;
				spellCastHandler.Initialize();
				foreach (SpellEffectHandler current in spellCastHandler.GetEffectHandlers())
				{
					current.EffectZone = new Zone((markShape.Shape == GameActionMarkCellsTypeEnum.CELLS_CROSS) ? SpellShapeEnum.Q : current.Effect.ZoneShape, markShape.Size);
				}
				spellCastHandler.Execute();
			}
		}
		public override GameActionMark GetHiddenGameActionMark()
		{
			return new GameActionMark(base.Caster.Id, Caster.Team.Id, base.CastedSpell.Id, 1, base.Id, (sbyte)this.Type, 1, new GameActionMarkedCell[0], true);
		}
		public override GameActionMark GetGameActionMark()
		{
			return new GameActionMark(base.Caster.Id, Caster.Team.Id, base.CastedSpell.Id, 1, base.Id, (sbyte)this.Type, 1, 
				from entry in base.Shapes
				select entry.GetGameActionMarkedCell(), true);
		}
	}
}
