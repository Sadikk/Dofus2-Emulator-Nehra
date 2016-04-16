









// Generated on 07/24/2015 23:20:00
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class LockableUseCodeMessage : Message
    {
        public const ushort Id = 5667;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public string code;
        
        public LockableUseCodeMessage()
        {
        }
        
        public LockableUseCodeMessage(string code)
        {
            this.code = code;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteUTF(code);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            code = reader.ReadUTF();
        }
        
    }
    
}