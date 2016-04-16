using System;
using System.IO;
namespace Stump.Server.BaseServer.IPC
{
	public class IPCMessagePart
	{
		private byte[] m_data;
		private bool m_dataMissing;
		public bool IsValid
		{
			get
			{
				return this.LengthBytesCount.HasValue && this.Length.HasValue && this.Data != null && this.Length == this.Data.Length;
			}
		}
		public byte? LengthBytesCount
		{
			get;
			private set;
		}
		public int? Length
		{
			get;
			private set;
		}
		public byte[] Data
		{
			get
			{
				return this.m_data;
			}
			private set
			{
				this.m_data = value;
			}
		}
		public bool Build(BinaryReader reader, long count)
		{
			bool result;
			if (count <= 0L)
			{
				result = false;
			}
			else
			{
				if (this.IsValid)
				{
					result = true;
				}
				else
				{
					if (!this.LengthBytesCount.HasValue && count < 1L)
					{
						result = false;
					}
					else
					{
						if (count >= 1L && !this.LengthBytesCount.HasValue)
						{
							this.LengthBytesCount = new byte?(reader.ReadByte());
						}
						bool arg_B8_0;
						if (this.LengthBytesCount.HasValue)
						{
							byte? lengthBytesCount = this.LengthBytesCount;
							if (count >= (long)((ulong)lengthBytesCount.GetValueOrDefault()) && lengthBytesCount.HasValue)
							{
								arg_B8_0 = this.Length.HasValue;
								goto IL_B8;
							}
						}
						arg_B8_0 = true;
						IL_B8:
						if (!arg_B8_0)
						{
							this.Length = new int?(0);
							for (int i = (int)(this.LengthBytesCount.Value - 1); i >= 0; i--)
							{
								this.Length |= (int)reader.ReadByte() << i * 8;
							}
						}
						if (this.Length.HasValue && !this.m_dataMissing)
						{
							if (this.Length == 0)
							{
								this.Data = new byte[0];
								result = true;
								return result;
							}
							int? length = this.Length;
							if (count >= (long)length.GetValueOrDefault() && length.HasValue)
							{
								this.Data = reader.ReadBytes(this.Length.Value);
								result = true;
								return result;
							}
							length = this.Length;
							if ((long)length.GetValueOrDefault() > count && length.HasValue)
							{
								this.Data = reader.ReadBytes((int)count);
								this.m_dataMissing = true;
								result = false;
								return result;
							}
						}
						else
						{
							if (this.Length.HasValue && this.m_dataMissing)
							{
								long num = (long)this.Data.Length + count;
								int? length = this.Length;
								if (num < (long)length.GetValueOrDefault() && length.HasValue)
								{
									int destinationIndex = this.m_data.Length;
									Array.Resize<byte>(ref this.m_data, (int)((long)this.Data.Length + count));
									byte[] array = reader.ReadBytes((int)count);
									Array.Copy(array, 0, this.Data, destinationIndex, array.Length);
									this.m_dataMissing = true;
									result = false;
									return result;
								}
								num = (long)this.Data.Length + count;
								length = this.Length;
								if (num >= (long)length.GetValueOrDefault() && length.HasValue)
								{
									int num2 = this.Length.Value - this.Data.Length;
									Array.Resize<byte>(ref this.m_data, this.Data.Length + num2);
									byte[] array = reader.ReadBytes(num2);
									Array.Copy(array, 0, this.Data, this.Data.Length - num2, num2);
								}
							}
						}
						result = this.IsValid;
					}
				}
			}
			return result;
		}
	}
}
