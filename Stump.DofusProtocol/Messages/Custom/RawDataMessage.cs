// Generated on 11/30/2014 19:15:48
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class RawDataMessage : Message
    {
        public const ushort Id = 6253;
        public byte[] _content;
        public override ushort MessageId
        {
            get
            {
                return 6253;
            }
        }
        public RawDataMessage()
        {
        }
        public RawDataMessage(byte[] content)
        {
            this._content = content;
        }
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteVarInt(this._content.Length);
            writer.WriteBytes(this._content);
        }
        public override void Deserialize(ICustomDataInput reader)
        {
            int n = reader.ReadVarInt();
            this._content = reader.ReadBytes(n);
        }
    }
}