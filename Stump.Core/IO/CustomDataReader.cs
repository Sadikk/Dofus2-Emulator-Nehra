using Stump.Core.IO.Types;
using Stump.Core.Pool.New;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stump.Core.IO
{
    public class CustomDataReader : BigEndianReader, ICustomDataInput
    {
        private static int INT_SIZE = 32;

        private static int SHORT_SIZE = 16;

        private static int SHORT_MAX_VALUE = 32767;

        private static int UNSIGNED_SHORT_MAX_VALUE = 65536;

        private static int CHUNCK_BIT_SIZE = 7;

        private static int MAX_ENCODING_LENGTH = (int)Math.Ceiling((double)INT_SIZE / CHUNCK_BIT_SIZE);

        private static int MASK_10000000 = 128;

        private static int MASK_01111111 = 127;

        public CustomDataReader(BufferSegment segment)
            : base(segment)
        {
        }

        public CustomDataReader(byte[] data)
            : base(data)
        {
        }

        public int ReadVarInt()
        {
            int b = 0;
            int value = 0;
            int offset = 0;
            bool hasNext = false;
            while (offset < INT_SIZE)
            {
                b = ReadByte();
                hasNext = (b & MASK_10000000) == MASK_10000000;
                if (offset > 0)
                {
                    value = value + ((b & MASK_01111111) << offset);
                }
                else
                {
                    value = value + (b & MASK_01111111);
                }
                offset = offset + CHUNCK_BIT_SIZE;
                if (!hasNext)
                {
                    return value;
                }
            }
            throw new Exception("Too much data");
        }

        public uint ReadVarUhInt()
        {
            int b = 0;
            uint value = 0;
            int offset = 0;
            bool hasNext = false;
            while (offset < INT_SIZE)
            {
                b = ReadByte();
                hasNext = (b & MASK_10000000) == MASK_10000000;
                if (offset > 0)
                {
                    value = (uint)(value + ((b & MASK_01111111) << offset));
                }
                else
                {
                    value = (uint)(value + (b & MASK_01111111));
                }
                offset = offset + CHUNCK_BIT_SIZE;
                if (!hasNext)
                {
                    return value;
                }
            }
            throw new Exception("Too much data");
        }

        public short ReadVarShort()
        {
            int b = 0;
            short value = 0;
            int offset = 0;
            bool hasNext = false;
            while (offset < SHORT_SIZE)
            {
                b = ReadByte();
                hasNext = (b & MASK_10000000) == MASK_10000000;
                if (offset > 0)
                {
                    value = (short)(value + ((b & MASK_01111111) << offset));
                }
                else
                {
                    value = (short)(value + (b & MASK_01111111));
                }
                offset = offset + CHUNCK_BIT_SIZE;
                if (!hasNext)
                {
                    if (value > SHORT_MAX_VALUE)
                    {
                        value = (short)(value - UNSIGNED_SHORT_MAX_VALUE);
                    }
                    return value;
                }
            }
            throw new Exception("Too much data");
        }

        public ushort ReadVarUhShort()
        {
            int b = 0;
            ushort value = 0;
            int offset = 0;
            bool hasNext = false;
            while (offset < SHORT_SIZE)
            {
                b = ReadByte();
                hasNext = (b & MASK_10000000) == MASK_10000000;
                if (offset > 0)
                {
                    value = (ushort)(value + ((b & MASK_01111111) << offset));
                }
                else
                {
                    value = (ushort)(value + (b & MASK_01111111));
                }
                offset = offset + CHUNCK_BIT_SIZE;
                if (!hasNext)
                {
                    if (value > SHORT_MAX_VALUE)
                    {
                        value = (ushort)(value - UNSIGNED_SHORT_MAX_VALUE);
                    }
                    return value;
                }
            }
            throw new Exception("Too much data");
        }

        public long ReadVarLong()
        {
            return readInt64().toNumber();
        }

        public ulong ReadVarUhLong()
        {
            return readUInt64().toNumber();
        }

        private CustomInt64 readInt64()
        {
            uint b = 0;
            CustomInt64 result = new CustomInt64();
            int i = 0;
            while (true)
            {
                b = ReadByte();
                if (i == 28)
                {
                    break;
                }
                if (b >= 128)
                {
                    result.low = result.low | (b & 127) << i;
                    i = i + 7;
                    continue;
                }
                result.low = result.low | b << i;
                return result;
            }

            if (b >= 128)
            {
                b = b & 127;
                result.low = result.low | b << i;
                result.high = b >> 4;
                i = 3;
                while (true)
                {
                    b = ReadByte();
                    if (i < 32)
                    {
                        if (b >= 128)
                        {
                            result.high = (uint)(result.high | (b & 127) << i);
                        }
                        else
                        {
                            break;
                        }
                    }
                    i = i + 7;
                }

                result.high = (uint)(result.high | (b << i));
                return result;
            }
            result.low = result.low | b << i;
            result.high = b >> 4;
            return result;
        }

        private CustomUInt64 readUInt64()
        {
            uint b = 0;
            var result = new CustomUInt64();
            int i = 0;
            while (true)
            {
                b = ReadByte();
                if (i == 28)
                {
                    break;
                }
                if (b >= 128)
                {
                    result.low = result.low | (b & 127) << i;
                    i = i + 7;
                    continue;
                }
                result.low = result.low | b << i;
                return result;
            }

            if (b >= 128)
            {
                b = b & 127;
                result.low = result.low | b << i;
                result.high = b >> 4;
                i = 3;
                while (true)
                {
                    b = ReadByte();
                    if (i < 32)
                    {
                        if (b >= 128)
                        {
                            result.high = result.high | (b & 127) << i;
                        }
                        else
                        {
                            break;
                        }
                    }
                    i = i + 7;
                }

                result.high = result.high | b << i;
                return result;
            }
            result.low = result.low | b << i;
            result.high = b >> 4;
            return result;
        }
    }
}