using Stump.Core.IO.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stump.Core.IO
{
    public class CustomDataWriter : BigEndianWriter, ICustomDataOutput
    {
        private static int INT_SIZE = 32;

        private static int SHORT_MIN_VALUE = -32768;

        private static int SHORT_MAX_VALUE = 32767;

        private static int UNSIGNED_SHORT_MAX_VALUE = 65536;

        private static int CHUNCK_BIT_SIZE = 7;

        private static int MAX_ENCODING_LENGTH = (int)Math.Ceiling((double)INT_SIZE / CHUNCK_BIT_SIZE);

        private static int MASK_10000000 = 128;

        private static int MASK_01111111 = 127;

        public CustomDataWriter()
        {
        }

        public CustomDataWriter(Stream stream)
            : base(stream)
        {
        }

        public void WriteVarInt(int value)
        {
            if (value >= 0 && value <= MASK_01111111)
            {
                WriteByte((byte)value);
                return;
            }
            int b = 0;
            int c = value;
            while (c != 0 && c != -1)
            {
                b = c & MASK_01111111;
                c = c >> CHUNCK_BIT_SIZE;
                if (c > 0)
                {
                    b = b | MASK_10000000;
                }
                WriteByte((byte)b);
            }
        }

        public void WriteVarUhInt(uint value)
        {
            if (value <= MASK_01111111)
            {
                WriteByte((byte)value);
                return;
            }
            uint b = 0;
            uint c = value;
            while (c != 0)
            {
                b = (uint)(c & MASK_01111111);
                c = c >> CHUNCK_BIT_SIZE;
                if (c > 0)
                {
                    b = b | (uint)MASK_10000000;
                }
                WriteByte((byte)b);
            }
        }

        public void WriteVarShort(short value)
        {
            if (value > SHORT_MAX_VALUE || value < SHORT_MIN_VALUE)
            {
                throw new Exception("Forbidden value");
            }
            else
            {
                var b = 0;
                if ((value >= 0) && (value <= MASK_01111111))
                {
                    WriteByte((byte)value);
                    return;
                }
                var c = value & 65535;
                while (c != 0 && c != -1)
                {
                    b = (c & MASK_01111111);
                    c = c >> CHUNCK_BIT_SIZE;
                    if (c > 0)
                    {
                        b = b | MASK_10000000;
                    }
                    WriteByte((byte)b);
                }
            }
        }

        public void WriteVarUhShort(ushort value)
        {
            if (value > UNSIGNED_SHORT_MAX_VALUE || value < SHORT_MIN_VALUE)
            {
                throw new Exception("Forbidden value");
            }
            else
            {
                var b = 0;
                if ((value >= 0) && (value <= MASK_01111111))
                {
                    WriteByte((byte)value);
                    return;
                }
                var c = value & 65535;
                while (c != 0)
                {
                    b = (c & MASK_01111111);
                    c = c >> CHUNCK_BIT_SIZE;
                    if (c > 0)
                    {
                        b = b | MASK_10000000;
                    }
                    WriteByte((byte)b);
                }
            }
        }

        public void WriteVarLong(long value)
        {
            uint i = 0;
            var val = CustomInt64.fromNumber(value);
            if (val.high == 0)
            {
                writeint32( val.low);
            }
            else
            {
                i = 0;
                while (i < 4)
                {
                    WriteByte((byte)(val.low & 127 | 128));
                    val.low = val.low >> 7;
                    i++;
                }
                if ((val.high & 268435455 << 3) == 0)
                {
                    WriteByte((byte)(val.high << 4 | val.low));
                }
                else
                {
                    WriteByte((byte)(((val.high << 4) | val.low) & 127 | 128));
                    writeint32(val.high >> 3);
                }
            }
        }

        public void WriteVarUhLong(ulong value)
        {
            WriteVarLong((long)value);
        }

        private void writeint32(uint value)
        {
            while (value >= 128)
            {
                WriteByte((byte)(value & 127 | 128));
                value = value >> 7;
            }
            WriteByte((byte)value);
        }
    }
}