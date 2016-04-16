using Stump.Server.WorldServer.Database.Spells;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Spells;
using Stump.Server.WorldServer.Game.Fights;
using Stump.Server.WorldServer.Game.Fights.Triggers;
using Stump.Server.WorldServer.Game.Maps;
using Stump.Server.WorldServer.Game.Maps.Cells;

namespace Stump.Server.WorldServer.Game.Spells.Casts
{
	public abstract class SpellCastHandler
	{
		private MapPoint m_castPoint;
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
		public SpellLevelTemplate SpellLevel
		{
			get
			{
				return this.Spell.CurrentSpellLevel;
			}
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
		public virtual bool SilentCast
		{
			get
			{
				return false;
			}
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
		protected SpellCastHandler(FightActor caster, Spell spell, Cell targetedCell, bool critical)
		{
			this.Caster = caster;
			this.Spell = spell;
			this.TargetedCell = targetedCell;
			this.Critical = critical;
		}
		public abstract void Initialize();
		public abstract void Execute();
		public virtual System.Collections.Generic.IEnumerable<SpellEffectHandler> GetEffectHandlers()
		{
			return new SpellEffectHandler[0];
		}
	}
}
