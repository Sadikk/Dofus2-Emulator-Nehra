using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Maps.Pathfinding;
using Stump.Server.WorldServer.Game.Spells;

namespace Stump.Server.WorldServer.AI.Fights.Spells
{
	public class SpellCast
	{
		public Spell Spell
		{
			get;
			set;
		}
		public Cell TargetCell
		{
			get;
			set;
		}
		public Path MoveBefore
		{
			get;
			set;
		}
		public Path MoveAfter
		{
			get;
			set;
		}
		public SpellCast(Spell spell, Cell targetCell)
		{
			this.Spell = spell;
			this.TargetCell = targetCell;
		}
	}
}
