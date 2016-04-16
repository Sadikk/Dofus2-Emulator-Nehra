









// Generated on 07/24/2015 23:19:53
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class MoodSmileyRequestMessage : Message
    {
        public const ushort Id = 6192;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public sbyte smileyId;
        
        public MoodSmileyRequestMessage()
        {
        }
        
        public MoodSmileyRequestMessage(sbyte smileyId)
        {
            this.smileyId = smileyId;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteSByte(smileyId);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            smileyId = reader.ReadSByte();
        }
        
    }
    
}