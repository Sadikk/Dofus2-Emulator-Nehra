namespace Stump.Server.WorldServer.Database.World.Maps
{
	public struct MapElement
	{
		public const int Size = 6;
		public short CellId;
		public uint ElementId;

		public MapElement(uint elementId, short cellId)
		{
			this.ElementId = elementId;
			this.CellId = cellId;
		}
		public byte[] Serialize()
		{
			return new byte[]
			{
				(byte)(this.CellId >> 8),
				(byte)(this.CellId & 255),
				(byte)(this.ElementId >> 24),
				(byte)(this.ElementId >> 16 & 255u),
				(byte)(this.ElementId >> 8 & 255u),
				(byte)(this.ElementId & 255u)
			};
		}
		public void Deserialize(byte[] data, int index)
		{
			this.CellId = (short)((int)data[index] << 8 | (int)data[index + 1]);
			this.ElementId = (uint)((int)data[index + 2] << 24 | (int)data[index + 3] << 16 | (int)data[index + 4] << 8 | (int)data[index + 5]);
		}
	}
}
