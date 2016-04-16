









// Generated on 07/24/2015 23:20:16
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class CinematicMessage : Message
    {
        public const ushort Id = 6053;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public ushort cinematicId;
        
        public CinematicMessage()
        {
        }
        
        public CinematicMessage(ushort cinematicId)
        {
            this.cinematicId = cinematicId;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteVarUhShort(cinematicId);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            cinematicId = reader.ReadVarUhShort();
            if (cinematicId < 0)
                throw new Exception("Forbidden value on cinematicId = " + cinematicId + ", it doesn't respect the following condition : cinematicId < 0");
        }
        
    }
    
}