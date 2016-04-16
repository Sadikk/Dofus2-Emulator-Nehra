









// Generated on 07/24/2015 23:19:53
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class MoodSmileyResultMessage : Message
    {
        public const ushort Id = 6196;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public sbyte resultCode;
        public sbyte smileyId;
        
        public MoodSmileyResultMessage()
        {
        }
        
        public MoodSmileyResultMessage(sbyte resultCode, sbyte smileyId)
        {
            this.resultCode = resultCode;
            this.smileyId = smileyId;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteSByte(resultCode);
            writer.WriteSByte(smileyId);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            resultCode = reader.ReadSByte();
            if (resultCode < 0)
                throw new Exception("Forbidden value on resultCode = " + resultCode + ", it doesn't respect the following condition : resultCode < 0");
            smileyId = reader.ReadSByte();
        }
        
    }
    
}