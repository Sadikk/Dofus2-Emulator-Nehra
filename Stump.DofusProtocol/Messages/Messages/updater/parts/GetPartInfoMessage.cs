









// Generated on 07/24/2015 23:20:18
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class GetPartInfoMessage : Message
    {
        public const ushort Id = 1506;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public string id;
        
        public GetPartInfoMessage()
        {
        }
        
        public GetPartInfoMessage(string id)
        {
            this.id = id;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteUTF(id);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            id = reader.ReadUTF();
        }
        
    }
    
}