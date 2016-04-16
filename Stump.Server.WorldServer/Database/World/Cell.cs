namespace Stump.Server.WorldServer.Database.World
{
	[System.Serializable]
	public class Cell
	{
		public const int StructSize = 11;
		public short Floor;
		public short Id;
		public byte LosMov;
		public byte MapChangeData;
		public uint MoveZone;
		public byte Speed;
		public bool Walkable
		{
			get
			{
				return (this.LosMov & 1) == 1;
			}
		}
		public bool LineOfSight
		{
			get
			{
				return (this.LosMov & 2) == 2;
			}
		}
		public bool NonWalkableDuringFight
		{
			get
			{
				return (this.LosMov & 4) == 4;
			}
		}
		public bool Red
		{
			get
			{
				return (this.LosMov & 8) == 8;
			}
		}
		public bool Blue
		{
			get
			{
				return (this.LosMov & 16) == 16;
			}
		}
		public bool FarmCell
		{
			get
			{
				return (this.LosMov & 32) == 32;
			}
		}
		public bool Visible
		{
			get
			{
				return (this.LosMov & 64) == 64;
			}
		}
		public bool NonWalkableDuringRP
		{
			get
			{
				return (this.LosMov & 128) == 128;
			}
		}
		public byte[] Serialize()
		{
			return new byte[]
			{
				(byte)(this.Id >> 8),
				(byte)(this.Id & 255),
				(byte)(this.Floor >> 8),
				(byte)(this.Floor & 255),
				this.LosMov,
				this.MapChangeData,
				this.Speed,
				(byte)(this.MoveZone >> 24),
				(byte)(this.MoveZone >> 16),
				(byte)(this.MoveZone >> 8),
				(byte)(this.MoveZone & 255u)
			};
		}
		public void Deserialize(byte[] data, int index = 0)
		{
			this.Id = (short)((int)data[index] << 8 | (int)data[index + 1]);
			this.Floor = (short)((int)data[index + 2] << 8 | (int)data[index + 3]);
			this.LosMov = data[index + 4];
			this.MapChangeData = data[index + 5];
			this.Speed = data[index + 6];
			this.MoveZone = (uint)((int)data[index + 7] << 24 | (int)data[index + 8] << 16 | (int)data[index + 9] << 8 | (int)data[index + 10]);
		}
	}
}
