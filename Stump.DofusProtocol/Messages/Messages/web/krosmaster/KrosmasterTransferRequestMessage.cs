









// Generated on 07/24/2015 23:20:19
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class KrosmasterTransferRequestMessage : Message
    {
        public const ushort Id = 6349;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public string uid;
        
        public KrosmasterTransferRequestMessage()
        {
        }
        
        public KrosmasterTransferRequestMessage(string uid)
        {
            this.uid = uid;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteUTF(uid);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            uid = reader.ReadUTF();
        }
        
    }
    
}