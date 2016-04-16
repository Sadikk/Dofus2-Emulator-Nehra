using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;

namespace Stump.Server.WorldServer.Game.Maps.Cells.Shapes
{
	public class Zone : IShape
	{
		private IShape m_shape;
		private SpellShapeEnum m_shapeType;
		private byte m_radius;
		private DirectionsEnum m_direction;
		public SpellShapeEnum ShapeType
		{
			get
			{
				return this.m_shapeType;
			}
			set
			{
				this.m_shapeType = value;
				this.InitializeShape();
			}
		}
		public IShape Shape
		{
			get
			{
				return this.m_shape;
			}
		}
		public uint Surface
		{
			get
			{
				return this.m_shape.Surface;
			}
		}
		public byte MinRadius
		{
			get
			{
				return this.m_shape.MinRadius;
			}
			set
			{
				this.m_shape.MinRadius = value;
			}
		}
		public DirectionsEnum Direction
		{
			get
			{
				return this.m_direction;
			}
			set
			{
				this.m_direction = value;
				if (this.m_shape != null)
				{
					this.m_shape.Direction = value;
				}
			}
		}
		public byte Radius
		{
			get
			{
				return this.m_radius;
			}
			set
			{
				this.m_radius = value;
				if (this.m_shape != null)
				{
					this.m_shape.Radius = value;
				}
			}
		}
		public Zone(SpellShapeEnum shape, byte radius)
		{
			this.Radius = radius;
			this.ShapeType = shape;
		}
		public Zone(SpellShapeEnum shape, byte radius, DirectionsEnum direction)
		{
			this.Radius = radius;
			this.Direction = direction;
			this.ShapeType = shape;
		}
		public Cell[] GetCells(Cell centerCell, Map map)
		{
			return this.m_shape.GetCells(centerCell, map);
		}
		private void InitializeShape()
		{
			SpellShapeEnum shapeType = this.ShapeType;
			if (shapeType <= SpellShapeEnum.plus)
			{
				if (shapeType == SpellShapeEnum.sharp)
				{
					this.m_shape = new Cross(1, this.Radius)
					{
						Diagonal = true
					};
					goto IL_24F;
				}
				switch (shapeType)
				{
				case SpellShapeEnum.star:
					this.m_shape = new Cross(0, this.Radius)
					{
						AllDirections = true
					};
					goto IL_24F;
				case SpellShapeEnum.plus:
					this.m_shape = new Cross(0, this.Radius)
					{
						Diagonal = true
					};
					goto IL_24F;
				}
			}
			else
			{
				if (shapeType == SpellShapeEnum.slash)
				{
					this.m_shape = new Line(this.Radius);
					goto IL_24F;
				}
				switch (shapeType)
				{
				case SpellShapeEnum.A:
					this.m_shape = new Lozenge(0, 63);
					goto IL_24F;
				case SpellShapeEnum.C:
					this.m_shape = new Lozenge(0, this.Radius);
					goto IL_24F;
				case SpellShapeEnum.D:
					this.m_shape = new Cross(0, this.Radius);
					goto IL_24F;
				case SpellShapeEnum.I:
					this.m_shape = new Lozenge(this.Radius, 63);
					goto IL_24F;
				case SpellShapeEnum.L:
					this.m_shape = new Line(this.Radius);
					goto IL_24F;
				case SpellShapeEnum.O:
                    this.m_shape = new Circle(this.Radius);
					goto IL_24F;
				case SpellShapeEnum.P:
					this.m_shape = new Single();
					goto IL_24F;
				case SpellShapeEnum.Q:
					this.m_shape = new Cross(1, this.Radius);
					goto IL_24F;
				case SpellShapeEnum.T:
					this.m_shape = new Cross(0, this.Radius)
					{
						OnlyPerpendicular = true
					};
					goto IL_24F;
				case SpellShapeEnum.U:
					this.m_shape = new HalfLozenge(0, this.Radius);
					goto IL_24F;
				case SpellShapeEnum.V:
					this.m_shape = new Cone(0, this.Radius);
					goto IL_24F;
				case SpellShapeEnum.W:
					this.m_shape = new Square(0, this.Radius)
					{
						DiagonalFree = true
					};
					goto IL_24F;
				case SpellShapeEnum.X:
					this.m_shape = new Cross(0, this.Radius);
					goto IL_24F;
				}
			}
			this.m_shape = new Cross(0, 0);
			IL_24F:
			this.m_shape.Direction = this.Direction;
		}
	}
}
