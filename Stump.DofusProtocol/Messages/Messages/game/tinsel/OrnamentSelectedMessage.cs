









// Generated on 07/24/2015 23:20:17
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class OrnamentSelectedMessage : Message
    {
        public const ushort Id = 6369;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public ushort ornamentId;
        
        public OrnamentSelectedMessage()
        {
        }
        
        public OrnamentSelectedMessage(ushort ornamentId)
        {
            this.ornamentId = ornamentId;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteVarUhShort(ornamentId);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            ornamentId = reader.ReadVarUhShort();
            if (ornamentId < 0)
                throw new Exception("Forbidden value on ornamentId = " + ornamentId + ", it doesn't respect the following condition : ornamentId < 0");
        }
        
    }
    
}