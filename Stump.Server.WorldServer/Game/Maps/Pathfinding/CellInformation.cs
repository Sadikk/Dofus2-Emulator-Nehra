using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Fights.Triggers;

namespace Stump.Server.WorldServer.Game.Maps.Pathfinding
{
	public class CellInformation
	{
		public Cell Cell
		{
			get;
			set;
		}
		public bool Walkable
		{
			get;
			set;
		}
		public bool Fighting
		{
			get;
			set;
		}
		public bool UseAI
		{
			get;
			set;
		}
		public int Efficience
		{
			get;
			set;
		}
		public Trap Trap
		{
			get;
			set;
		}
		public Glyph Glyph
		{
			get;
			set;
		}
		public CellInformation(Cell cell, bool walkable)
		{
			this.Cell = cell;
			this.Walkable = walkable;
		}
		public CellInformation(Cell cell, bool walkable, bool fighting)
		{
			this.Cell = cell;
			this.Walkable = walkable;
			this.Fighting = fighting;
		}
		public CellInformation(Cell cell, bool walkable, bool fighting, bool useAI, int efficience, Trap trap, Glyph glyph)
		{
			this.Cell = cell;
			this.Walkable = walkable;
			this.Fighting = fighting;
			this.UseAI = useAI;
			this.Efficience = efficience;
			this.Trap = trap;
			this.Glyph = glyph;
		}
	}
}
