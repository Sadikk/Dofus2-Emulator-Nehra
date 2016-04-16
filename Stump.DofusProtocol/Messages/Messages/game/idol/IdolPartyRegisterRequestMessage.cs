









// Generated on 07/24/2015 23:20:07
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class IdolPartyRegisterRequestMessage : Message
    {
        public const ushort Id = 6582;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public bool register;
        
        public IdolPartyRegisterRequestMessage()
        {
        }
        
        public IdolPartyRegisterRequestMessage(bool register)
        {
            this.register = register;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteBoolean(register);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            register = reader.ReadBoolean();
        }
        
    }
    
}