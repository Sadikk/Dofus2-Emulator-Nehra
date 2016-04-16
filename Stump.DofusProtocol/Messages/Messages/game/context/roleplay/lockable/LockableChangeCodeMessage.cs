









// Generated on 07/24/2015 23:19:59
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class LockableChangeCodeMessage : Message
    {
        public const ushort Id = 5666;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public string code;
        
        public LockableChangeCodeMessage()
        {
        }
        
        public LockableChangeCodeMessage(string code)
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