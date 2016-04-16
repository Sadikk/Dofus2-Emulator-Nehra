using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Maps.Cells.Shapes;
using System.Drawing;

namespace Stump.Server.WorldServer.Game.Fights.Triggers
{
    public class MarkShape
    {
        // FIELDS
        private readonly Zone m_zone;
        private readonly Cell[] m_cells;

        // PROPERTIES
        public Fight Fight { get; private set; }
        public Cell Cell { get; private set; }
        public GameActionMarkCellsTypeEnum Shape { get; private set; }
        public byte Size { get; private set; }
        public Color Color { get; private set; }

        // CONSTRUCTORS
        public MarkShape(Fight fight, Cell cell, GameActionMarkCellsTypeEnum shape, byte size, Color color)
        {
            this.Fight = fight;
            this.Cell = cell;
            this.Shape = shape;
            this.Size = size;
            this.Color = color;
            this.m_zone = ((this.Shape == GameActionMarkCellsTypeEnum.CELLS_CROSS)
                ? new Zone(SpellShapeEnum.Q, size)
                : new Zone(SpellShapeEnum.C, size));
            this.m_cells = this.m_zone.GetCells(this.Cell, fight.Map);
        }

        // METHODS
        public Cell[] GetCells()
        {
            return this.m_cells;
        }

        public GameActionMarkedCell GetGameActionMarkedCell()
        {
            return new GameActionMarkedCell((ushort) this.Cell.Id, (sbyte) this.Size, this.Color.ToArgb() & 16777215,
                (sbyte) this.Shape);
        }
    }
}
